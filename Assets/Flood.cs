using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flood : MonoBehaviour
{
    [SerializeField] private float _totalSeconds = 600;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _offset;

    private float _timePassed;

    private void Update()
    {
        _timePassed += Time.deltaTime;
        float progress = _timePassed / _totalSeconds;
        progress = _curve.Evaluate(progress);
        float maxHeight = GameManager.i.GetShortestMaxHeight() - _offset;
        float targetHeight = progress * maxHeight;

        var pos = transform.position;
        if (pos.y < targetHeight) pos.y = targetHeight;
        transform.position = pos;
        if (GameManager.i.Player.transform.position.y < transform.position.y) GameManager.i.LoseGame();
    }
}
