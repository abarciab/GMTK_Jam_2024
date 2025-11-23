using TMPro;
using UnityEngine;

public class QuestTitleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;

    private Quest _quest;
    private QuestUIController _controller;

    public void Initialize(Quest quest, QuestUIController controller)
    {
        _quest = quest;
        _controller = controller;
        _title.text = quest.Name;
    }

    public void Select()
    {
        _controller.DisplayQuestInfo(_quest);
    }

}
