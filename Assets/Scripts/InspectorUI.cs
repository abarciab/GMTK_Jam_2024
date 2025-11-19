using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectorUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private Image _previewImg;


    public void InspectItem(string name, string description, Sprite icon)
    {
        if (gameObject.activeInHierarchy) return;

        gameObject.SetActive(true);
        GameManager.i.OpenMenu(true);

        _nameText.text = name;
        _mainText.text = description;
        _previewImg.sprite = icon;
    }

    public void Close()
    {
        var item = new JournalItem();

        item.Name = _nameText.text;
        item.Content = _mainText.text;
        item.Sprite = _previewImg.sprite;
        UIManager.i.Journal.AddItem(item);

        GameManager.i.CloseMenu();
        gameObject.SetActive(false);
    }
}
