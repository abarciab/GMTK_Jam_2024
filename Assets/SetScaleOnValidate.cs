using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SetScaleOnValidate : MonoBehaviour
{
    [SerializeField] private Vector3 _scale = Vector3.one;

    private void Update() {
        if (transform.lossyScale != _scale) transform.SetLossyScale(_scale);
    }
}
