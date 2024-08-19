using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [SerializeField] private Transform _start;
    [SerializeField] private Transform _end;

    [Header("parts")]
    [SerializeField] private Transform _deck;
    [SerializeField] private Transform _sideWallParent;
    [SerializeField] private GameObject _sideWallPrefab;
    [SerializeField] private float _sideWallWidth;

    private Vector3 _middle => Vector3.Lerp(_start.position, _end.position, 0.5f);

    [ButtonMethod]
    void Update()
    {
        if (!_start || !_end) return;

        transform.position = _middle;
        transform.LookAt(_end.position);

        var dist = Vector3.Distance(_start.position, _end.position);

        var scale = _deck.localScale;
        scale.z = dist;
        _deck.localScale = scale;
    }

    [ButtonMethod]
    private void PlaceSideWalls()
    {
        var totalDist = Vector3.Distance(_start.position, _end.position);

        int sideWallCount = Mathf.FloorToInt(totalDist / _sideWallWidth);
        for (int i = 0; i < sideWallCount; i++) {
            var newSideWall = Instantiate(_sideWallPrefab, _sideWallParent);
            newSideWall.transform.localRotation = Quaternion.identity;
            var pos = new Vector3(_deck.transform.localScale.x / 2, 0, i * _sideWallWidth - (totalDist-_sideWallWidth)/2);
            newSideWall.transform.localPosition = pos;

            newSideWall = Instantiate(_sideWallPrefab, _sideWallParent);
            newSideWall.transform.localRotation = Quaternion.Euler(0, 180, 0);
            pos = new Vector3(-_deck.transform.localScale.x/2, 0, i * _sideWallWidth - (totalDist - _sideWallWidth) / 2);
            newSideWall.transform.localPosition = pos;
        }
    }

    public void Initialize(Transform start, Transform end)
    {
        _start = start;
        _end = end;
        PlaceSideWalls();
        Update();
    }
}
