using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flood : MonoBehaviour
{
    [SerializeField] private float _totalSeconds = 600;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _offset = 90;
    [SerializeField] private float _topOffset = 25;

    [SerializeField] private Material slimePipeMat;

    private float _timePassed;

    private void OnEnable()
    {
        slimePipeMat.SetFloat("_Ping", Time.time);
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;
        float progress = _timePassed / _totalSeconds;
        progress = _curve.Evaluate(progress);
        float maxHeight = GameManager.i.GetShortestMaxHeight() - _topOffset;
        float targetHeight = progress * maxHeight;

        var pos = transform.position;
        if (pos.y < targetHeight) pos.y = targetHeight - _offset;
        transform.position = pos;
        if (GameManager.i.Player.transform.position.y < transform.position.y) GameManager.i.LoseGame();
    }
}
