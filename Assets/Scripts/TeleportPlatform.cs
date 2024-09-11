using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class TeleportPlatform : MonoBehaviour
{
    [SerializeField] private float _cooldown = 0.05f;
    [SerializeField] private Vector3 _teleportOffset;
    [SerializeField] private TeleportPlatform _target;
    [SerializeField] private bool _2Way;
    [SerializeField] private Sound _sound;

    private float _currentCooldown;
    private Collider _collider;
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
        _currentCooldown -= Time.deltaTime;
        if (_ready) {
            UIManager.i.SetInteractPromptEnabled(true, gameObject, "teleport");
            if (InputController.GetDown(Control.INTERACT)) Teleport();
        }
        else UIManager.i.SetInteractPromptEnabled(false, gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>()) _playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_playerInRange) return;
        if (other.GetComponent<PlayerController>()) _playerInRange = false;
    }

    private void Teleport()
    {
        _sound.Play();
        _playerInRange = false;
        _currentCooldown = _cooldown;
        GameManager.i.Player.transform.position = _target.TeleportPoint;
    }

    private void OnDrawGizmosSelected()
    {
        if (_target == null) return;
        Gizmos.DrawWireSphere(TeleportPoint, 0.1f);
        Gizmos.DrawLine(transform.position, _target.TeleportPoint + Vector3.down * 0.5f);
    }
}
