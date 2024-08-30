using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private float _coyoteTime = 1f;
    [SerializeField] private float _jumpingUpGravity = 2;
    [SerializeField] private float _fallingGravity = 5;
    [SerializeField] private int _groundLayer;

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

    private bool _isGrounded => _currentGroundObj != null;
    private Rigidbody _rb => _controller.RB;
    private PlayerSounds _sounds => _controller.Sounds; 
    private bool _isCoyoteGrounded => _isGrounded || (_numJumpsLeft > 0 && Time.time - _timeWhenLastGrounded < _coyoteTime);
    private void ApplyGravity(float amount) => _rb.velocity += 10 * amount * Time.deltaTime * Vector3.down;

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
        if (!_isGrounded) ApplyGravity(_rb.velocity.y > 0.1f ? _jumpingUpGravity : (_rb.velocity.y < 0.1f ? _fallingGravity : 0));
    }

    

    public void UpdateIsGrounded(bool canLand = true) {
        bool WasGrounded = _isGrounded;
        var colliders = _controller.GetCollidersBelow().Where(x => !x.isTrigger && !x.GetComponent<PlayerController>() && x.gameObject.layer == _groundLayer).ToList();
        //foreach (var col in colliders) print("col: " + col.gameObject.name + ", layer: " + col.gameObject.layer + ", groundLayer: " + _groundLayer);
        _currentGroundObj = colliders.Count > 0 ? colliders[0].gameObject : null;
        if (!_isGrounded) {
            _onMovingPlatform = false;
            return;
        }
        else HasBeenGrounded = true;

        bool onBridge = _currentGroundObj.GetComponentInParent<BridgeController>() != null; 
        _onMovingPlatform = _currentGroundObj.GetComponentInParent<MovingPlatform>() != null;

        if (!onBridge) GameManager.i.UpdateCurrentTower(_currentGroundObj.GetComponentInParent<TowerController>());
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
        _rb.drag = _isGrounded ? groundedDrag : ungroundedDrag;
    }

    private void WalkRun() {
        Vector3 inputDir = _controller.GetInputDir().normalized;
        if (inputDir == Vector3.zero) {
            _isRunning = false;
            return;
        }

        if (!_isGrounded) inputDir *= _airSpeedMod;
        _isRunning = InputController.Get(Control.RUN);
        
        var moveDir = _rb.velocity;
        AddSpeedAlongAxisWithLimits(inputDir.x, transform.right, ref moveDir);
        AddSpeedAlongAxisWithLimits(inputDir.y, transform.forward, ref moveDir);
        _rb.velocity = moveDir;
    }

    private void AddSpeedAlongAxisWithLimits(float inputVel, Vector3 Axis, ref Vector3 currentTotalVel) {
        var speed = _isRunning ? _runSpeed : _walkSpeed;
        var currentVel = Vector3.Dot(currentTotalVel, Axis);
        float acceleration = speed * 10f * Time.deltaTime;
        bool changingDirection = Mathf.Sign(currentVel) != Mathf.Sign(inputVel);
        bool notAtMaxSpeed = Mathf.Abs(inputVel) < speed;
        if (changingDirection || notAtMaxSpeed) currentTotalVel += inputVel * acceleration * Axis;
    }

    public void Jump(bool overrideConditions = false) {
        if (!overrideConditions) {
            float timeSinceLastJump = Time.time - _lastJumpTime;
            if (_numJumpsLeft <= 0) return;
        }

        transform.SetParent(null);
        _numJumpsLeft -= 1;
        _sounds.Get(PlayerSoundKey.JUMP).Play(restart: false);
        var vel = _rb.velocity;
        vel.y = _jumpForce;
        _rb.velocity = vel;
        _lastJumpTime = Time.time;
        _timeWhenLastGrounded = Time.time;
    }
}
