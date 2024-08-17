using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private float _height;
    [SerializeField] private CardinalDirection _exitSide;
    public CardinalDirection ExitSide => _exitSide;
    public enum CardinalDirection { South, East, North, West };
    public Vector3 TopPos => transform.position + Vector3.up * _height;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * _height);
    }
}
