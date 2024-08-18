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
    [SerializeField] private float _glideAngleIncreaseFactor = 1;
    [SerializeField] private float _glideSpeedMax = 10;
    [SerializeField] private float _glideGravity = 1;
    [SerializeField] private float _glideLerpFactor = 3;
    [SerializeField] private float _glideEndBoost = 3;
    [SerializeField] private float _forwardGlideCheckerRadius;
    [SerializeField] private Vector3 _forwardGlideCheckerOffset;
    [SerializeField] private float _downGlideCheckerRadius;
    [SerializeField] private Vector3 _downGlideCheckerOffset;


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
    private InventoryItem _currentDroppedItem;
    private float _glideSpeed;
    private Vector3 oldPos;
    private GameObject _currentFloorObj;

    private float DistanceTo(Vector3 pos) => Vector3.Distance(transform.position, pos);
    private float _currentLadderDist => _currentLadder == null ? Mathf.Infinity : DistanceTo(_currentLadder.transform.position);
    private float _currentItemDist => _currentDroppedItem == null ? Mathf.Infinity : DistanceTo(_currentDroppedItem.transform.position);

    private InventoryItem[] _inventoryItems = {null, null, null, null, null, null, null, null, null, null};
    private int _currentItemIndex = 0;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        GameManager.i.Player = this;
        _lastJumpTime = Time.time;
        _isGrounded = true;

        EquipItem(0);

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

    public void SetClosestItem(InventoryItem item)
    {
        if (_currentDroppedItem == null || _currentItemDist < DistanceTo(item.transform.position)) _currentDroppedItem = item;
    }

    public void ClearItem(InventoryItem item)
    {
        if (_currentDroppedItem == item) _currentDroppedItem = null;
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
        Inventory();
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

    private void Inventory()
    {
        if(InputController.GetDown(Control.INTERACT) && _currentDroppedItem)
        {
            _currentDroppedItem.Pickup(transform);
            _inventoryItems[_currentItemIndex]?.Drop();
            _inventoryItems[_currentItemIndex] = _currentDroppedItem;
            UIManager.i.SetInventoryImage(_currentDroppedItem.GetSprite(), _currentItemIndex);
            EquipItem(_currentItemIndex);
            _currentDroppedItem = null;
        }

        else if(InputController.GetDown(Control.DROP) && _inventoryItems[_currentItemIndex])
        {
            _inventoryItems[_currentItemIndex].Drop();
            _inventoryItems[_currentItemIndex] = null;
            UIManager.i.RemoveInventoryImage(_currentItemIndex);
        }

        else if(InputController.GetDown(Control.USE_PRIMARY) && _inventoryItems[_currentItemIndex])
        {
            _inventoryItems[_currentItemIndex].LeftClick();
        }

        else if(InputController.GetDown(Control.USE_SECONDARY) && _inventoryItems[_currentItemIndex])
        {
            _inventoryItems[_currentItemIndex].RightClick();
        }

        else if(InputController.GetDown(Control.NEXT_ITEM) || Input.mouseScrollDelta.y > 0f)
        {
            EquipNextItem();
        }

        else if(InputController.GetDown(Control.LAST_ITEM) || Input.mouseScrollDelta.y < 0f)
        {
            EquipPreviousItem();
        }
    }

    private void StartGliding()
    {
        _openGliderSound.Play();
        _isGliding = true;
        _rb.isKinematic = true;
        _glideSpeed = _tempConstFlySpeed;
    }

    private void Glide()
    {
        CheckIfShouldLand();

        var currentSpeed = transform.position - oldPos;
        oldPos = transform.position;

        var dir = Camera.main.transform.forward.normalized;
        
        float downAngle = Vector3.Dot(dir, Vector3.down);
        float targetGlideSpeed = _glideSpeed + _glideAngleIncreaseFactor * downAngle;
        _glideSpeed = Mathf.Lerp(_glideSpeed, targetGlideSpeed, _glideLerpFactor * Time.deltaTime);
        _glideSpeed = Mathf.Clamp(_glideSpeed, 0, _glideSpeedMax);

        var posDelta = dir * _glideSpeed * Time.deltaTime;
        posDelta += (currentSpeed.y + _glideGravity) * Time.deltaTime * Vector3.down;

        transform.position += posDelta;
    }

    private void CheckIfShouldLand()
    {
        var camTrans = Camera.main.transform;
        var all = new List<Collider>();
        var forward = Physics.OverlapSphere(camTrans.TransformPoint(_forwardGlideCheckerOffset), _forwardGlideCheckerRadius);
        var down = Physics.OverlapSphere(camTrans.TransformPoint(_forwardGlideCheckerOffset), _forwardGlideCheckerRadius);
        all.AddRange(forward);
        all.AddRange(down);
        all = all.Where(x => x.GetComponent<PlayerController>() == null).ToList();

        if (all.Count > 0) {
            StopGliding();
            //print("overlapping w: " + all[0].gameObject.name);
        }
    }

    private void StopGliding()
    {
        _isGliding = false;
        _rb.isKinematic = false;
        transform.position += Vector3.up * _glideEndBoost;
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
            var collider = colliders[0];

            GameManager.i.UpdateCurrentTower(collider.GetComponentInParent<TowerController>());
            
            var movingPlatform = collider.GetComponentInParent<MovingPlatform>();
            if (movingPlatform) transform.SetParent(movingPlatform.transform);
            else transform.SetParent(null);

            var decayingPlatform = collider.GetComponentInParent<DecayingPlatform>();
            decayingPlatform?.StartStandingOnPlatform();
            
            if (!oldState) Land();

            if (_currentFloorObj) {
                var oldDecay = _currentFloorObj.GetComponent<DecayingPlatform>();
                if (oldDecay && oldDecay != decayingPlatform) oldDecay.LeavePlatform();
            } 
            if (movingPlatform) _currentFloorObj = movingPlatform.gameObject;
            if (decayingPlatform) _currentFloorObj = decayingPlatform.gameObject;
            else _currentFloorObj = collider.gameObject;
        }
        else {
            transform.SetParent(null);
            if (_currentFloorObj) {
                //_currentFloorObj.GetComponent<DecayingPlatform>()?.LeavePlatform();
            }
            _currentFloorObj = null;
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.TransformPoint(_forwardGlideCheckerOffset), _forwardGlideCheckerRadius);
        Gizmos.DrawWireSphere(transform.TransformPoint(_downGlideCheckerOffset), _downGlideCheckerRadius);
    }

    private void EquipItem(int index)
    {
        if (_currentItemIndex != index) _inventoryItems[_currentItemIndex]?.Unequip();
        _currentItemIndex = index;
        _inventoryItems[_currentItemIndex]?.Equip();
        UIManager.i.SelectInventoryImage(index);
    }

    private void EquipNextItem()
    {
        var nextIndex = _currentItemIndex + 1;
        if (nextIndex >= _inventoryItems.Length) nextIndex = 0;
        EquipItem(nextIndex);
    }

    private void EquipPreviousItem()
    {
        var previousIndex = _currentItemIndex - 1;
        if (previousIndex < 0) previousIndex = _inventoryItems.Length - 1;
        EquipItem(previousIndex);
    }
}
