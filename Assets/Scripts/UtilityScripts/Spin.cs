using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _axis;

    private void Update()
    {
        transform.localEulerAngles += _axis * _speed * Time.deltaTime;
    }
}
