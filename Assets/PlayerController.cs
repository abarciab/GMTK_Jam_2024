using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    [Header("Walking, running, turning")]
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _runSpeed = 10;
    [SerializeField] private float _rotateSpeed;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpingUpGravity = 2;
    [SerializeField] private float _fallingGravity = 5;
    [SerializeField, Tooltip("Minimum time between jumps")] private float _minimumJumpTime = 0.1f;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private Vector3 _groundCheckOffset;
    [SerializeField] private LayerMask _groundLayermask;

    [Header("Sounds")]
    [SerializeField] private Sound _jumpUpSound;
    [SerializeField] private Sound _landSound;

    private Vector3 _moveDir;
    private Rigidbody _rb;
    private bool _isGrounded;
    private bool _isRunning;
    public bool IsRunning => _isRunning;
    private float _lastJumpTime;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        GameManager.i.Player = this;
        _lastJumpTime = Time.time;
        _isGrounded = true;

        InitializeSounds();
    }

    private void InitializeSounds() 
    {
        _jumpUpSound = Instantiate(_jumpUpSound);
        _landSound = Instantiate(_landSound);
    }

    private void Update()
    {
        Rotate();
        Move();
    }

    private void Rotate()
    {
        var mouseX = Input.GetAxis("Mouse X");

        var rot = Vector3.up * mouseX * _rotateSpeed * Time.deltaTime * 100;
        transform.Rotate(rot);
    }

    private void Move()
    {
        DoHorizontalMovement();
        UpdateIsGrounded();
        if (_isGrounded && InputController.Get(Control.JUMP)) Jump();
        if (!_isGrounded && _rb.velocity.y > 0.1) ApplyGravity(_jumpingUpGravity);
        if (!_isGrounded && _rb.velocity.y < 0.1) ApplyGravity(_fallingGravity);
    }

    private void ApplyGravity(float amount)
    {
        _rb.velocity += Vector3.down * amount * Time.deltaTime * 10;
    }

    private void Jump()
    {
        if (Time.time - _lastJumpTime < _minimumJumpTime) return;

        _jumpUpSound.Play();
        _rb.velocity += Vector3.up * _jumpForce;
        _lastJumpTime = Time.time;
    }

    private void UpdateIsGrounded()
    {
        bool oldState = _isGrounded;
        var colliders = Physics.OverlapSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius, _groundLayermask);
        _isGrounded = colliders.Length > 0;
        if (!oldState && _isGrounded) _landSound.Play();
    }

    private void DoHorizontalMovement()
    {
        _moveDir = GetMoveDir();
        _isRunning = InputController.Get(Control.RUN) && _moveDir != Vector3.zero;
        var speed = _isRunning ? _runSpeed : _walkSpeed;
        if (_moveDir != Vector3.zero) {
            var current = _rb.velocity;
            var target = _moveDir * speed;
            target.y = current.y;
            _rb.velocity = target;
        }
    }

    private Vector3 GetMoveDir()
    {
        _moveDir = Vector3.zero;
        if (InputController.Get(Control.MOVE_FORWARD)) _moveDir += transform.forward;
        if (InputController.Get(Control.MOVE_BACK)) _moveDir += transform.forward * -1;
        if (InputController.Get(Control.MOVE_RIGHT)) _moveDir += transform.right;
        if (InputController.Get(Control.MOVE_LEFT)) _moveDir += transform.right * -1;
        _moveDir = _moveDir.normalized;
        return _moveDir;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius);
    }
}
