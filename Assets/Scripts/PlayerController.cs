using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    [Header("Walking, running, turning")]
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _runSpeed = 10;
    [SerializeField, Range(0, 1), Tooltip("What percent of forward speed is used for strafe")] private float _strafeSpeedMod = 0.8f;
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

    [Header("Climbing")]
    [SerializeField] private float _vertClimbSpeed = 3;
    [SerializeField] private float _climbStrafeSpeed = 1;

    [HideInInspector] public bool IsRunning => _isRunning;

    private Vector3 _moveDir;
    private Rigidbody _rb;
    private bool _isGrounded;
    private bool _isRunning;
    private bool _isClimbing;
    private float _lastJumpTime;
    private Ladder _currentLadder;

    private float DistanceTo(Vector3 pos) => Vector3.Distance(transform.position, pos);
    private float _currentLadderDist => _currentLadder == null ? Mathf.Infinity : DistanceTo(_currentLadder.transform.position);

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        GameManager.i.Player = this;
        _lastJumpTime = Time.time;
        _isGrounded = true;

        InitializeSounds();
    }

    public void SetClosestLadder(Ladder ladder)
    {
        if (_currentLadder == null || _currentLadderDist < DistanceTo(ladder.transform.position)) _currentLadder = ladder;
    }

    public void ClearLadder(Ladder ladder)
    {
        if (_currentLadder == ladder) _currentLadder = null;
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
        if (_currentLadder && !_isClimbing) _isClimbing = InputController.Get(Control.MOVE_FORWARD);

        if (_isClimbing) {
            Climb();
            return;
        }

        DoHorizontalMovement();
        UpdateIsGrounded();
        if (_isGrounded && InputController.Get(Control.JUMP)) Jump();
        if (!_isGrounded && _rb.velocity.y > 0.1) ApplyGravity(_jumpingUpGravity);
        if (!_isGrounded && _rb.velocity.y < 0.1) ApplyGravity(_fallingGravity);
    }

    private void Climb()
    {
        if (!_currentLadder) {
            _isClimbing = false;
            return;
        }

        var inputDir = GetInputDir();
        var climbDir = Vector3.zero;
        if (inputDir.y > 0) climbDir += Vector3.up;
        if (inputDir.x > 0) climbDir += transform.right;
        if (inputDir.y < 0) climbDir -= Vector3.up;
        if (inputDir.x < 0) climbDir -= transform.right;
        if (climbDir == Vector3.zero) {
            _isClimbing = false;
            return;
        }

        climbDir.y *= _vertClimbSpeed;
        climbDir.x *= _climbStrafeSpeed;
        _rb.velocity = climbDir;
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
        if (!oldState && _isGrounded) Land();
    }

    private void Land()
    {
        _landSound.Play();
        GameManager.i.Camera.GetComponent<CameraShake>().ShakeManual(3, 0.1f, 0.05f);
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

    private Vector2 GetInputDir()
    {
        var inputDir = Vector2.zero;
        if (InputController.Get(Control.MOVE_FORWARD)) inputDir += Vector2.up;
        if (InputController.Get(Control.MOVE_BACK)) inputDir += Vector2.down;
        if (InputController.Get(Control.MOVE_RIGHT)) inputDir += Vector2.right;
        if (InputController.Get(Control.MOVE_LEFT)) inputDir += Vector2.left;
        return inputDir;
    }


    private Vector3 GetMoveDir()
    {
        _moveDir = Vector3.zero;
        var inputDir = GetInputDir();
        if (inputDir.y > 0) _moveDir += transform.forward;
        if (inputDir.x > 0) _moveDir += transform.right * _strafeSpeedMod;
        if (inputDir.y < 0) _moveDir -= transform.forward;
        if (inputDir.x < 0) _moveDir -= transform.right * _strafeSpeedMod;
        _moveDir = _moveDir.normalized;
        return _moveDir;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius);
    }
}
