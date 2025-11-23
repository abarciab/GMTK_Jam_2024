using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum JournalItemType { ITEM, CONVERSATION }

[System.Serializable]
public class QuestObjective
{
    public string Name;
    [TextArea(2, 2)] public string Description;
    [TextArea(5, 10)] public string LongDescription;
    [TextArea(5, 10)] public string Lore;
}

[System.Serializable]
public class Quest
{
    public string Name;
    public string Description;
    public List<QuestObjective> Objectives = new();
}

[System.Serializable]
public class JournalItem
{
    public string Name;
    public Sprite Sprite;
    [TextArea(3, 10)] public string Content;
    public JournalItemType Type = JournalItemType.ITEM;

    public override bool Equals(object obj)
    {
        if (obj is JournalItem other) {
            return string.Equals(Content, other.Content);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Content.GetHashCode();
    }
}

public class JournalController : MonoBehaviour
{
    [SerializeField] private List<JournalItem> _allItems;
    [SerializeField] private GameObject _listItemPrefab;
    [SerializeField] private Transform _listParent;
    [SerializeField] private JournalSelectedItemDisplay _selectedDisplay;
    [SerializeField] private GameObject _conversationParent;
    [SerializeField] private JournalItemsUI _itemParent;
    [SerializeField] private QuestUIController _questParent;

    [Header("Testing")]
    [SerializeField] private List<Quest> _quests = new();

    private List<JournalItemListEntry> _spawnedEntries = new();

    private void Start()
    {
        ShowQuest();
    }

    private void OnEnable()
    {
        if (_questParent.gameObject.activeInHierarchy) ShowQuest();
        if (_conversationParent.activeInHierarchy) ShowConversations();
        if (_itemParent.gameObject.activeInHierarchy) ShowItems();
        if (GameManager.i) GameManager.i.OpenMenu(true);
        else gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (GameManager.i) GameManager.i.CloseMenu();
    }

    public void ShowItems()
    {
        var items = _allItems.Where(x => x.Type == JournalItemType.ITEM).ToList();
        _itemParent.Initialize(items);
        _conversationParent.SetActive(false);
        _questParent.gameObject.SetActive(false);
    }
    public void ShowConversations()
    {
        BuildList(_allItems.Where(x => x.Type == JournalItemType.CONVERSATION).ToList());
        _conversationParent.SetActive(true);
        _itemParent.gameObject.SetActive(false);
        _questParent.gameObject.SetActive(false);
    }

    public void ShowQuest()
    {
        _questParent.Initialize(_quests);
        _itemParent.gameObject.SetActive(false);
        _conversationParent.SetActive(false);
    }

    public void AddConversation(ConversationData conversation)
    {
        var item = new JournalItem();
        var allLines = string.Join("\n", conversation.Lines.Select(x => x.Text));
        item.Content = allLines;
        item.Type = JournalItemType.CONVERSATION;

        AddItem(item);
    }

    public void AddItem(JournalItem item)
    {
        if (!_allItems.Contains(item)) _allItems.Add(item);
    }

    public void Toggle()
    {
        if (gameObject.activeInHierarchy) Close();
        else Open();
    }

    private void Close()
    {
        gameObject.SetActive(false);
        GameManager.i.CloseMenu();
    }

    private void Open()
    {
        gameObject.SetActive(true);
        GameManager.i.OpenMenu(true);
    }

    public void Select(JournalItem item)
    {
        _selectedDisplay.DisplayItem(item);
    }

    private void BuildList(List<JournalItem> itemsToShow)
    {
        foreach (var e in _spawnedEntries) Destroy(e.gameObject);
        _spawnedEntries.Clear();

        foreach (var i in itemsToShow) SpawnEntry(i);
    }

    private void SpawnEntry(JournalItem item)
    {
        var newEntry = Instantiate(_listItemPrefab, _listParent).GetComponent<JournalItemListEntry>();
        newEntry.Initialize(item, this);

        _spawnedEntries.Add(newEntry);
    }
}
