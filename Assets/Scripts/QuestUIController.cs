using System.Collections.Generic;
using UnityEngine;

public class QuestUIController : MonoBehaviour
{
    [SerializeField] private GameObject _titlePrefab;
    [SerializeField] private Transform _titleListParent;
    [SerializeField] private SelectedQuestDisplay _questDisplay;
    [SerializeField] private ObjectiveDetailsController _objectiveDisplay;

    private List<QuestTitleUI> _spawnedQuestTitles = new List<QuestTitleUI>();

    private void OnEnable()
    {
        _questDisplay.gameObject.SetActive(false);
        _objectiveDisplay.gameObject.SetActive(false);
    }

    public void Initialize(List<Quest> quests)
    {
        gameObject.SetActive(true);

        foreach (var s in _spawnedQuestTitles) Destroy(s.gameObject);
        _spawnedQuestTitles.Clear();

        foreach (var q in quests) ShowQuestTitle(q);
        _titleListParent.GetComponent<SingleButtonSelector>().Initialize(true);
    }

    private void ShowQuestTitle(Quest quest)
    {
        var newTitle = Instantiate(_titlePrefab, _titleListParent).GetComponent<QuestTitleUI>();
        newTitle.Initialize(quest, this);
        _spawnedQuestTitles.Add(newTitle);
    }

    public void DisplayQuestInfo(Quest quest)
    {
        _objectiveDisplay.gameObject.SetActive(false);
        _questDisplay.ShowQuest(quest);
    }

    public void ShowObjective(QuestObjective objective)
    {
        _objectiveDisplay.ShowObjective(objective);
    }

}
