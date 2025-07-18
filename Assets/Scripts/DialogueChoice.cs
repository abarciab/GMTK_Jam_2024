using TMPro;
using UnityEngine;

public class DialogueChoice : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textBox;

    private LineData _data;
    private DialogueController _controller;

    public void Initialize(LineData data, DialogueController controller)
    {
        _controller = controller;
        _data = data;
        _textBox.text = data.Text;
    }

    public void Select()
    {
        _controller.SelectChoice(_data);
    }
}
