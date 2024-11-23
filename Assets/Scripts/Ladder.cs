using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class Ladder : MonoBehaviour
{
    public bool IsRope;
    [SerializeField, ConditionalField(nameof(IsRope))] private float _maxLength;
    [SerializeField, ConditionalField(nameof(IsRope))] private LayerMask _groundLayer;
    [SerializeField, ConditionalField(nameof(IsRope))] private Collider _solidCollider;
    [SerializeField, ConditionalField(nameof(IsRope))] private UnityEvent _OnUncoil;

    private bool _isCoiled = true;
    private bool _playerClimbing;

    public void Uncoil()
    {
        bool didHit = Physics.Raycast(transform.position, Vector3.down, out var hitData, _maxLength, _groundLayer);
        float targetLength = _maxLength;
        if (didHit) targetLength = hitData.distance - 3;
        var scale = transform.localScale;
        scale.y = targetLength;
        transform.localScale = scale;
        _isCoiled = false;
        _OnUncoil.Invoke();
    }

    private void Update()
    {
        if (IsRope) {
            var rot = transform.eulerAngles;
            rot.x = rot.z = 0;
            transform.eulerAngles = rot;
            _solidCollider.enabled = _playerClimbing;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsRope && _isCoiled) return;
        
        var player = other.GetComponent<PlayerController>();
        if (player) {
            player.SetClosestLadder(this);
            _playerClimbing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player) {
            player.ClearLadder(this);
            _playerClimbing = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _maxLength);
    }
}
