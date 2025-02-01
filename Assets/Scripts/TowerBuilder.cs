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

    public ColorPaletteData Palette => _palette;

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
        var selectedPrefab = _towerFloorPrefabs[Random.Range(0, _towerFloorPrefabs.Count)];
        if (_currentTopFloor) {
            var options = _towerFloorPrefabs.Where(x => x.GetComponent<FloorController>().Name != _currentTopFloor.Name).ToList();
            if (options.Count > 0) selectedPrefab = options[Random.Range(0, options.Count)];
        }

        if (_placedFloors.Count == _targetHeight - 1) selectedPrefab = _lastFloorPrefab;
        if (_placedFloors.Count == 0) selectedPrefab = _firstFloorPrefab;
        PlaceFloor(selectedPrefab);
    }

    private void PlaceFloor(GameObject floorPrefab)
    {
        var y = 0;
        var newFloorRot = Vector3.zero;
        var newFloorPos = transform.position;
        if (_currentTopFloor) {
            newFloorRot = RotFromPrevious(_currentTopFloor.ExitSide, _currentTopFloor.transform.localEulerAngles.y);
            newFloorPos = _currentTopFloor.TopPos;
        }

        GameObject newFloorObj = Instantiate(floorPrefab, newFloorPos, Quaternion.identity, transform);
        newFloorObj.transform.localRotation = Quaternion.Euler(newFloorRot);
        FloorController newFloor = newFloorObj.GetComponent<FloorController>(); 
        if (_placedFloors.Count > 0) newFloor.PreviousFloor = _placedFloors[^1];

        _placedFloors.Add(newFloor);
        newFloorObj.name = "floor " + _placedFloors.Count;
        _currentTopFloor = newFloor;
        newFloor.SetPalette(_palette);
    }

    public Vector3 RotFromPrevious(CardinalDirection dir, float _prevLocalY)
    {
        var y = 0;
        if (dir == CardinalDirection.South) y = 360;
        if (dir == CardinalDirection.East) y = 270;
        if (dir == CardinalDirection.North) y = 180;
        if (dir == CardinalDirection.West) y = 90;
       return new Vector3(0, y + _prevLocalY, 0);
    }
}
