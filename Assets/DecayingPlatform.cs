using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[SelectionBase]
public class DecayingPlatform : MonoBehaviour
{
    [SerializeField] private GameObject _collider;
    [SerializeField] private float _decayTime = 3;
    [SerializeField, Tooltip("The time the platform waits to reset after breaking")] private float _delayTime = 2;
    [SerializeField, ReadOnly] private float _timeLeft;
    [SerializeField, ReadOnly] private bool _decaying;

    public void StartStandingOnPlatform()
    {
        if (_decaying) return;
        _timeLeft = _decayTime;
        _decaying = true;
    }

    public void LeavePlatform()
    {
        _decaying = false;
    }

    private void Update()
    {
        if (!_decaying) return;
        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0) BreakPlatform();
    }

    private void BreakPlatform()
    {
        _decaying = false;
        _collider.SetActive(false);
        StartCoroutine(WaitThenReset());
    }

    private IEnumerator WaitThenReset()
    {
        yield return new WaitForSeconds(_delayTime);
        _collider.SetActive(true);
    }
}
