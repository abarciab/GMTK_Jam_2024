using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[SelectionBase]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField, ReadOnly] private DoorState _state1;
    [SerializeField, ReadOnly] private DoorState _state2;

    [Header("Animation")]
    [SerializeField] private float _animateTime = 1.2f;
    [SerializeField] private AnimationCurve _curve;

    private bool _inState1;

    private void Start()
    {
        SnapToState(_state1, true);
        GoToState2();
    }

    [ButtonMethod]
    private void SetState1()
    {
        _state1.Pos = transform.localPosition;
        _state1.Rot = transform.localRotation;
    }

    [ButtonMethod]
    private void SetState2()
    {
        _state2.Pos = transform.localPosition;
        _state2.Rot = transform.localRotation;
    }


    [ButtonMethod]
    public void GoToState1()
    {
        StopAllCoroutines();
        if (!Application.isPlaying) SnapToState(_state1, true);
        else StartCoroutine(AnimateToState(_state1, true));
    }

    [ButtonMethod]
    public void GoToState2()
    {
        StopAllCoroutines();
        if (!Application.isPlaying) SnapToState(_state2, false);
        else StartCoroutine(AnimateToState(_state2, false));
    }

    private void SnapToState(DoorState state, bool targetIsState1)
    {
        transform.SetPositionAndRotation(state.Pos, state.Rot);
        _inState1 = targetIsState1;
    }

    private IEnumerator AnimateToState(DoorState target, bool targetIsState1)
    {
        DoorState startState = new DoorState(transform.localPosition, transform.localRotation);

        float timePassed = 0;
        while (timePassed < _animateTime) {
            float progress = timePassed / _animateTime;
            progress = _curve.Evaluate(progress);

            var pos = Vector3.Lerp(startState.Pos, target.Pos, progress);
            var rot = Quaternion.Lerp(startState.Rot, target.Rot, progress);
            transform.SetLocalPositionAndRotation(pos, rot);

            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        SnapToState(target, targetIsState1);

        if (Application.isPlaying) {
            if (_inState1) GoToState2();
            else GoToState1();
        }
    }
}
