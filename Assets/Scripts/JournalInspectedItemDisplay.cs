using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalInspectedItemDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _sprite;

    public void ShowItem(JournalItem item)
    {
        gameObject.SetActive(true);
        _nameText.text = item.Name;
        _descriptionText.text = item.Content;
        _sprite.sprite = item.Sprite;
    }
}
