using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Camera _camera;

    [Header("FOV")]
    [SerializeField] private Vector2 _walkRunFovs;
    [SerializeField] private float _fovChangeLerpFator = 20;

    void Update()
    {
        SetPosAndRotation();
        SetFov();
    }

    private void SetPosAndRotation()
    {
        transform.position = _player.TransformPoint(_offset);
        var playerRot = _player.eulerAngles;
        var targetRot = new Vector3(0, playerRot.y, 0);
        transform.eulerAngles = targetRot;
    }

    private void SetFov()
    {
        var current = _camera.fieldOfView;
        var target = GameManager.i.Player.IsRunning ? _walkRunFovs.y : _walkRunFovs.x;
        _camera.fieldOfView = Mathf.Lerp(current, target, _fovChangeLerpFator * Time.deltaTime);
    }
}
