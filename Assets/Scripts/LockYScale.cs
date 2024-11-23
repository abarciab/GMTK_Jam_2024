using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LockYScale : MonoBehaviour
{
    [SerializeField] private float _y;

    private void OnValidate()
    {
        Update();
    }

    private void Update()
    {
        if (transform.localScale.y == _y) return;
        var scale = transform.localScale;
        scale.y = _y;
        transform.localScale = scale;
    }

}
