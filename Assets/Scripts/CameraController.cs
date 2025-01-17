using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Position and rotation")]
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _verticalRotSpeed;
    [SerializeField] private float _yOffsetLerpFactor = 10;

    [Header("FOV")]
    [SerializeField] private Vector2 _walkRunFovs;
    [SerializeField] private Vector2 _glideMinMaxFovs;
    [SerializeField] private float _fovChangeLerpFator = 20;

    [Header("Misc")]
    [SerializeField] private Camera _camera;

    [Header("Cinematics")]
    [SerializeField] private Transform _spinner;
    [SerializeField] private AnimationCurve _spinAnimateCurve;

    private PlayerController _playerController;
    private float _yOffset;
    [SerializeField, ReadOnly] private bool _cinematic;

    public void SetOffset(float newOffset) => _yOffset = newOffset;

    void LateUpdate() {
        if (!_playerController) _playerController = GameManager.i.Player;
        if (_cinematic) return;
        if (_yOffset > 0.05f) _yOffset = Mathf.Lerp(_yOffset, 0, _yOffsetLerpFactor * Time.deltaTime);
        SetPosAndRotation();
        if (_playerController.IsGliding) SetGlideFov();
        else SetGroundFov();
    }

    private void SetGlideFov() {
        var percent = _playerController.GlideSpeedPercent; 
        var current = _camera.fieldOfView;
        var target = Mathf.Lerp(_glideMinMaxFovs.x, _glideMinMaxFovs.y, percent);
        _camera.fieldOfView = Mathf.Lerp(current, target, _fovChangeLerpFator * Time.deltaTime);
    }

    public void StartCinematicRotation(Vector3 source, float distFromObject, float totalDeltaY, float time)
    {
        StopAllCoroutines();
        _cinematic = true;
        StartCoroutine(AnimateSpin(source, distFromObject, totalDeltaY, time));
    }

    private IEnumerator AnimateSpin(Vector3 source, float distFromObject, float totalY, float time)
    {
        float timePassed = 0;
        _spinner.SetParent(null);
        transform.SetParent(_spinner);

        while (timePassed < time) {
            float progress = timePassed / time;
            progress = _spinAnimateCurve.Evaluate(progress);

            _spinner.position = source;

            var pos = Vector3.zero;
            pos.y += Mathf.Lerp(0, totalY, progress);
            pos.z = distFromObject;
            transform.localPosition = Vector3.Lerp(transform.localPosition, pos, 15 * Time.deltaTime);

            var _startRot = transform.rotation;
            transform.LookAt(source);
            //transform.rotation = Quaternion.Lerp(_startRot, transform.rotation, 15 * Time.deltaTime);

            var rot = _spinner.eulerAngles;
            rot.y = progress * 360;
            rot.x = rot.z = 0;
            _spinner.eulerAngles = rot;

            //transform.RotateAround(source.position, Vector3.up, progress * 360);

            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
        }
        transform.SetParent(null);
        _spinner.SetParent(transform);

        UIManager.i.FadeToBlack(0.1f);
        yield return new WaitForSeconds(0.2f);
        UIManager.i.FadeFromBlack(0.1f);

        _cinematic = false;
    }

    private void SetPosAndRotation()
    {
        SetPosition();
        SetRotation();

    }

    private void SetPosition()
    {
        var targetPos = _player.TransformPoint(_offset) + Vector3.down * _yOffset;
        var delta = targetPos - transform.position;
        transform.position = targetPos;
    }

    private void SetRotation()
    {
        var playerRot = _player.eulerAngles;
        var targetRot = new Vector3(transform.eulerAngles.x, playerRot.y, 0);
        transform.eulerAngles = targetRot;
        if (GameManager.i.Player.IsStunned) return;

        var mouseY = Input.GetAxis("Mouse Y");
        var rotDelta = mouseY * Time.deltaTime * Settings.MouseSensetivity * 100 * _verticalRotSpeed * -1;
        transform.localEulerAngles += Vector3.right * rotDelta;
    }

    private void SetGroundFov()
    {
        var current = _camera.fieldOfView;
        var target = GameManager.i.Player.IsRunning ? _walkRunFovs.y : _walkRunFovs.x;
        _camera.fieldOfView = Mathf.Lerp(current, target, _fovChangeLerpFator * Time.deltaTime);
    }
}
