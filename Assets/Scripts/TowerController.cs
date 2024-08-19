using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public string Name;

    [Range(0, 3)] public int Index;
    [SerializeField] private float _initialBuildTime = 25;
    [SerializeField] private float _secondToFullExpansion = 60;
    [SerializeField] private Sound _completeSound;
    [SerializeField] private bool _isCurrentTower;
    [SerializeField] private bool _displayOnly;
    [SerializeField] private float _startOffsetTime = 1;

    [Header("Materials")]
    [SerializeField, ReadOnly] private List<Material> _allMaterials = new List<Material>();
    [SerializeField] private List<Material> _altMaterials = new List<Material>();

    [HideInInspector] public float MaxHeight => _floors[_floors.Count-1].TopPos.y;
    [HideInInspector] public bool Complete { get; private set; }

    private List<FloorController> _floors = new List<FloorController>();
    private Dictionary<Material, Material> _matDict = new Dictionary<Material, Material>();


    private void Start()
    {

        _initialBuildTime *= Random.Range(0.8f, 1.2f);
        _startOffsetTime *= Random.Range(0.8f, 1.2f);
        _secondToFullExpansion *= Random.Range(0.8f, 1.2f);
        _completeSound = Instantiate(_completeSound);
        if (GameManager.i) GameManager.i.RegisterTower(this);
    }

    public FloorController GetFloorAtY(float y)
    {
        FloorController closestFloor = null;
        float shortestDist = Mathf.Infinity;
        foreach (var floor in _floors) {
            var dist = Mathf.Abs(floor.transform.position.y - y);
            if (dist > shortestDist || floor.transform.position.y <= y || floor.HasBridge) continue;
            shortestDist = dist;
            closestFloor = floor;
        }
        return closestFloor;
    }

    private void BuildMatDict()
    {
        _matDict.Clear();
        for (int i = 0; i < _allMaterials.Count; i++) {
            _matDict.Add(_allMaterials[i], _altMaterials[i]);
        }
    }

    [ButtonMethod]
    private void GetMaterials()
    {
        _allMaterials.Clear();
        foreach (FloorController floor in GetComponent<TowerBuilder>().GetFloorPrefabs()) {
            foreach (var mat in floor.GetUniqueMaterials()) if (!_allMaterials.Contains(mat)) _allMaterials.Add(mat);
        }
        _altMaterials = new List<Material>(_allMaterials);
    }

    public void SetAsCurrentTower(bool isCurrent)
    {
        _isCurrentTower = isCurrent;
        if (isCurrent) FinishAllInProgressFloors();
    }

    private void FinishAllInProgressFloors()
    {
        foreach (var f in _floors) {
            if (f.TargetExpansion > 0.5f) f.TargetExpansion = 1;
            if (f.TargetExpansion > 0 && f.TargetExpansion < 0.5f) f.TargetExpansion = 0.5f;
        }
    }

    public void Initialize(List<FloorController> floors)
    {
        BuildMatDict();
        _floors = floors;
        foreach (var f in _floors) f.gameObject.SetActive(false);
        if (!_displayOnly)_floors[0].gameObject.SetActive(true);
        //_floors[_floors.Count-1].gameObject.SetActive(true);
        if (_displayOnly) StartGrowing();
    }

    public void StartGrowing()
    {
        StartCoroutine(GrowTower());
    }

    private IEnumerator GrowTower()
    {
        //print("Growing");
        yield return new WaitForSeconds(_startOffsetTime);
        float intialStep = _initialBuildTime / _floors.Count;
        for (int i = _displayOnly ? 0 :  1; i < _floors.Count; i++) {
            var floor = _floors[i];
            floor.gameObject.SetActive(true);
            floor.EmitParticlesAtBase();
            floor.Initialize(_matDict);
            yield return new WaitForSeconds(intialStep);
        }

        List<FloorController> incompleteFloors = new List<FloorController>(_floors);
        int numSteps = _floors.Count * 4;
        float step = _secondToFullExpansion / numSteps;
        for (int i = 0; i < numSteps; i++) {
            while (_isCurrentTower) yield return null;

            var selected = incompleteFloors[Random.Range(0, incompleteFloors.Count)];
            if (selected.TargetExpansion > 0.9f) incompleteFloors.Remove(selected);
            else selected.IncrementTargetExpansion();

            yield return new WaitForSeconds(step);
        }
    }

    public float CheckProgress(float y)
    {
        if (Complete) return 1;
        var progress = Mathf.InverseLerp(transform.position.y, MaxHeight, y);
        if (y >= MaxHeight) CompleteTower();
        return progress;
    }

    private void CompleteTower()
    {
        Complete = true;
        _completeSound.Play();
        GameManager.i.CompleteTower();
    }
}