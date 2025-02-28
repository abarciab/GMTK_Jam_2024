using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnPlayerEnterTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent _onStart;
    [SerializeField] private UnityEvent _onEnter;
    [SerializeField] private bool _destroyAfterInvoke = false;
    [SerializeField] private bool _destroyBehaviorAfterInvoke = false;

    private void Start()
    {
        _onStart.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<PlayerController>()) return;
        
        _onEnter.Invoke();
        if (_destroyAfterInvoke) Destroy(gameObject);
        if (_destroyBehaviorAfterInvoke) Destroy(this);
    }
}