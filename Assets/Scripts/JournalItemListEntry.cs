using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalItemListEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _sprite;

    private JournalController _controller;
    private JournalItem _data;

    public void Initialize(JournalItem data, JournalController controller)
    {
        _data = data;
        _controller = controller;

        _nameText.text = data.Name;
        _sprite.sprite = data.Sprite;   
    }

    public void Select()
    {
        _controller.Select(_data);
    }
}
