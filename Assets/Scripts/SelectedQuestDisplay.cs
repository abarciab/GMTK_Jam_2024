using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectedQuestDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private GameObject _objectivePrefab;
    [SerializeField] private Transform _listParent;
    [SerializeField] private QuestUIController _controller;

    private Quest _quest;
    private List<QuestObjectiveDisplay> _spawnedObjectives = new();

    public void ShowQuest(Quest quest)
    {
        gameObject.SetActive(true);
        _quest = quest;
        _titleText.text = _quest.Name;
        _descriptionText.text = _quest.Description;

        foreach (var s in _spawnedObjectives) Destroy(s.gameObject);
        _spawnedObjectives.Clear();

        foreach (var o in quest.Objectives) DisplayObjective(o);
        _listParent.GetComponent<SingleButtonSelector>().Initialize(true);
    }

    private void DisplayObjective(QuestObjective objective)
    {
        var newObjective = Instantiate(_objectivePrefab, _listParent).GetComponent<QuestObjectiveDisplay>();
        newObjective.Initialize(objective, this);
        _spawnedObjectives.Add(newObjective);
    }

    public void ShowObjective(QuestObjective objective)
    {
        _controller.ShowObjective(objective);
    }
}
