using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Collider))]
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

    public void Activate()
    {
        if(!_isActiveByDefault)
        {
            UIManager.i.AddPointOfInterest(transform, _icon);
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == GameManager.i.Player.gameObject)
        {
            UIManager.i.RemovePointOfInterest(transform);
            Destroy(gameObject);
        }
    }*/
}
