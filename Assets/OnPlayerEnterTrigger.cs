using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnPlayerEnterTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent _onEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>()) _onEnter.Invoke();
    }

}
