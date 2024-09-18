using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CheckBox : MonoBehaviour
{
    [SerializeField] private GameObject _check;
    [SerializeField] private Image _checkboxImg;
    [SerializeField] private bool _checked = false;

    [SerializeField] private UnityEvent OnToggleOn;
    [SerializeField] private UnityEvent OnToggleOff;

    private void Start()
    {
        _check.SetActive(_checked);
    }

    public void Toggle()
    {
        if (_checked) ToggleOff();
        else ToggleOn();
    }

    private void ToggleOn()
    {
        _checked = true;
        OnToggleOn.Invoke();
        _check.SetActive(true);
    }

    private void ToggleOff()
    {
        _checked = false;
        OnToggleOff.Invoke();
        _check.SetActive(false);
    }

}
