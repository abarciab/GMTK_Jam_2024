using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;
using MyBox;

[ExecuteInEditMode, RequireComponent(typeof(SplineContainer))]
public class SplineRotationFixer : MonoBehaviour
{
    private SplineContainer _spline;

    private void OnValidate()
    {
        //FixRotation();
    }

    [ButtonMethod]
    private void FixRotation()
    {
        if (_spline == null) _spline = GetComponent<SplineContainer>();
        var knots = _spline.Spline;

        for (int i = 0; i < knots.Count; i++) {
            var knot = knots[i]; 
            Quaternion rot = knot.Rotation;
            var euler = rot.eulerAngles;

            euler.x = 0;
            euler.z = 0;

            var newRot = Quaternion.Euler(euler.x, euler.y, euler.z);
            knot.Rotation = newRot;

            knots.SetKnot(i, knot);
        }

        Utils.SetDirty(_spline);

        transform.eulerAngles = Vector3.zero;
        Utils.SetDirty(transform);
    }
}
