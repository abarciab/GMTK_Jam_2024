using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UnstablePlatform : MonoBehaviour
{
    [SerializeField] private float _shownTime = 2;
    [SerializeField] private float _hiddenTime = 1;
    [SerializeField] private float _startOffset = 0;

    [SerializeField] private Sound _appearSound;
    [SerializeField] private Sound _disapearSound;

    private bool _shown;
    private float _currentCountdown;
    private Vector3 _scale;
    private Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _scale = transform.localScale;
        _currentCountdown = _startOffset + _shownTime;
        _disapearSound = Instantiate(_disapearSound);
        _appearSound = Instantiate(_appearSound);
        _shown = true;
    }

    void Update()
    {
        _currentCountdown -= Time.deltaTime;
        if (_currentCountdown < 0 ) {
            transform.localScale = _shown ? Vector3.zero : _scale;
            _shown = !_shown;
            _collider.enabled = _shown;
            _currentCountdown = _shown ? _shownTime : _hiddenTime;
            if (_shown) _appearSound.Play(transform); else _disapearSound.Play(transform);
            foreach (Transform child in transform) if (!child.GetComponent<AudioSource>()) child.gameObject.SetActive(_shown);
        }
    }
}
