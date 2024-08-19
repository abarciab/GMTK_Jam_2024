using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof (TowerController))]
public class TowerBuilder : MonoBehaviour
{
    [SerializeField] private GameObject _firstFloorPrefab;
    [SerializeField] private GameObject _lastFloorPrefab;
    [SerializeField] private List<TowerFloorData> _towerFloorPrefabs = new List<TowerFloorData>();
    [SerializeField, Min(2)] private int _targetHeight = 10;
    [SerializeField] private float _difficultyThreshold;

    private List<FloorController> _placedFloors = new List<FloorController>();
    private FloorController _currentTopFloor;
    private TowerController _controller;

    private void Start()
    {
        _controller = GetComponent<TowerController>();
        GenerateTower(); 
    }

    public List<FloorController> GetFloorPrefabs()
    {
        var floors = new List<FloorController>();
        foreach (var item in _towerFloorPrefabs) floors.Add(item.floorPrefab.GetComponent<FloorController>());
        return floors;
    }

    private void GenerateTower()
    {
        PlaceFirstFloor();
        for (int i = 0; i < _targetHeight - 1; i++) PlaceNextFloor();
        _controller.Initialize(_placedFloors);
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

        if(_currentTopFloor.ExitSide == CardinalDirection.North)
        {
            newFloorRotation =  new Vector3(0f, 180f + _currentTopFloor.transform.localEulerAngles.y + 90f, 0f);
        }
        else if(_currentTopFloor.ExitSide == CardinalDirection.South)
        {
             newFloorRotation =  new Vector3(0f, _currentTopFloor.transform.localEulerAngles.y + 90f, 0f);
        }
        else if(_currentTopFloor.ExitSide == CardinalDirection.East)
        {
             newFloorRotation =  new Vector3(0f, 270f + _currentTopFloor.transform.localEulerAngles.y + 90f, 0f);
        }
        else if(_currentTopFloor.ExitSide == CardinalDirection.West)
        {
             newFloorRotation =  new Vector3(0f, 90f + _currentTopFloor.transform.localEulerAngles.y + 90f, 0f);
        }

        float currentDifficulty = (float)_placedFloors.Count / _targetHeight;

        GameObject selectedPrefab = GetRandomFloorPrefabWithinDifficultyRange(currentDifficulty);
        if (_placedFloors.Count == _targetHeight - 1) selectedPrefab = _lastFloorPrefab;

        GameObject newFloorObj = Instantiate(selectedPrefab, _currentTopFloor.TopPos, Quaternion.Euler(newFloorRotation), transform);
        FloorController newFloor = newFloorObj.GetComponent<FloorController>();
        _placedFloors.Add(newFloor);
        newFloorObj.name = "floor " + _placedFloors.Count;
        _currentTopFloor = newFloor;
        if (_placedFloors.Count > 1) newFloor.PreviousFloor = _placedFloors[_placedFloors.Count-2];
    }

    private GameObject GetRandomFloorPrefabWithinDifficultyRange(float currentDifficulty)
    {
        List<GameObject> validPrefabs = new List<GameObject>();

        float tempDifficultyThreshold = _difficultyThreshold;

        while(validPrefabs.Count == 0)
        {
            foreach (TowerFloorData floor in _towerFloorPrefabs)
            {
                if(floor.difficulty >= currentDifficulty - tempDifficultyThreshold && floor.difficulty <= currentDifficulty + tempDifficultyThreshold)
                {
                    validPrefabs.Add(floor.floorPrefab);
                }
            }

            tempDifficultyThreshold += 0.05f;
        }

        return validPrefabs[Random.Range(0, validPrefabs.Count)];
    }
}
