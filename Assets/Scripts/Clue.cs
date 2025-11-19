using UnityEngine;

[RequireComponent (typeof(Interactable))]
public class Clue : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField, TextArea(3, 10)] private string _text;
    [SerializeField] private Sprite _itemSprite;

    public void Inspect()
    {
        //GetComponent<Interactable>().Disable();

        UIManager.i.Inspector.InspectItem(_name, _text, _itemSprite);
    }
}
