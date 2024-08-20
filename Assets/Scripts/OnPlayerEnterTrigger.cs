using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnPlayerEnterTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent _onEnter;
    [SerializeField] private bool _destroyAfterInvoke = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            _onEnter.Invoke();
            if (_destroyAfterInvoke)
            {
                Destroy(gameObject);
            }
        }
    }
}