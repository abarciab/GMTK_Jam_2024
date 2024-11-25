using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LockYScale : MonoBehaviour
{
    [SerializeField] private float _y;
    [SerializeField] private bool _ignoreParent;

    private void OnValidate()
    {
        Update();
    }

    private void Update()
    {
        if (_ignoreParent) LockAgainstGrandparent();
        else Lock(_y);
    }

    private void Lock(float y)
    {
        if (transform.localScale.y == y) return;
        var scale = transform.localScale;
        scale.y = y;
        transform.localScale = scale;
    }

    private void LockAgainstGrandparent()
    {
        if (transform.parent == null) Lock(_y);
        else Lock(_y / transform.parent.localScale.y);
    }

}
