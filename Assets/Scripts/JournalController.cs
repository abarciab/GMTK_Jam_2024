using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class JournalItem
{
    public string Name;
    public Sprite Sprite;
    [TextArea(3, 10)] public string Content;

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

    private List<JournalItemListEntry> _spawnedEntries = new List<JournalItemListEntry>();

    private void OnEnable()
    {
        BuildList();
    }

    public void AddConversation(ConversationData conversation)
    {
        var item = new JournalItem();
        var allLines = string.Join("\n", conversation.Lines.Select(x => x.Text));
        item.Content = allLines;

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

    private void BuildList()
    {
        foreach (var e in _spawnedEntries) Destroy(e.gameObject);
        _spawnedEntries.Clear();

        foreach (var i in _allItems) SpawnEntry(i);
    }

    private void SpawnEntry(JournalItem item)
    {
        var newEntry = Instantiate(_listItemPrefab, _listParent).GetComponent<JournalItemListEntry>();
        newEntry.Initialize(item, this);

        _spawnedEntries.Add(newEntry);
    }
}
