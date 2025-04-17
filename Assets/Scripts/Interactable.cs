using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private float _activationDistance;
    [SerializeField] private UnityEvent _onActivate;
    [SerializeField] private bool _equipGlider;
    [SerializeField] private string _verb;

    private Transform _player;

    private void Start()
    {
        _player = GameManager.i.Player.transform;
    }

    private void Update()
    {
        var dist = Vector3.Distance(transform.position, _player.position);
        bool inRange = dist < _activationDistance;
        UIManager.i.SetInteractPromptState(inRange, gameObject, _verb);

        if (inRange && InputController.GetDown(Control.INTERACT)) Activate();
    }

    private void Activate()
    {
        if (_equipGlider) GameManager.i.Player.EnableGlider();
        UIManager.i.SetInteractPromptState(false, gameObject, _verb);
        Destroy(gameObject);
        _onActivate.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _activationDistance);
    }
}
