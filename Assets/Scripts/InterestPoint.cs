using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Collider))]
public class InterestPoint : MonoBehaviour
{
    [SerializeField] private Sprite _icon;

    private void Start()
    {
        if (UIManager.i) UIManager.i.AddPointOfInterest(transform, _icon);
    }

    public void Remove()
    {
        UIManager.i.RemovePointOfInterest(transform);
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
