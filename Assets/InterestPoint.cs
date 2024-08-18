using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InterestPoint : MonoBehaviour
{
    private void Start()
    {
        UIManager.i.AddPointOfInterest(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == GameManager.i.Player.gameObject)
        {
            UIManager.i.RemovePointOfInterest(transform);
            Destroy(gameObject);
        }
    }
}
