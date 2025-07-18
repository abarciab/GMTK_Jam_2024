using MyBox;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum CameraAnimationType { TELEPORT}

[System.Serializable]
public class CameraAnimationData
{
    [HideInInspector] public string Name;

    public CameraAnimationType Type;
    public float Duration;
    public AnimationCurve Curve;
    public float FovTarget;
}

[RequireComponent (typeof(CameraController))]
public class CameraAnimator : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private PlayerController _player;

    [Header("Animation Data")]
    [SerializeField] private List<CameraAnimationData> _data = new List<CameraAnimationData>();

    private CameraController _controller;

    private void OnValidate()
    {
        var list = Utils.EnumToList<CameraAnimationType>();
        for (int i = 0; i < list.Count; i++) {
            if (i >= _data.Count) _data.Add(new CameraAnimationData());
            _data[i].Type = list[i];
            _data[i].Name = list[i].ToString();
        }
        while (_data.Count > list.Count) _data.RemoveAt(_data.Count - 1);
    }

    private void Start()
    {
        _controller = GetComponent<CameraController> ();
    }

    [ButtonMethod]
    public async void TeleportAnimate()
    {
        _controller.enabled = false;
        //_player.Freeze(true);

        var data = GetData(CameraAnimationType.TELEPORT);

        var fovStart = _cam.fieldOfView;

        float timePassed = 0;
        while (timePassed < (data.Duration)) {

            var progress = timePassed / data.Duration;
            progress = data.Curve.Evaluate(progress);
            var fov = Mathf.Lerp(fovStart, data.FovTarget, progress);
            _cam.fieldOfView = fov;

            await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000));
            timePassed += Time.deltaTime;
        }

        _controller.enabled = true;
        //_player.Freeze(false);

    }

    private CameraAnimationData GetData(CameraAnimationType type) => _data.Where(x => x.Type == type).First(); 
}
