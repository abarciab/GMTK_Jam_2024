using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
    [SerializeField] private float _activationDistance;
    [SerializeField] private UnityEvent _OnActivate;
    [SerializeField] private bool _autoDeactivate = true;
    private Transform _player;

    private void Update()
    {
        if (!_player) _player = GameManager.i.Player.transform;

        if (UIManager.i._dialogue.gameObject.activeInHierarchy) return;

        var dist = Vector3.Distance(transform.position, _player.position);
        UIManager.i.SetInteractPromptEnabled(dist < _activationDistance, gameObject);

        if (dist > _activationDistance) return;
        else if (InputController.GetDown(Control.INTERACT)) {
            UIManager.i.SetInteractPromptEnabled(false, gameObject);
            _OnActivate.Invoke();
            if (_autoDeactivate) enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _activationDistance);
    }

}