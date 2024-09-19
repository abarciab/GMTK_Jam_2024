using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerClimbBehavior : MonoBehaviour
{
    [SerializeField] private float _vertClimbSpeed = 8;
    [SerializeField] private float _climbStrafeSpeed = 6;
    [SerializeField] private float _ropSpinSpeed = 25;

    [HideInInspector] public Ladder CurrentLadder => _currentLadder;
    [HideInInspector] public bool ReadyToClimb => _currentLadder && !enabled;

    private PlayerController _controller;
    private Ladder _currentLadder;
    private float _currentLadderDist => _currentLadder == null || _controller == null ? Mathf.Infinity : _controller.DistanceTo(_currentLadder.transform.position);
    private bool _onRope => _currentLadder && _currentLadder.IsRope;
    private Rigidbody _rb => _controller.RB;

    private void OnEnable() {
        if (!_controller) _controller = GetComponent<PlayerController>();
        _rb.isKinematic = false;
    }

    public void SetClosestLadder(Ladder ladder) {
        if (_currentLadder != null && _currentLadderDist < _controller.DistanceTo(ladder.transform.position)) return;

        transform.SetParent(ladder.transform);
        if (ladder.IsRope) _rb.velocity = Vector3.zero;
        _currentLadder = ladder;
    }

    public void ClearLadder() {
        _currentLadder = null;
    }

    private void Update() {
        _controller.Rotate();


        var inputDir = _controller.GetInputDir();
        var climbDir = GetClimbDir(inputDir);

        if (_onRope && InputController.GetDown(Control.JUMP)) _controller.JumpOffRope();
        if (!_currentLadder || (!_onRope && climbDir == Vector3.zero)) _controller.ChangeState(PlayerState.WALK);
        if (!enabled) return;

        GameManager.i.UpdateCurrentTower(_currentLadder.GetComponentInParent<TowerController>());
        if (_currentLadder.IsRope) {
            var rot = Vector3.up * inputDir.x * -_ropSpinSpeed * Time.deltaTime;
            transform.parent.Rotate(rot);
            climbDir.x = 0;
        }
        climbDir.y *= _vertClimbSpeed;
        climbDir.x *= _climbStrafeSpeed;
        _rb.velocity = climbDir;
    }

    private Vector3 GetClimbDir(Vector3 inputDir) {
        var climbDir = Vector3.zero;
        if (inputDir.y > 0) climbDir += Vector3.up;
        if (inputDir.x > 0) climbDir += transform.right;
        if (inputDir.y < 0) climbDir -= Vector3.up;
        if (inputDir.x < 0) climbDir -= transform.right;
        return climbDir;
    }

}
