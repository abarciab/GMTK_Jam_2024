using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[SelectionBase]
public class TeleportPlatform : MonoBehaviour
{
    [SerializeField] private float _cooldown = 0.05f;
    [SerializeField] private Vector3 _teleportOffset;
    [SerializeField] private TeleportPlatform _target;
    [SerializeField] private bool _reciever;
    [SerializeField] private bool _2Way;
    [SerializeField] private Sound _sound;
    [SerializeField] private Transform _particleParent;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private Animator _animator;

    private float _currentCooldown;
    private bool _playerInRange;
    private bool _ready => _target && _playerInRange && _currentCooldown <= 0;
    [HideInInspector] public Vector3 TeleportPoint => transform.TransformPoint(_teleportOffset);

    public void SetTarget(TeleportPlatform newTarget) => _target = newTarget;

    private void OnValidate()
    {
        if (_2Way && _target) _target.SetTarget(this);
    }

    private void Start()
    {
        _sound = Instantiate(_sound);
    }

    private void Update()
    {
        if (_target) _particleParent.LookAt(_target.TeleportPoint + Vector3.up * 1f);

        _currentCooldown -= Time.deltaTime;
        if (_ready) {
            UIManager.i.SetInteractPromptState(true, gameObject, "teleport");
            if (InputController.GetDown(Control.INTERACT)) Teleport();
        }
        else UIManager.i.SetInteractPromptState(false, gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_reciever) return;

        if (other.GetComponent<PlayerController>()) {
            _playerInRange = true;
            SetActive(true);
            if (_target) _target.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_playerInRange) return;
        if (other.GetComponent<PlayerController>()) {
            _playerInRange = false;
            SetActive(false);
            if (_target) _target.SetActive(false); 
        }
    }

    public void SetActive(bool active)
    {
        if (active) _particles.Play();
        else _particles.Stop();
        _animator.SetBool("Active", active);
    }

    public void Recieve()
    {
        if (!_reciever) return;
        SetActive(false);
    }

    private void Teleport()
    {
        _sound.Play();
        _playerInRange = false;
        _currentCooldown = _cooldown;
        GameManager.i.Player.transform.position = _target.TeleportPoint;
        _target.Recieve();
        GameManager.i.Player._teleportParticles.Play();

        SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (_target == null) return;
        Gizmos.DrawWireSphere(TeleportPoint, 0.1f);
        Gizmos.DrawLine(transform.position, _target.TeleportPoint + Vector3.down * 0.5f);
    }
}
