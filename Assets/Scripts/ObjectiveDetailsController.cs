using TMPro;
using UnityEngine;

public class ObjectiveDetailsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _loreText;

    private void OnEnable()
    {
        print("objective set active");
    }

    public void ShowObjective(QuestObjective objective)
    {
        gameObject.SetActive(true);
        _nameText.text = objective.Name;
        _descriptionText.text = objective.LongDescription;
        _loreText.text = objective.Lore;
    }
}
