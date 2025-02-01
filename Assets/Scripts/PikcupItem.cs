using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PikcupItem : MonoBehaviour
{
    [SerializeField] private float _pickupRadius;
    [SerializeField] private UnityEvent _onPickup = new UnityEvent();
    [SerializeField] private bool _windCharge;
    [SerializeField] private bool _standingOnly = true;
    [SerializeField] private Sound _pickupSound;
    [SerializeField, Range(0, 1)] private float _spawnPercent = 0.25f;
    private float _lifetime = 0.1f;
    private bool _willDie;
    private bool _respawned;

    Transform _player;

    private void Start()
    {
        if (!_respawned && Random.Range(0f, 1) >= _spawnPercent) {
            _willDie = true;
        }

        _pickupSound = Instantiate(_pickupSound);
        _player = GameManager.i.Player.transform;
        if (_windCharge) _onPickup.AddListener(GameManager.i.AddSingleCharge);
    }

    public void Respawn()
    {
        _respawned = true;
        _willDie = false;
    }

    private void Update()
    {
        if (_willDie && !_respawned) {
            _lifetime -= Time.deltaTime;
            if (_lifetime <= 0) Destroy(gameObject);
            return;
        }

        if (_standingOnly && GameManager.i.Player.IsGliding) return;
        if (GameManager.i.WindCharges > 1) return;

        var dist = Vector3.Distance(transform.position, _player.position);
        if (dist < _pickupRadius) Pickup();
    }

    private void Pickup()
    {
        _pickupRadius = -1;

        _pickupSound.Play();
        _onPickup.Invoke();
        if (_windCharge) AddRespawnPoint();
        Destroy(gameObject);
    }

    private void AddRespawnPoint()
    {
        var respawnPoint = new GameObject("windCharge respawn point").transform;
        respawnPoint.SetParent(transform.parent);
        respawnPoint.localPosition = transform.localPosition;
        respawnPoint.localScale = transform.localScale;
        GameManager.i.AddRespawnPoint(respawnPoint);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _pickupRadius);
    }
}
