using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(TowerController))]
public class TowerBuilder : MonoBehaviour
{
    [SerializeField] private GameObject _firstFloorPrefab;
    [SerializeField] private GameObject _lastFloorPrefab;
    [SerializeField] private List<GameObject> _towerFloorPrefabs = new List<GameObject>();
    [SerializeField, Min(2)] private int _targetHeight = 10;
    [SerializeField] private float _difficultyThreshold;
    [SerializeField] private ColorPaletteData _palette;
    public bool _testTower;

    private List<FloorController> _placedFloors = new List<FloorController>();
    private FloorController _currentTopFloor;
    private TowerController _controller;

    private void Start()
    {
        _controller = GetComponent<TowerController>();
        if (!_testTower) GenerateTower();
    }

    public void TestBuild(List<FloorController> floors, string newName = "")
    {
        if (newName != "") GetComponent<TowerController>().Name = newName;

        _targetHeight = floors.Count;
        name = string.Join(" ", floors.Select(x => x.Name)); 
        _controller = GetComponent<TowerController>();

        _firstFloorPrefab = floors[0].gameObject;
        floors.RemoveAt(0);
        _lastFloorPrefab = floors[^1].gameObject;
        floors.RemoveAt(floors.Count - 1);

        _towerFloorPrefabs = floors.Select(x => x.gameObject).ToList();
        GenerateTower();
    }

    public List<FloorController> GetFloorPrefabs()
    {
        var floors = new List<FloorController>();
        foreach (var item in _towerFloorPrefabs) floors.Add(item.GetComponent<FloorController>());
        return floors;
    }

    private void GenerateTower()
    {
        for (int i = 0; i < _targetHeight; i++) PlaceNextFloor();
        _controller.Initialize(_placedFloors);
    }

    private void PlaceNextFloor()
    {
        GameObject floorPrefab = _towerFloorPrefabs[Random.Range(0, _towerFloorPrefabs.Count)];
        if (_placedFloors.Count == _targetHeight - 1) floorPrefab = _lastFloorPrefab;
        if (_placedFloors.Count == 0) floorPrefab = _firstFloorPrefab;
        PlaceFloor(floorPrefab);
    }

    private void PlaceFloor(GameObject floorPrefab)
    {
        var y = 0;
        var newFloorRot = Vector3.zero;
        var newFloorPos = transform.position;
        if (_currentTopFloor) {
            var dir = _currentTopFloor.ExitSide;
            if (dir == CardinalDirection.South) y = 360;
            if (dir == CardinalDirection.East) y = 270;
            if (dir == CardinalDirection.North) y = 180;
            if (dir == CardinalDirection.West) y = 90;
            newFloorRot = new Vector3(0, y + _currentTopFloor.transform.localEulerAngles.y, 0);
            newFloorPos = _currentTopFloor.TopPos;
        }

        GameObject newFloorObj = Instantiate(floorPrefab, newFloorPos, Quaternion.Euler(newFloorRot), transform);
        FloorController newFloor = newFloorObj.GetComponent<FloorController>(); 
        if (_placedFloors.Count > 0) newFloor.PreviousFloor = _placedFloors[^1];

        _placedFloors.Add(newFloor);
        newFloorObj.name = "floor " + _placedFloors.Count;
        _currentTopFloor = newFloor;
        newFloor.SetPalette(_palette);
    }
}
