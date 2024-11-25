using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LockScale : MonoBehaviour
{
    [SerializeField] private bool _x;
    [SerializeField, OverrideLabel("x"), ConditionalField(nameof(_x))] private float _xVal;
    [SerializeField] private bool _y;
    [SerializeField, OverrideLabel("y"), ConditionalField(nameof(_y))] private float _yVal;
    [SerializeField] private bool _z;
    [SerializeField, OverrideLabel("z"), ConditionalField(nameof(_z))] private float _zVal;
    [Space(20)]
    [SerializeField] private Transform _referenceParent;

    private void Update()
    {
        var scale = transform.localScale;
        if (_x) scale.x = _referenceParent ? (_xVal / _referenceParent.localScale.x) : _xVal;
        if (_y) scale.y = _referenceParent ? (_yVal / _referenceParent.localScale.y) : _yVal;
        if (_z) scale.z = _referenceParent ? (_zVal / _referenceParent.localScale.z) : _zVal;
        transform.localScale = scale;
    }
}
