using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [SerializeField] private bool _passiveBounce;
    [SerializeField, ConditionalField(nameof(_passiveBounce))] private float _bounceMult;
    [SerializeField] private bool _activeBounce;
    [SerializeField, ConditionalField(nameof(_activeBounce))] private float _cooldown;
    [SerializeField, ConditionalField(nameof(_activeBounce))] private float _activeForce;
    [SerializeField] private BouncerAnimator _bouncerAnimator;

    private float _currentCooldown;
    private float _storedVel;
    private bool _playerInRange;

    private void OnCollisionEnter(Collision collision)
    {
        if (_passiveBounce && collision.collider.GetComponent<PlayerController>()) Bounce();
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player) _playerInRange = true;

        if (!_passiveBounce) return;
        if (player) _storedVel = other.GetComponent<Rigidbody>().velocity.y;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_playerInRange) return;
        if (other.GetComponent<PlayerController>()) _playerInRange = false;
    }

    private void Update()
    {
        if (!_activeBounce) return;
        _currentCooldown -= Time.deltaTime;
        if (_currentCooldown < 0 && _playerInRange) UIManager.i.SetInteractPromptEnabled(true, gameObject, "bounce");
        else UIManager.i.SetInteractPromptEnabled(false, gameObject);

        if (_currentCooldown < 0 && _playerInRange && InputController.GetDown(Control.INTERACT)) {
            _currentCooldown = _cooldown;
            Bounce(true);
        }
    }

    private void Bounce(bool active = false)
    {
        var vel = _storedVel;
        if (active) vel = -_activeForce;
        if (vel > 0.01f) return;

        _bouncerAnimator.PlayBounce();

        GameManager.i.Player.PassiveBounce(_bounceMult * Mathf.Abs(vel));
    }
}
