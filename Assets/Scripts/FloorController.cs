using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private float _height;
    [SerializeField] private Vector2 _exitSide;
    public Vector2 ExitSide => _exitSide;
    public Vector3 TopPos => transform.position + Vector3.up * _height;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * _height);
    }
}
