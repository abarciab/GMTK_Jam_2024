using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterestPoint : MonoBehaviour
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private bool _isActiveByDefault = true;

    private void Start()
    {
        if (UIManager.i && _isActiveByDefault) UIManager.i.AddPointOfInterest(transform, _icon);
    }

    public void Remove()
    {
        UIManager.i.RemovePointOfInterest(transform);
    }

    void OnDestroy()
    {
        UIManager.i.RemovePointOfInterest(transform);
    }

    public void Activate()
    {
        if(!_isActiveByDefault)
        {
            UIManager.i.AddPointOfInterest(transform, _icon);
        }
    }

}
