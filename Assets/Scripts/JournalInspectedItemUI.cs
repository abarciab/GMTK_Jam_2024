using UnityEngine;
using UnityEngine.UI;

public class JournalInspectedItemUI : MonoBehaviour
{
    [SerializeField] private Image _image;

    private JournalItem _item;
    private JournalInspectedItemDisplay _display;

    public void Initialize(JournalItem item, JournalInspectedItemDisplay display)
    {
        _display = display;
        _item = item;
        _image.sprite = item.Sprite;
    }

    public void Select()
    {
        _display.ShowItem(_item);
    }
}
