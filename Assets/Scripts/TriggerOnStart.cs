using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnStart : MonoBehaviour
{
    [SerializeField] private UnityEvent _onStart;

    public void ManualTrigger() => _onStart.Invoke();

    private void Start()
    {
        _onStart.Invoke();
    }
}
