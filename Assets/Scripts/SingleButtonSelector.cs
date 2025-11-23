using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SingleButtonSelector : MonoBehaviour
{
    List<SelectableItem> _buttons = new List<SelectableItem>();
    [SerializeField] private bool _selectOnEnable = false;
    [SerializeField, ConditionalField(nameof(_selectOnEnable))] private int _onEnableChildIndex = 0;

    bool _initialized = false;

    private void OnEnable()
    {
        if (!_initialized) Initialize();
        if (_selectOnEnable && _buttons.Count > 0) if (_buttons[_onEnableChildIndex] != null)_buttons[_onEnableChildIndex].Select(true, true); 
    }

    public void Initialize(bool force = false)
    {
        if (_initialized && !force) return;
        _buttons = GetComponentsInChildren<SelectableItem>().ToList();
        foreach (var b in _buttons) if (b) b.OnSelect.AddListener(() => DeselectOthers(b));
        _initialized = true;
    }

    private void DeselectOthers(SelectableItem selected)
    {
        foreach (var b in _buttons) if (b && selected != b) b.Deselect();
    }
}
