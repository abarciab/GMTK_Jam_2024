using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerRunWalkBehavior : MonoBehaviour
{
    [Header("moveSpeed")]
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _runSpeed = 10;
    [SerializeField, Range(0, 1)] private float _airSpeedMod = 0.5f;
    [SerializeField, Range(0, 1)] private float _strafeSpeedMod = 0.8f;

    [Header("Jumping")]
    [SerializeField] private int _numJumps = 1;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _boostJumpForce = 400;
    [SerializeField] private float _coyoteTime = 1f;
    [SerializeField] private int _groundLayer;

    [Header("gravity")]
    [SerializeField] private float _topOfJumpThreshold = 0.1f;
    [SerializeField] private float _jumpingUpGravity = 2;
    [SerializeField] private Vector2 _fallingGravityMinMax = new Vector2(15, 25);
    [SerializeField] private float _minFallAccelerationTime = 1;
    [SerializeField] private float _timeToMaxFallGravity = 2f;

    [Header("Misc")]
    [SerializeField] private Vector2 _groundedAndUngroundedDrag = new Vector2(4, 1);
    [SerializeField] private float _groundDragNoInputIncrease = 2;
    [SerializeField] private bool _debug;
    [SerializeField, ReadOnly, ConditionalField(nameof(_debug))] private GameObject _currentGroundObj;

    [HideInInspector] public bool HasBeenGrounded;
    [HideInInspector] public bool IsRunning => _isRunning;
    [HideInInspector] public bool IsCoyoteGrounded => _isCoyoteGrounded;

    private PlayerController _controller;
    private bool _isRunning;
    private float _lastJumpTime;
    private float _timeWhenLastGrounded;
    private int _numJumpsLeft;
    private bool _onMovingPlatform;
    private float _timeWhenTopOfJump;

    private bool _isGrounded => _currentGroundObj != null;
    private Rigidbody _rb => _controller.RB;
    private PlayerSounds _sounds => _controller.Sounds; 
    private bool _isCoyoteGrounded => _isGrounded || (_numJumpsLeft > 0 && (Time.time - _timeWhenLastGrounded) < _coyoteTime);
    private void ApplySpecificGravity(float amount) => _rb.linearVelocity += 10 * amount * Time.deltaTime * Vector3.down;
    public void ApplyFallingGravity() => ApplySpecificGravity(_fallingGravityMinMax.x);

    private void OnEnable() {
        HasBeenGrounded = false;
    }

    private void Start() {
        _controller = GetComponent<PlayerController>();
        _lastJumpTime = Time.time;
        UpdateIsGrounded(false);
    }

    private void Update() {
        _controller.Rotate();
        UpdateIsGrounded();

        if (_isCoyoteGrounded && InputController.GetDown(Control.JUMP)) Jump();
        WalkRun();
        if (!_isGrounded) ApplyGravity();
    }

    public void BoostJump()
    {
        Jump(boost: true);
    }

    private void ApplyGravity()
    {
        if (Mathf.Abs(_rb.linearVelocity.y) < _topOfJumpThreshold) _timeWhenTopOfJump = Time.time;

        if (_rb.linearVelocity.y > _topOfJumpThreshold) {
            ApplySpecificGravity(_jumpingUpGravity);
        }
        else if (_rb.linearVelocity.y < -_topOfJumpThreshold) {
            float timeSinceTopOfJump = Time.time - _timeWhenTopOfJump;
            float progress = Mathf.InverseLerp(_minFallAccelerationTime, _timeToMaxFallGravity, timeSinceTopOfJump);
            ApplySpecificGravity(Mathf.Lerp(_fallingGravityMinMax.x, _fallingGravityMinMax.y, progress));
        }
    }

    public async void Bouce(float speed)
    {
        if (speed < 0.5f) return;
        _sounds.Get(PlayerSoundKey.BOUNCE).Play();
        _numJumpsLeft = 0;
        var vel = _rb.linearVelocity;
        vel.y = speed;
        _rb.linearVelocity = vel;
        await Task.Delay(10);
        _rb.linearVelocity = vel;
    }

    public void UpdateIsGrounded(bool canLand = true) {
        bool WasGrounded = _isGrounded;
        var colliders = _controller.GetCollidersBelow().Where(x => !x.isTrigger && !x.GetComponent<PlayerController>() && x.gameObject.layer == _groundLayer).ToList();
        _currentGroundObj = colliders.Count > 0 ? colliders[0].gameObject : null;

        if (!_isGrounded) {
            _onMovingPlatform = false;
            return;
        }
        else {
            HasBeenGrounded = true;
            _timeWhenLastGrounded = Time.time;
        }

        _onMovingPlatform = _currentGroundObj.GetComponentInParent<MovingPlatform>() != null;

        GameManager.i.UpdateCurrentTower(_currentGroundObj.GetComponentInParent<TowerController>());
        if (!WasGrounded && canLand) Land();
        HandleDecayingPlatform();
        
        SetDrag();
    }

    private void HandleDecayingPlatform() {
        var decayingPlatform = _currentGroundObj.GetComponentInParent<DecayingPlatform>();
        if (decayingPlatform) {
            _currentGroundObj = decayingPlatform.gameObject;
            decayingPlatform.StartStandingOnPlatform();
        }
    }

    private void Land() {
        _numJumpsLeft = _numJumps;
        _sounds.Get(PlayerSoundKey.JUMP_LAND).Play(); 
        transform.parent = _onMovingPlatform ? _currentGroundObj.transform : null;
    }


    private void SetDrag() {
        bool pressingInput = _controller.GetInputDir().magnitude > 0;
        var groundedDrag = _groundedAndUngroundedDrag.x + (pressingInput ? 0 : _groundDragNoInputIncrease);
        var ungroundedDrag = _groundedAndUngroundedDrag.y;
        _rb.linearDamping = _isGrounded ? groundedDrag : ungroundedDrag;
    }

    private void WalkRun() {
        Vector3 inputDir = _controller.GetInputDir().normalized;
        if (inputDir == Vector3.zero) {
            _isRunning = false;
            return;
        }

        if (!_isGrounded) inputDir *= _airSpeedMod;
        _isRunning = InputController.Get(Control.RUN);
        
        var moveDir = _rb.linearVelocity;
        AddSpeedAlongAxisWithLimits(inputDir.x, transform.right, ref moveDir);
        AddSpeedAlongAxisWithLimits(inputDir.y, transform.forward, ref moveDir);
        _rb.linearVelocity = moveDir;
    }

    private void AddSpeedAlongAxisWithLimits(float inputVel, Vector3 Axis, ref Vector3 currentTotalVel) {
        var speed = _isRunning ? _runSpeed : _walkSpeed;
        var currentVel = Vector3.Dot(currentTotalVel, Axis);
        float acceleration = speed * 10f * Time.deltaTime;
        bool changingDirection = Mathf.Sign(currentVel) != Mathf.Sign(inputVel);
        bool notAtMaxSpeed = Mathf.Abs(inputVel) < speed;
        if (changingDirection || notAtMaxSpeed) currentTotalVel += inputVel * acceleration * Axis;
    }

    public void Jump(bool overrideConditions = false, bool boost = false) {
        if (!overrideConditions) {
            float timeSinceLastJump = Time.time - _lastJumpTime;
            if (_numJumpsLeft <= 0) return;
        }

        transform.SetParent(null);
        _numJumpsLeft -= 1;
        _sounds.Get(PlayerSoundKey.JUMP).Play(restart: false);
        var vel = _rb.linearVelocity;
        vel.y = boost ? _boostJumpForce : _jumpForce;
        _rb.linearVelocity = vel;
        _lastJumpTime = Time.time;
        _timeWhenLastGrounded = Time.time;
    }
}
