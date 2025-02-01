using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemController : MonoBehaviour
{
    [SerializeField] private float _interactDist = 10;
    [SerializeField] private GemAnimator _animatorScript;
    [SerializeField] private Sound _pickupSound;

    private TowerController _tower;
    private Transform _player;

    private void Start()
    {
        if (!GameManager.i || !GameManager.i.Player) {
            enabled = false;
            return;
        }

        _player = GameManager.i.Player.transform;
        _pickupSound = Instantiate(_pickupSound);
        _tower = GetComponentInParent<TowerController>();
    }

    private void Update()
    {
        var dist = Vector3.Distance(transform.position, _player.position);
        bool inRange = dist <= _interactDist;

        _animatorScript.SetIdleParticles(inRange);
        UIManager.i.SetInteractPromptState(inRange, gameObject, "activate");

        if (inRange && InputController.GetDown(Control.INTERACT)) Activate();
    }

    private void Activate()
    {
        enabled = false;
        UIManager.i.SetInteractPromptState(false, gameObject);
        _animatorScript.PickUpGem();
        GameManager.i.CompleteTower(_tower);
        _pickupSound.Play();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _interactDist);
    }
}
