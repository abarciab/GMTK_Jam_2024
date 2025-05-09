using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum PlayerState { WALK, CLIMB, GLIDE, STUNNED}
[RequireComponent(typeof(PlayerRunWalkBehavior))]
[RequireComponent(typeof(PlayerGlideBehavior))]
[RequireComponent(typeof(PlayerClimbBehavior))]
[RequireComponent(typeof(PlayerStunnedBehavior))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerSounds))]
public class PlayerController : MonoBehaviour
{
    [Header("State")]
    [SerializeField, ReadOnly] private PlayerState _currentState = PlayerState.WALK;

    [Header("Shared")]
    [SerializeField] private float _rotateSpeed = 8;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private Vector3 _groundCheckOffset;
    [SerializeField] private LayerMask _nonPlayerLayers;
    [SerializeField] private float _chargeCooldown;
    [SerializeField] private bool _hasGlider;
    private float _timeWhenLastBoosted;

    public ParticleSystem _teleportParticles;

    [HideInInspector] public Rigidbody RB;

    private PlayerRunWalkBehavior _runWalkBehavior;
    private PlayerClimbBehavior _climbBehavior;
    private PlayerGlideBehavior _glideBehavior;
    private PlayerStunnedBehavior _stunBehavior;

    public PlayerSounds Sounds { get; private set; }
    public bool IsRunning => _currentState == PlayerState.WALK && _runWalkBehavior.IsRunning;
    public bool IsStunned => _currentState == PlayerState.STUNNED;
    public bool IsGliding => _currentState == PlayerState.GLIDE;
    public float GlideSpeedPercent => _glideBehavior.GlideSpeedPercent;
    private bool _resetFromGlideInturrupt => _runWalkBehavior.HasBeenGrounded || _glideBehavior.InturruptPoint.y - transform.position.y > 1f;
    private bool _canGlideCurrent => _resetFromGlideInturrupt && _currentState == PlayerState.WALK && !_runWalkBehavior.IsCoyoteGrounded && RB.linearVelocity.y < 0;
    public void ApplyFallingGravity() => _runWalkBehavior.ApplyFallingGravity();
    public float DistanceTo(Vector3 pos) => Vector3.Distance(transform.position, pos);
    public void Shock() => ChangeState(PlayerState.STUNNED);
    public Collider[] GetCollidersBelow() => Physics.OverlapSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius);
    public bool CanGlide => _canGlideCurrent && Time.time - _timeWhenCantGlide > _glideBehavior.MinGlideTimeReq && DistanceDown() > _glideBehavior.MinGlideDistReq && _hasGlider;

    private float _timeWhenCantGlide;

    private void Awake()
    {
        FindFirstObjectByType<GameManager>().Player = this;
        _runWalkBehavior = GetComponent<PlayerRunWalkBehavior>();
        _runWalkBehavior.HasBeenGrounded = true;
    }

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        Sounds = GetComponent<PlayerSounds>();
        _climbBehavior = GetComponent<PlayerClimbBehavior>();
        _glideBehavior = GetComponent<PlayerGlideBehavior>();
        _stunBehavior = GetComponent<PlayerStunnedBehavior>();
    }

    private void Update()
    {
        CheckStateTransitions();
        transform.SetLossyScale(Vector3.one);
        if (!_canGlideCurrent) _timeWhenCantGlide = Time.time;

#if UNITY_EDITOR
        if (InputController.GetDown(Control.DEBUG)) EnableGlider();
#endif

        if (Time.time - _timeWhenLastBoosted > _chargeCooldown && GameManager.i.WindCharges > 0 && Input.GetMouseButtonDown(0)) UseWindCharge();
    }

    public void EnableGlider()
    {
        _hasGlider = true;
    }

    private void UseWindCharge()
    {
        _timeWhenLastBoosted = Time.time;
#if UNITY_EDITOR
        _timeWhenLastBoosted = 0;
#endif

        if (_glideBehavior.enabled) _glideBehavior.Boost();
        else if (_runWalkBehavior.IsCoyoteGrounded) _runWalkBehavior.BoostJump();
        else return;

        GameManager.i.RemoveCharge();
        Sounds.Get(PlayerSoundKey.WIND_CHARGE).Play();
    }

    private async void OnDisable()
    {
        if (GetComponentInParent<MovingPlatform>()) {
            await Task.Delay(100);

            transform.SetParent(null);
            gameObject.SetActive(true);
        }
    }

    private float DistanceDown()
    {
        bool hit = Physics.Raycast(transform.position, Vector3.down, out var hitData, 100, _nonPlayerLayers);
        if (!hit) return Mathf.Infinity;

        return transform.position.y - hitData.point.y;
    }

    public void PassiveBounce(float mult = 1)
    {
        ChangeState(PlayerState.WALK);
        _runWalkBehavior.Bouce(mult);
    }

    private void CheckStateTransitions() {
        if (ShouldStartGliding()) ChangeState(PlayerState.GLIDE);
        if (ShouldStartClimbing()) ChangeState(PlayerState.CLIMB);
    }

    public void ChangeState(PlayerState newState) {
        if (_currentState == newState) return;
        var oldstate = _currentState;
        _currentState = newState;

        _stunBehavior.enabled = newState == PlayerState.STUNNED;
        _runWalkBehavior.enabled = newState == PlayerState.WALK;
        _climbBehavior.enabled = newState == PlayerState.CLIMB;
        _glideBehavior.enabled = newState == PlayerState.GLIDE;

        if (oldstate == PlayerState.CLIMB && newState == PlayerState.WALK) _runWalkBehavior.HasBeenGrounded = true;
        if (newState == PlayerState.STUNNED) _runWalkBehavior.HasBeenGrounded = true;

        var cam = GameManager.i.Camera.GetComponent<CameraController>();
        cam.SetFlyParticles(0, newState == PlayerState.GLIDE);
    }

    private bool ShouldStartGliding() {
        return (CanGlide && Input.GetMouseButton(1));
    }

    private bool ShouldStartClimbing() {
        return _climbBehavior.ReadyToClimb && InputController.Get(Control.MOVE_FORWARD);
        //should take into account facing direction and position relative to top of ladder. probaby a raycast
    }

    public void SetClosestLadder(Ladder ladder) {
        _climbBehavior.SetClosestLadder(ladder);
        var pos = transform.position;
        transform.position = pos;
    }

    public void ClearLadder(Ladder ladder) {
        if (_climbBehavior.CurrentLadder != ladder) return;
        _climbBehavior.ClearLadder();
        if (ladder.IsRope) transform.SetParent(null);
        ChangeState(PlayerState.WALK);
    }

    public void JumpOffRope() {
        ClearLadder(_climbBehavior.CurrentLadder);
        _runWalkBehavior.Jump(true);
    }

    public Vector2 GetInputDir()
    {
        var inputDir = Vector2.zero;
        if (InputController.Get(Control.MOVE_FORWARD)) inputDir += Vector2.up;
        if (InputController.Get(Control.MOVE_BACK)) inputDir += Vector2.down;
        if (InputController.Get(Control.MOVE_RIGHT)) inputDir += Vector2.right;
        if (InputController.Get(Control.MOVE_LEFT)) inputDir += Vector2.left;
        return inputDir;
    }

    public void Rotate(float speedMod = 1) {
        var mouseX = Input.GetAxis("Mouse X");

        var rotDelta = (_rotateSpeed * speedMod) * 100 * mouseX * Time.deltaTime * Settings.MouseSensetivity;

        transform.Rotate(rotDelta * Vector3.up);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius);
    }
}
