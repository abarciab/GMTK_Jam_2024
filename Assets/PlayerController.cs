using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _runSpeed = 10;
    [SerializeField] private float _rotateSpeed;

    [ReadOnly, SerializeField] private Vector3 _moveDir;

    private Rigidbody _rb;
    private bool _isRunning;
    public bool IsRunning => _isRunning;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        GameManager.i.Player = this;
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
        _moveDir = GetMoveDir();
        _isRunning = InputController.Get(Control.RUN) && _moveDir != Vector3.zero;
        if (_moveDir == Vector3.zero) return;

        var speed = _isRunning ? _runSpeed : _walkSpeed;    
        _rb.velocity = _moveDir * speed;
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
}
