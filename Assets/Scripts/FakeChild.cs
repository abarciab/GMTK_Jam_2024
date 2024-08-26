using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class FakeChild : MonoBehaviour
{
    [SerializeField] private Transform _fakeParent;
    [SerializeField] private bool _set; 
    [SerializeField, ReadOnly] private Vector3 localPos;

    private void OnValidate()
    {
        if (_set) {
            SetLocalPos();
            _set = false;
        }
    }

    void SetLocalPos()
    {
        var realParent = transform.parent;
        transform.SetParent(_fakeParent);
        localPos = transform.localPosition;
        transform.SetParent(realParent);
    }

    void Update()
    {
        transform.position = _fakeParent.TransformPoint(localPos);
    }
}
