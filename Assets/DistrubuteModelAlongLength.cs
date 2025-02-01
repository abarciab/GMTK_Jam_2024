using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistrubuteModelAlongLength : MonoBehaviour
{
    [SerializeField] private float _length;
    [SerializeField] private float _sectionLength;

    [SerializeField] private GameObject _ropePrefab;
    [SerializeField] private Transform _ropeParent;
    
    private List<GameObject> _spawnedSections = new List<GameObject>();

    public void AddRope(float length)
    {
        _length = length;
        AddRope();
    }

    [ButtonMethod]
    private void AddRope()
    {
        foreach (var s in _spawnedSections) {
            if (Application.isPlaying) Destroy(s.gameObject);
            else DestroyImmediate(s.gameObject);
        }
        _spawnedSections.Clear();

        for (int i = 0; i < Mathf.CeilToInt(_length / _sectionLength) + 1; i++) {
            var newSection = Instantiate(_ropePrefab, _ropeParent);
            newSection.transform.position += Vector3.down * _spawnedSections.Count * _sectionLength;
            _spawnedSections.Add(newSection);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_ropeParent) Gizmos.DrawLine(_ropeParent.position, _ropeParent.position + Vector3.down * _length);
    }
}
