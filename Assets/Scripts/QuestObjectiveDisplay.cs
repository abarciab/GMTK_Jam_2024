using TMPro;
using UnityEngine;

public class QuestObjectiveDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    private QuestObjective _objective;
    private SelectedQuestDisplay _controller;

    public void Initialize(QuestObjective objective, SelectedQuestDisplay controller)
    {
        _nameText.text = objective.Name;
        _descriptionText.text = objective.Description;
        _objective = objective;
        _controller = controller;
    }

    public void Select()
    {
        _controller.ShowObjective(_objective);
    }
}
