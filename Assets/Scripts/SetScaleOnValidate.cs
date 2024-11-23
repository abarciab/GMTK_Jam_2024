using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SetScaleOnValidate : MonoBehaviour
{
    private void OnValidate()
    {
        Update();
    }

    private void Update() {
        var scale = transform.lossyScale;
        scale.x = scale.z = scale.y;
        if (transform.lossyScale != scale) transform.SetLossyScale(scale);
    }

}
