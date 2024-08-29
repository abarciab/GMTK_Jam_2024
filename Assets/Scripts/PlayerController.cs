using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public enum PlayerState { WALK, CLIMB, GLIDE}
[RequireComponent(typeof(PlayerRunWalkBehavior))]
[RequireComponent(typeof(PlayerGlideBehavior))]
[RequireComponent(typeof(PlayerClimbBehavior))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerSounds))]
[RequireComponent(typeof(PlayerInventory))]
public class PlayerController : MonoBehaviour
{
    [Header("State")]
    [SerializeField, ReadOnly] private PlayerState _currentState = PlayerState.WALK;

    [HideInInspector] public Rigidbody RB;

    private PlayerRunWalkBehavior _runWalkBehavior;
    private PlayerClimbBehavior _climbBehavior;
    private PlayerGlideBehavior _glideBehavior;
    private PlayerInventory _inventory;

    [HideInInspector] public PlayerSounds Sounds { get; private set; }
    [HideInInspector] public bool IsRunning => _currentState == PlayerState.WALK && _runWalkBehavior.IsRunning;
    [HideInInspector] public bool IsGliding => _currentState == PlayerState.GLIDE;
    [HideInInspector] public float DistanceTo(Vector3 pos) => Vector3.Distance(transform.position, pos);
    [HideInInspector] public float GetGlideSpeedPercent() => _glideBehavior.GetGlideSpeedPercent();
    [HideInInspector] public void SetClosestItem(InventoryItem item) => _inventory.SetClosestItem(item);
    [HideInInspector] public void ClearItem(InventoryItem item) => _inventory.ClearItem(item);

    private void Awake()
    {
        FindObjectOfType<GameManager>().Player = this;
    }

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        Sounds = GetComponent<PlayerSounds>();
        _runWalkBehavior = GetComponent<PlayerRunWalkBehavior>();
        _climbBehavior = GetComponent<PlayerClimbBehavior>();
        _glideBehavior = GetComponent<PlayerGlideBehavior>();
        _inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        CheckStateTransitions();
        transform.SetLossyScale(Vector3.one);
    }

    private void CheckStateTransitions() {
        if (ShouldStartGliding()) ChangeState(PlayerState.GLIDE);
        if (ShouldStartClimbing()) ChangeState(PlayerState.CLIMB);
    }

    public void ChangeState(PlayerState newState) {
        if (_currentState == newState) return;
        _currentState = newState;

        _runWalkBehavior.enabled = newState == PlayerState.WALK;
        _climbBehavior.enabled = newState == PlayerState.CLIMB;
        _glideBehavior.enabled = newState == PlayerState.GLIDE;
    }

    private bool ShouldStartGliding() {
        return (_currentState == PlayerState.WALK && !_runWalkBehavior.IsCoyoteGrounded && RB.velocity.y < 0 && InputController.GetDown(Control.JUMP));
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

    private bool ShouldStartClimbing() {
        return _climbBehavior.ReadyToClimb && InputController.Get(Control.MOVE_FORWARD);
        //should take into account facing direction and position relative to top of ladder. probaby a raycast
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

    public void Rotate(float rotateSpeed) {
        var mouseX = Input.GetAxis("Mouse X");

        var rotDelta = rotateSpeed * 100 * mouseX * Time.deltaTime * Settings.MouseSensetivity;

        transform.Rotate(rotDelta * Vector3.up);
    }
}
