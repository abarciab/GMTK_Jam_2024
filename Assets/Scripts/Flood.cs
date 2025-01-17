using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flood : MonoBehaviour
{
    [SerializeField] private float _totalSeconds = 600;
    [SerializeField] private float _maxHeight = 450;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _offset = 90;

    [SerializeField] private Material slimePipeMat;
    private Material refMat;

    private float _timePassed;

    private void OnEnable()
    {
        refMat = new Material(slimePipeMat);

        slimePipeMat.SetFloat("_Ping", Time.time);
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;
        float progress = _timePassed / _totalSeconds;
        progress = _curve.Evaluate(progress);
        float maxHeight = _maxHeight;
        float targetHeight = progress * maxHeight;

        var pos = transform.position;
        if (pos.y < targetHeight) pos.y = targetHeight - _offset;
        transform.position = pos;
        if (GameManager.i.Player.transform.position.y < transform.position.y) GameManager.i.LoseGame();
    }


#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        slimePipeMat.CopyPropertiesFromMaterial(refMat);
    }
#endif
}
