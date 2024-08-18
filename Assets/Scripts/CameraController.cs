using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Position and rotation")]
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _verticalRotSpeed;

    [Header("FOV")]
    [SerializeField] private Vector2 _walkRunFovs;
    [SerializeField] private float _fovChangeLerpFator = 20;

    [Header("Misc")]
    [SerializeField] private Camera _camera;

    void Update()
    {
        SetPosAndRotation();
        SetFov();
    }

    private void SetPosAndRotation()
    {
        transform.position = _player.TransformPoint(_offset);
        var playerRot = _player.eulerAngles;
        var targetRot = new Vector3(transform.eulerAngles.x, playerRot.y, 0);
        transform.eulerAngles = targetRot;

        var mouseY = Input.GetAxis("Mouse Y");
        transform.localEulerAngles += Vector3.right * mouseY * Time.deltaTime * Settings.MouseSensetivity * 100 * _verticalRotSpeed * -1;
    }

    private void SetFov()
    {
        var current = _camera.fieldOfView;
        var target = GameManager.i.Player.IsRunning ? _walkRunFovs.y : _walkRunFovs.x;
        _camera.fieldOfView = Mathf.Lerp(current, target, _fovChangeLerpFator * Time.deltaTime);
    }
}
