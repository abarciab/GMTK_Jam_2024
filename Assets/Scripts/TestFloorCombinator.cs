using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFloorCombinator : MonoBehaviour
{
    [Header("parameters")]
    [SerializeField] private FloorController _testFloor;
    [SerializeField] private bool _includeGeneric;
    [SerializeField] private bool _includeTeleport;
    [SerializeField] private bool _includeBounce;
    [SerializeField] private bool _includeCharged;
    [SerializeField] private bool _includeUnstable;

    [Header("Library")]
    [SerializeField] private List<FloorController> _genericFloors;
    [SerializeField] private List<FloorController> _teleportFloors;
    [SerializeField] private List<FloorController> _bounceFloors;
    [SerializeField] private List<FloorController> _chargedFloors;
    [SerializeField] private List<FloorController> _unstableFloors;

    [Header("Placement")]
    [SerializeField] private GameObject _testTowerPrefab;
    [SerializeField] private Transform _corner;
    [SerializeField] private float _gapSize;

    private List<FloorController> _possibleNeighbors = new List<FloorController>();
    private List<TowerController> _placedTowers = new List<TowerController>();

    [ButtonMethod]
    private void Start()
    {
        CalculatePossibleNeighbors();

        var towerOptions = GetAllTowerOptions();
        foreach (var option in towerOptions) PlaceTower(option);
    }

    private void PlaceTower(List<FloorController> floors)
    {
        int index = _placedTowers.Count;
        int width = _possibleNeighbors.Count;
        int x = Mathf.FloorToInt(index / width);
        int y = index % width;

        var pos = new Vector3(x, 0, y) * _gapSize + _corner.position;
        var newTower = Instantiate(_testTowerPrefab.gameObject, pos, Quaternion.identity, transform);
        newTower.GetComponent<TowerBuilder>().TestBuild(floors, "test " + (_placedTowers.Count + 1));
        _placedTowers.Add(newTower.GetComponent<TowerController>());    
    }

    private void CalculatePossibleNeighbors()
    {
        _possibleNeighbors.Clear();
        if (_includeGeneric) _possibleNeighbors.AddRange(_genericFloors);
        if (_includeTeleport) _possibleNeighbors.AddRange(_teleportFloors);
        if (_includeBounce) _possibleNeighbors.AddRange(_bounceFloors);
        if (_includeCharged) _possibleNeighbors.AddRange(_chargedFloors);
        if (_includeUnstable) _possibleNeighbors.AddRange(_unstableFloors);
    }

    private List<List<FloorController>> GetAllTowerOptions()
    {
        List<List<FloorController>> options = new List<List<FloorController>>();
        foreach (var bottomFloor in _possibleNeighbors) {
            var baseTower = new List<FloorController>();
            baseTower.Add(bottomFloor);
            baseTower.Add(_testFloor);
            foreach (var topFloor in _possibleNeighbors) {
                var tower = new List<FloorController>(baseTower);
                tower.Add(topFloor);
                options.Add(tower);
            }
        }
        return options;
    }
}
