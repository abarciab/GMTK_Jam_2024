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
    [SerializeField, ConditionalField(nameof(IsRope))] private float _startOffset;
    [SerializeField, ConditionalField(nameof(IsRope))] private LayerMask _groundLayer;
    [SerializeField, ConditionalField(nameof(IsRope))] private Collider _solidCollider;
    [SerializeField, ConditionalField(nameof(IsRope))] private UnityEvent _OnUncoil;
    [SerializeField, ConditionalField(nameof(IsRope))] private DistrubuteModelAlongLength _ropeScript;

    private bool _isCoiled = true;
    private bool _playerClimbing;

    private void Update()
    {
        if (IsRope) {
            var rot = transform.eulerAngles;
            rot.x = rot.z = 0;
            transform.eulerAngles = rot;
            _solidCollider.enabled = _playerClimbing;
        }
    }

    private void OnDestroy()
    {
        if (_ropeScript && _ropeScript.gameObject) Destroy(_ropeScript.gameObject);
    }

    public void Uncoil()
    {
        bool didHit = Physics.Raycast(transform.position + (Vector3.down * _startOffset), Vector3.down, out var hitData, _maxLength, _groundLayer);
        float targetLength = _maxLength;
        if (didHit) targetLength = hitData.distance - 1 + _startOffset;
        var scale = transform.localScale;
        scale.y = targetLength;
        transform.localScale = scale;
        _isCoiled = false;
        _OnUncoil.Invoke();
        _ropeScript.AddRope(targetLength);
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
        Gizmos.DrawLine(transform.position + Vector3.down * _startOffset, transform.position + Vector3.down * _maxLength);
    }
}
