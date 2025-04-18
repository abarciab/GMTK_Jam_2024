using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[SelectionBase]
public class UnstablePlatform : MonoBehaviour
{
    [SerializeField] private float _shownTime = 2;
    [SerializeField] private float _hiddenTime = 1;
    [SerializeField] private float _startOffset = 0;
    [SerializeField] private float _animationTime = 0.15f;
    [SerializeField] private bool _startState = true;

    [SerializeField] private Sound _appearSound;
    [SerializeField] private Sound _disapearSound;
    [SerializeField] private GameObject _platformParent;
    [SerializeField] private GameObject _lowOpacityParent;
    [SerializeField] private Animator _visbilbleAnimator;
    [SerializeField] private Animator _ghostAnimator;

    private bool _shown;
    private float _currentCountdown;
    private Vector3 _scale;
    private bool _alreadyShook;

    private void OnValidate()
    {
        _platformParent.SetActive(_startState);
    }

    private void Start()
    {
        _scale = transform.localScale;
        _currentCountdown = _startOffset;
        _disapearSound = Instantiate(_disapearSound);
        _appearSound = Instantiate(_appearSound);

        _shown = _startState;
        SetPlatformVisible(_shown);
    }

    void Update()
    {
        _currentCountdown -= Time.deltaTime;

        if (_currentCountdown <= _animationTime && !_alreadyShook) {
            if (_shown) _visbilbleAnimator.SetTrigger("Shake");
            else _ghostAnimator.SetTrigger("Shake");
            _alreadyShook = true;
        }

        if (_currentCountdown < 0 ) {
            _shown = !_shown;
            _currentCountdown = _shown ? _shownTime : _hiddenTime;
            if (_shown) _appearSound.Play(transform); else _disapearSound.Play(transform);
            SetPlatformVisible(_shown);
            _alreadyShook = false;
        }
    }

    private void SetPlatformVisible(bool state)
    {
        _platformParent.SetActive(state);
        _lowOpacityParent.SetActive(!state);
    }
}
