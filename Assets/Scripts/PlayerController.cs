using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEditorInternal;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    [Header("Walking, running, turning")]
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _runSpeed = 10;
    [SerializeField, Range(0, 1), Tooltip("What percent of forward speed is used for strafe")] private float _strafeSpeedMod = 0.8f;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private Vector2 _groundedAndUngroundedRbDrag = new Vector2(4, 1);

    [Header("Jumping")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpingUpGravity = 2;
    [SerializeField] private float _fallingGravity = 5;
    [SerializeField, Tooltip("Minimum time between jumps")] private float _minimumJumpTime = 0.1f;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private Vector3 _groundCheckOffset;
    [SerializeField] private LayerMask _groundLayermask;

    [Header("Gliding")]
    [SerializeField] private bool _gliderUnlocked;
    [SerializeField] private float _tempConstFlySpeed = 10;


    [Header("Climbing")]
    [SerializeField] private float _vertClimbSpeed = 3;
    [SerializeField] private float _climbStrafeSpeed = 1;

    [Header("Sounds")]
    [SerializeField] private Sound _jumpUpSound;
    [SerializeField] private Sound _landSound;
    [SerializeField] private Sound _openGliderSound;

    [HideInInspector] public bool IsRunning => _isRunning;

    private Vector3 inputDir;
    private Rigidbody _rb;
    private bool _isGrounded;
    private bool _isRunning;
    private bool _isGliding;
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
        _openGliderSound = Instantiate(_openGliderSound);
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
        UpdateIsGrounded();

        if (!_isGliding && !_isGrounded && _rb.velocity.y < 0 && InputController.GetDown(Control.JUMP) && _gliderUnlocked) StartGliding();
        if (_isGliding) {
            Glide();
            return;
        }

        if (_currentLadder && !_isClimbing) _isClimbing = InputController.Get(Control.MOVE_FORWARD);
        if (_isClimbing) {
            Climb();
            return;
        }

        if (_isGrounded && InputController.Get(Control.JUMP)) Jump();
        WalkRun();
        if (!_isGrounded && _rb.velocity.y > 0.1) ApplyGravity(_jumpingUpGravity);
        if (!_isGrounded && _rb.velocity.y < 0.1) ApplyGravity(_fallingGravity);
    }

    private void StartGliding()
    {
        _openGliderSound.Play();
        _isGliding = true;
        _rb.isKinematic = true;
    }

    private void Glide()
    {

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

        GameManager.i.UpdateCurrentTower(_currentLadder.GetComponentInParent<TowerController>());
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
        float timeSinceLastJump = Time.time - _lastJumpTime;
        if (timeSinceLastJump < _minimumJumpTime) return;

        print("jumping. timeSinceLast: " + timeSinceLastJump);
        _jumpUpSound.Play(restart:false);
        _rb.velocity += Vector3.up * _jumpForce;
        _lastJumpTime = Time.time;
        _isGrounded = false;
    }

    private void UpdateIsGrounded()
    {
        bool oldState = _isGrounded;
        var colliders = Physics.OverlapSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius, _groundLayermask);
        _isGrounded = colliders.Length > 0;
        if (_isGrounded) {
            GameManager.i.UpdateCurrentTower(colliders[0].GetComponentInParent<TowerController>());
            if (!oldState) Land();
        }
        _rb.drag = _isGrounded ? _groundedAndUngroundedRbDrag.x : _groundedAndUngroundedRbDrag.y;
    }

    private void Land()
    {
        _landSound.Play();
        GameManager.i.Camera.GetComponent<CameraShake>().ShakeManual(3, 0.1f, 0.05f);
    }

    private void WalkRun()
    {
        inputDir = GetInputDir().normalized;
        _isRunning = InputController.Get(Control.RUN) && inputDir != Vector3.zero;
        var speed = _isRunning ? _runSpeed : _walkSpeed;
        if (inputDir != Vector3.zero) {
            if (!_isGrounded) inputDir *= 0.5f;
            var current = _rb.velocity;
            float speedForward = Vector3.Dot(current, transform.forward);
            float speedRight = Vector3.Dot(current, transform.right);

            var moveDir = current;
            if (Mathf.Sign(inputDir.x) != Mathf.Sign(speedRight) || Mathf.Abs(speedRight) < speed) moveDir += inputDir.x * speed * transform.right; 
            if (Mathf.Sign(inputDir.y) != Mathf.Sign(speedForward) || Mathf.Abs(speedForward) < speed) moveDir += inputDir.y * speed * transform.forward;

            _rb.velocity = moveDir;
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
        this.inputDir = Vector3.zero;
        var inputDir = GetInputDir();
        if (inputDir.y > 0) this.inputDir += transform.forward;
        if (inputDir.x > 0) this.inputDir += transform.right * _strafeSpeedMod;
        if (inputDir.y < 0) this.inputDir -= transform.forward;
        if (inputDir.x < 0) this.inputDir -= transform.right * _strafeSpeedMod;
        this.inputDir = this.inputDir.normalized;
        return this.inputDir;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius);
    }
}
