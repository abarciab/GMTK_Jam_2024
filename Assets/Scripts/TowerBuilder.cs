using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (TowerController))]
public class TowerBuilder : MonoBehaviour
{
    [SerializeField] private GameObject _firstFloorPrefab;
    [SerializeField] private GameObject _lastFloorPrefab;
    [SerializeField] private List<TowerFloorData> _towerFloorPrefabs = new List<TowerFloorData>();
    [SerializeField, Min(2)] private int _targetHeight = 10;

    private List<FloorController> _placedFloors = new List<FloorController>();
    private FloorController _currentTopFloor;
    private TowerController _controller;

    private void Start()
    {
        _controller = GetComponent<TowerController>();
        GenerateTower();  
    }

    private void GenerateTower()
    {
        PlaceFirstFloor();
        for (int i = 0; i < _targetHeight - 1; i++) PlaceNextFloor();
        _controller.MaxHeight = _currentTopFloor.TopPos.y;
    }

    private void PlaceFirstFloor()
    {
        _currentTopFloor = Instantiate(_firstFloorPrefab, transform.position, transform.rotation, transform).GetComponent<FloorController>();
        _currentTopFloor.gameObject.name = "floor 1";
        _placedFloors.Add(_currentTopFloor);
    }

    private void PlaceNextFloor()
    {
        Vector3 newFloorRotation = Vector3.zero;

        // switch (_currentTopFloor.ExitSide)
        // {
        //     case 
        // }

        GameObject selectedPrefab = _towerFloorPrefabs[Random.Range(0, _towerFloorPrefabs.Count)].floorPrefab;
        if (_placedFloors.Count == _targetHeight - 1) selectedPrefab = _lastFloorPrefab;

        GameObject newFloorObj = Instantiate(selectedPrefab, _currentTopFloor.TopPos, Quaternion.identity, transform);
        FloorController newFloor = newFloorObj.GetComponent<FloorController>();
        _placedFloors.Add(newFloor);
        newFloorObj.name = "floor " + _placedFloors.Count;
        _currentTopFloor = newFloor;
    }
}
