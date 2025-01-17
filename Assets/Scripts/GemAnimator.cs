using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GemAnimator : MonoBehaviour
{
    [SerializeField] private Transform _model;

    [Header("Rotation")]
    [SerializeField] private Vector2 _idleRotSpeeds;
    [SerializeField] private float _flyingRotSpeed;
    [SerializeField] private float _rotSnappiness = 4;

    [Header("Motion")]
    [SerializeField] private AnimationCurve _vertCurve;
    [SerializeField] private AnimationCurve _sizeCurve;
    [SerializeField] private float _flyAwayTime = 3;
    [SerializeField] private float _maxVertDist = 20;
    

    public List<ParticleSystem> _idleParticles = new List<ParticleSystem>();
    public ParticleSystem _pickupParticles;
    
    private bool _idleParticlesActive;
    private bool _flying;
    private float _currRotSpeed;

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        var rotSpeed = _idleRotSpeeds.x;
        if (_idleParticlesActive) rotSpeed = _idleRotSpeeds.y;
        if (_flying) rotSpeed = _flyingRotSpeed;
        _currRotSpeed = Mathf.Lerp(_currRotSpeed, rotSpeed, _rotSnappiness * Time.deltaTime);
        _model.localEulerAngles += Vector3.up * rotSpeed;
    }

    public void SetIdleParticles(bool active)
    {
        if (active == _idleParticlesActive) return;
        _idleParticlesActive = active;

        foreach (var part in _idleParticles) {
            if (active) part.Play();
            else part.Stop();
        }
    }

    public void PickUpGem()
    {
        _pickupParticles.Emit(150);
        SetIdleParticles(false);
        FlyAway();
    }

    private void OnDestroy()
    {
        _flying = false;
    }

    private async void FlyAway()
    {
        _flying = true;
        var startY = _model.localPosition.y;
        var targetHeight = startY + _maxVertDist;
        var startScale = _model.localScale;

        float timeLeft = _flyAwayTime;
        while (timeLeft > 0) {
            if (!_flying) return;
            float progress = 1 - (timeLeft / _flyAwayTime);

            var y = Mathf.Lerp(startY, targetHeight, _vertCurve.Evaluate(progress));
            var pos = _model.transform.localPosition;
            pos.y = y;

            _model.localPosition = pos;
            _model.localScale = Vector3.Lerp(startScale, Vector3.zero, _sizeCurve.Evaluate(progress));

            timeLeft -= Time.deltaTime;
            await Task.Yield();
        }
    }
}
