using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [SerializeField] private GameObject _firstFloorPrefab;
    [SerializeField] private List<GameObject> _floorPrefabs = new List<GameObject>();
    [SerializeField] private int _targetHeight = 10;

    private List<FloorController> _placedFloors = new List<FloorController>();
    private FloorController _currentTopFloor;

    private void Start()
    {
        GenerateTower();
    }

    private void GenerateTower()
    {
        PlaceFirstFloor();
        for (int i = 0; i < _targetHeight - 1; i++) PlaceNextFloor();
    }

    private void PlaceFirstFloor()
    {
        _currentTopFloor = Instantiate(_firstFloorPrefab, transform.position, transform.rotation, transform).GetComponent<FloorController>();
        _currentTopFloor.gameObject.name = "floor 1";
        _placedFloors.Add(_currentTopFloor);
    }

    private void PlaceNextFloor()
    {
        var selectedPrefab = _floorPrefabs[Random.Range(0, _floorPrefabs.Count)];
        var newFloorObj = Instantiate(selectedPrefab, _currentTopFloor.TopPos, transform.rotation, transform);
        var newFloor = newFloorObj.GetComponent<FloorController>();
        _placedFloors.Add(newFloor);
        newFloor.gameObject.name = "floor " + _placedFloors.Count;
        _currentTopFloor = newFloor;
    }
}
