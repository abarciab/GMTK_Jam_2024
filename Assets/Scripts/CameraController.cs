using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Position and rotation")]
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _verticalRotSpeed;
    //[SerializeField] private float _maxCameraTpDist = 2;

    [Header("FOV")]
    [SerializeField] private Vector2 _walkRunFovs;
    [SerializeField] private Vector2 _glideMinMaxFovs;
    [SerializeField] private float _fovChangeLerpFator = 20;

    [Header("Misc")]
    [SerializeField] private Camera _camera;
    private bool _cinematic;

    [Header("Cinematics")]
    [SerializeField] private Transform _spinner;
    [SerializeField] private AnimationCurve _spinAnimateCurve;

    void LateUpdate()
    {
        if (_cinematic) return;
        SetPosAndRotation();
        if (!GameManager.i.Player.IsGliding) SetFov();
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
            transform.rotation = Quaternion.Lerp(_startRot, transform.rotation, 15 * Time.deltaTime);

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

        //_camera.enabled = false;
        //yield return new WaitForSeconds(1.5f);
        //_camera.enabled = true;

        //timePassed = 0;
        //float returnTime = 1.5f;
        /*Vector3 startPos = transform.position;
        var startRot = transform.rotation;
        while (timePassed < returnTime) {
            float progress = timePassed / returnTime;
            transform.position = Vector3.Lerp(startPos, _player.TransformPoint(_offset), progress);

            var playerRot = _player.rotation;
            transform.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(transform.eulerAngles.x, playerRot.y, 0), progress);

            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
        }*/
        _cinematic = false;
    }

    private void SetPosAndRotation()
    {
        var targetPos = _player.TransformPoint(_offset);
        var delta = targetPos - transform.position;
        //if (delta.magnitude > _maxCameraTpDist) transform.position = Vector3.Lerp(transform.position, targetPos, 10 * Time.deltaTime);
        transform.position = targetPos;

        var playerRot = _player.eulerAngles;
        var targetRot = new Vector3(transform.eulerAngles.x, playerRot.y, 0);
        transform.eulerAngles = targetRot;

        var mouseY = Input.GetAxis("Mouse Y");
        var rotDelta = mouseY * Time.deltaTime * Settings.MouseSensetivity * 100 * _verticalRotSpeed * -1;
        rotDelta = Mathf.Clamp(rotDelta, -5, 5);
        transform.localEulerAngles += Vector3.right * rotDelta;
    }

    public void SetGlideFovPercent(float percent)
    {
        var current = _camera.fieldOfView;
        var target = Mathf.Lerp(_glideMinMaxFovs.x, _glideMinMaxFovs.y, percent);
        _camera.fieldOfView = Mathf.Lerp(current, target, _fovChangeLerpFator * Time.deltaTime);
    }

    private void SetFov()
    {
        var current = _camera.fieldOfView;
        var target = GameManager.i.Player.IsRunning ? _walkRunFovs.y : _walkRunFovs.x;
        _camera.fieldOfView = Mathf.Lerp(current, target, _fovChangeLerpFator * Time.deltaTime);
    }
}
