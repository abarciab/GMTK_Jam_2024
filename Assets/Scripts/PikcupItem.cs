using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PikcupItem : MonoBehaviour
{
    [SerializeField] private float _pickupRadius;
    [SerializeField] private UnityEvent _onPickup = new UnityEvent();
    [SerializeField] private bool _windCharge;
    [SerializeField] private Sound _pickupSound;
    [SerializeField, Range(0, 1)] private float _spawnPercent = 0.25f;

    Transform _player;

    private void Start()
    {
        if (Random.Range(0f ,1) >= _spawnPercent) {
            Destroy(gameObject);
            return;
        }

        _pickupSound = Instantiate(_pickupSound);
        _player = GameManager.i.Player.transform;
        if (_windCharge) _onPickup.AddListener(GameManager.i.AddSingleCharge);
    }

    private void Update()
    {
        var dist = Vector3.Distance(transform.position, _player.position);
        if (dist < _pickupRadius) Pickup();
    }

    private void Pickup()
    {
        _pickupRadius = -1;

        _pickupSound.Play();
        _onPickup.Invoke();
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _pickupRadius);
    }
}
