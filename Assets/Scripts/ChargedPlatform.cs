using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedPlatform : MonoBehaviour
{
    [SerializeField] private float _onTime;
    [SerializeField] private float _offTime;
    [SerializeField] private float _startOffset;
    [SerializeField] private GameObject _chargedGraphic;
    [SerializeField] private Sound _chargeSound;

    private bool _charged;
    private float _currentCountdown;

    private void Start()
    {
        _currentCountdown = _offTime + _startOffset;
        _chargeSound = Instantiate(_chargeSound);
        _chargeSound.PlaySilent(transform);
    }

    private void Update()
    {
        _chargeSound.SetPercentVolume(_charged ? 1 : 0, 0.3f);
        _currentCountdown -= Time.deltaTime;
        if (_currentCountdown < 0) {
            _charged = !_charged;
            _chargedGraphic.SetActive(_charged);
            _currentCountdown = _charged ? _onTime : _offTime;
        }
    }

    private void OnTriggerStay(Collider other) => Check(other);
    private void OnTriggerEnter(Collider other) => Check(other);

    private void Check(Collider other)
    {
        if (_charged && other.GetComponent<PlayerController>()) GameManager.i.Player.Shock();
    }
}
