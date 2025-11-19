using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private float _activationDistance;
    [SerializeField] private UnityEvent _onActivate;
    [SerializeField] private bool _equipGlider;
    [SerializeField] private bool _destroyOnInteract = true;
    [SerializeField] private string _verb;
    [SerializeField] private bool _requireHover;

    private Transform _player;

    private void Start()
    {
        _player = GameManager.i.Player.transform;
    }

    private void Update()
    {
        if (GameManager.i.MenusOpen > 0) return;
        var dist = Vector3.Distance(transform.position, _player.position);
        bool inRange = dist < _activationDistance;

        if (inRange && _requireHover) {

            var mask = 1 << gameObject.layer;
            var didHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo, 1000, mask);
            if (!didHit || hitInfo.collider.GetComponentInParent<Interactable>() != this) {
                inRange = false;
            }
        }

        UIManager.i.SetInteractPromptState(inRange, gameObject, _verb);

        if (inRange && InputController.GetDown(Control.INTERACT)) Activate();
    }

    public void Disable()
    {
        UIManager.i.SetInteractPromptState(false, gameObject, _verb);
        enabled = false;
    }

    private void Activate()
    {
        if (_equipGlider) GameManager.i.Player.EnableGlider();
        UIManager.i.SetInteractPromptState(false, gameObject, _verb);
        if (_destroyOnInteract) Destroy(gameObject);
        _onActivate.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _activationDistance);
    }
}
