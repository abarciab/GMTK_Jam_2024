using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerGlideBehavior : MonoBehaviour
{
    [SerializeField] private int _groundLayer = 8;
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
    public float MinGlideTimeReq = 0.5f;
    public float MinGlideDistReq = 5f;

    [HideInInspector] public Vector3 InturruptPoint;
    public float GlideSpeedPercent => _glideSpeed / _glideSpeedMax;

    private PlayerController _controller;
    private float _glideSpeed;
    private Vector3 _oldPos;
    private PlayerSounds Sounds => _controller.Sounds;
    private Rigidbody _rb => _controller.RB;

    private void OnEnable() {
        if (!_controller) _controller = GetComponent<PlayerController>();
        Sounds.Get(PlayerSoundKey.WIND_LOOP).PlaySilent();
        Sounds.Get(PlayerSoundKey.OPEN_GLIDER).Play();
        _glideSpeed = _tempConstFlySpeed;
        _oldPos = transform.position;
        _rb.isKinematic = true;
    }

    private void Update() {
        CheckIfShouldLand();
        if (!enabled) return;


        _controller.Rotate(); 
        Glide();
    }

    private void OnDisable()
    {
        Sounds.Get(PlayerSoundKey.WIND_LOOP).SetPercentVolume(0);
    }

    private void Glide() {
        var currentSpeed = transform.position - _oldPos;
        _oldPos = transform.position;

        var dir = Camera.main.transform.forward.normalized;

        float downAngle = Vector3.Dot(dir, Vector3.down);
        float targetGlideSpeed = _glideSpeed + _glideAngleIncreaseFactor * downAngle;
        _glideSpeed = Mathf.Lerp(_glideSpeed, targetGlideSpeed, _glideLerpFactor * Time.deltaTime);
        _glideSpeed = Mathf.Clamp(_glideSpeed, 0, _glideSpeedMax);

        Sounds.Get(PlayerSoundKey.WIND_LOOP).SetPercentVolume(_glideSpeed / _glideSpeedMax, 0.5f);

        var posDelta = dir * _glideSpeed * Time.deltaTime;
        posDelta += (currentSpeed.y + _glideGravity) * Time.deltaTime * Vector3.down;

        transform.position += posDelta;
    }

    private void CheckIfShouldLand() {
        var camTrans = Camera.main.transform;
        var all = new List<Collider>();

        var forwardPoint = camTrans.TransformPoint(_forwardGlideCheckerOffset);
        var forward = Physics.OverlapSphere(forwardPoint, _forwardGlideCheckerRadius);
        var forwardList = forward.Where(x => x.GetComponent<PlayerController>() == null && !x.isTrigger).ToList();
        var belowList = _controller.GetCollidersBelow().Where(x => x.gameObject.layer == _groundLayer);

        all.AddRange(forwardList);
        all.AddRange(belowList);
        if (all.Count == 0) return;

        if (forwardList.Count > 0) {
            var y = forwardPoint.y + _glideEndBoost;
            GameManager.i.Camera.GetComponent<CameraController>().SetOffset(y - transform.position.y);
            var pos = transform.position;
            pos.y = y;
            transform.position = pos;
        }
        else {
            transform.position += Vector3.up * 0.5f;
        }

        StopGliding();
    }


    private void StopGliding() {
        _rb.isKinematic = false;
        Sounds.Get(PlayerSoundKey.WIND_LOOP).SetPercentVolume(0);
        Sounds.Get(PlayerSoundKey.GLIDER_LAND).Play();
        InturruptPoint = transform.position;
        _controller.ChangeState(PlayerState.WALK);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.TransformPoint(_forwardGlideCheckerOffset), _forwardGlideCheckerRadius);
        Gizmos.DrawWireSphere(transform.TransformPoint(_downGlideCheckerOffset), _downGlideCheckerRadius);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * MinGlideDistReq);
    }
}
