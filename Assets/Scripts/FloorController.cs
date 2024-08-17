using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MyBox;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private float _height;
    [SerializeField] private Vector2 _exitSide;

    [SerializeField] private List<Transform> _floorSections = new List<Transform>();
    [SerializeField] private List<Transform> _extras = new List<Transform>();

    [Header("expansion")]
    [SerializeField, Range(0, 1)] private float _expansionProgress;
    [SerializeField, ReadOnly] private List<float> _compressedOffsets = new List<float>(); 
    [SerializeField, ReadOnly] private List<float> _expandedOffsets = new List<float>();
    [SerializeField, ReadOnly] private List<float> _currentOffsets = new List<float>();

    public Vector2 ExitSide => _exitSide;
    public Vector3 TopPos => transform.position + Vector3.up * _height;

    private void OnValidate()
    {
        if (_floorSections.Count == 5) UpdateModel();
    }

    private void UpdateModel()
    {
        _currentOffsets.Clear();

        float quarterProgress = Mathf.InverseLerp(0f, 0.25f, _expansionProgress);
        float secondCurrentOffset = Mathf.Lerp(_compressedOffsets[1], _expandedOffsets[1], quarterProgress);
        _floorSections[1].localPosition = Vector3.up * secondCurrentOffset;

        float extras1Progress = Mathf.InverseLerp(0.15f, 0.4f, _expansionProgress);
        var extra1Scale = new Vector3(extras1Progress, 1, extras1Progress);
        _extras[0].localScale = extra1Scale;

        float halfProgress = Mathf.InverseLerp(0f, 0.5f, _expansionProgress);
        float thirdCurrentOffset = Mathf.Lerp(_compressedOffsets[2], _expandedOffsets[2], halfProgress);
        _floorSections[2].localPosition = Vector3.up * thirdCurrentOffset;

        float thirdQuarterProgress = Mathf.InverseLerp(0.5f, 0.75f, _expansionProgress);
        float fourthCurrentOffset = Mathf.Lerp(_compressedOffsets[3], _expandedOffsets[3], thirdQuarterProgress);
        _floorSections[3].localPosition = Vector3.up * fourthCurrentOffset;

        float extras2Progress = Mathf.InverseLerp(0.65f, 0.9f, _expansionProgress);
        var extra2Scale = new Vector3(extras2Progress, 1, extras2Progress);
        _extras[1].localScale = extra2Scale;

        float secondHalfProgress = Mathf.InverseLerp(0.5f, 1f, _expansionProgress);
        float fifthCurrentOffset = Mathf.Lerp(_compressedOffsets[4], _expandedOffsets[4], secondHalfProgress);

        _floorSections[4].localPosition = Vector3.up * fifthCurrentOffset;
    }

    [ButtonMethod]
    private void SetCompressedPositions()
    {
        _compressedOffsets.Clear();
        foreach (var s in _floorSections) _compressedOffsets.Add(s.localPosition.y);
    }

    [ButtonMethod]
    private void SetExpandedPositions()
    {
        _expandedOffsets.Clear();

        for (int i = 0; i < _floorSections.Count; i++) {
            _expandedOffsets.Add(_floorSections[i].localPosition.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * _height);
    }
}
