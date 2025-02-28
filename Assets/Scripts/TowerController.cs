using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(TowerBuilder))]
public class TowerController : MonoBehaviour
{
    public string Name;

    [Range(0, 3)] public int ID;
    [SerializeField] private float _initialBuildTime = 25;
    [SerializeField] private float _secondToFullExpansion = 60;
    [SerializeField] private float _individualFloorExpansionTime = 3;
    [SerializeField] private Sound _completeSound;
    [SerializeField] private Sound _calmSound;
    [SerializeField] private bool _isCurrentTower;
    [SerializeField] private bool _displayOnly;
    [SerializeField] private float _startOffsetTime = 1;
    [SerializeField] private ColorPaletteData _completePalette;

    [Header("Extra floors")]
    [SerializeField] private List<GameObject> _extraFloors = new List<GameObject>();
    [SerializeField] private int _numFloorsToAdd = 5;

    private List<FloorController> _floors = new List<FloorController>();
    private TowerBuilder _builder;
    private float _currentHeight;
    private float _projectedTop;
    private float _stunTimeLeft;
    private float _maxStunTime;
    private float _timeAlone;
    private bool _abdandoned;
    private bool _growing;

    [HideInInspector] public float MaxHeight => _floors[_floors.Count - 1].TopPos.y;
    [HideInInspector] public bool IsComplete { get; private set; }

    private void Start()
    {
        _builder = GetComponent<TowerBuilder>();

        if (!_builder._testTower && GameManager.i) {
            GameManager.i.RegisterTower(this);
            _startOffsetTime = GameManager.i.GetStartTime(ID);
        }

        _initialBuildTime *= Random.Range(0.8f, 1.2f);
        _startOffsetTime *= Random.Range(0.8f, 1.2f);
        _secondToFullExpansion *= Random.Range(0.8f, 1.2f);
        _completeSound = Instantiate(_completeSound);
        _calmSound = Instantiate(_calmSound);

        if (UIManager.i) UIManager.i.TowerIndicator.UpdateValue(ID, 0, 0, 0);
    }

    private void Update()
    {
        if (IsComplete) {
            _currentHeight = _floors[^1].TopPos.y - transform.position.y;
            UIManager.i.TowerIndicator.UpdateValue(ID, _currentHeight, _currentHeight, 0);
            return;
        }
        if (!_growing || !UIManager.i) return;

        _stunTimeLeft -= Time.deltaTime;
        UIManager.i.TowerIndicator.UpdateValue(ID, _currentHeight, _projectedTop, (_stunTimeLeft / _maxStunTime));
        if (_floors.Where(x => x.gameObject.activeInHierarchy).ToList().Count > 0) UIManager.i.TowerIndicator.UpdateTowerStatus(ID, _isCurrentTower, CheckProgress(GameManager.i.Player.transform.position.y));

        if (!_isCurrentTower && _stunTimeLeft < 0) {
            _timeAlone += Time.deltaTime;
            if (_timeAlone > GameManager.i.AbandonedTowerTimer) Abandon();
        }
    }

    public void AddFloors()
    {
        if (_extraFloors.Count == 0) return;
        for (int i = 0; i < _numFloorsToAdd; i++) {
            AddNewFloor(_extraFloors[0]);
        }
    }

    private void AddNewFloor(GameObject floor)
    {
        if (IsComplete) return;
        var index = Random.Range(1, _floors.Count);

        var prevFloor = _floors[index - 1];
        var newFloorRot = _builder.RotFromPrevious(prevFloor.ExitSide, prevFloor.transform.localEulerAngles.y);
        var newFloorPos = prevFloor.TopPos;

        var newFloorObj = Instantiate(floor, newFloorPos, Quaternion.identity, transform);
        newFloorObj.transform.localRotation = Quaternion.Euler(newFloorRot);
        var newFloor = newFloorObj.GetComponent<FloorController>();
        newFloor.PreviousFloor = prevFloor;
        newFloor.SetPalette(_builder.Palette);
        _projectedTop += newFloor.MaxHeight * transform.localScale.y;

        _floors.Insert(index, newFloor);

        for (int i = index + 1; i < _floors.Count; i++) {
            var prev = _floors[i - 1];
            _floors[i].PreviousFloor = prev;
            _floors[i].transform.position = prev.TopPos;
            var newRot = _builder.RotFromPrevious(prev.ExitSide, prev.transform.localEulerAngles.y);
            _floors[i].transform.localRotation = Quaternion.Euler(newRot);
        }

        _currentHeight = _floors[^1].TopPos.y - transform.position.y;
    }

    public void Stun(float stunTime)
    {
        _stunTimeLeft = Mathf.Max(_stunTimeLeft, stunTime);
        _maxStunTime = GameManager.i.TowerStunTime;
        _abdandoned = false;
        UIManager.i.TowerIndicator.SetAbdandoned(ID, false);
    }

    public void ForceAbandon()
    {
        _stunTimeLeft = -1;
        _timeAlone = GameManager.i.AbandonedTowerTimer * 2;
        Abandon();
    }

    private void Abandon()
    {
        if (_abdandoned) return;
        _abdandoned = true;
        UIManager.i.TowerIndicator.SetAbdandoned(ID, true);
    }

    public FloorController GetFloorAtY(float y)
    {
        FloorController closestFloor = null;
        float shortestDist = Mathf.Infinity;
        foreach (var floor in _floors) {
            var dist = Mathf.Abs(floor.transform.position.y - y);
            if (dist > shortestDist || floor.transform.position.y <= y || dist > 30 || floor.HasBridge || !floor.GetClosestBridgePoint(y)) continue;
            shortestDist = dist;
            closestFloor = floor;
        }
        return closestFloor;
    }

    public void SetCurrent(bool isCurrent)
    {
        if (!IsComplete && _abdandoned) {
            GameManager.i.StunAllTowers();
            _calmSound.Play();
        }

        _isCurrentTower = isCurrent;
        if (isCurrent) {
            FinishAllInProgressFloors();
            _maxStunTime = _stunTimeLeft = GameManager.i.TowerStunTime;
            _timeAlone = 0;
            _abdandoned = false;
        }
    }

    private void FinishAllInProgressFloors()
    {
        foreach (var f in _floors) {
            f.FinishInteriorProgress();
        }
    }

    public void Initialize(List<FloorController> floors)
    {
        if (!_builder) _builder = GetComponent<TowerBuilder>();
        _floors = floors;
        foreach (var f in _floors) f.gameObject.SetActive(false);

        if (_displayOnly) StartGrowing();
        if (_builder._testTower) StartGrowing();

    }

    public void StartGrowing()
    {
        StartCoroutine(GrowTower());
    }

    private IEnumerator GrowTower()
    {
        _projectedTop = 0;
        _currentHeight = 0;
        var yScale = transform.localScale.y;
        if (UIManager.i) UIManager.i.TowerIndicator.UpdateValue(ID, 0, _projectedTop, 0);

        yield return new WaitForSeconds(_startOffsetTime);
        float intialStep = _initialBuildTime / _floors.Count;
        for (int i = 0; i < _floors.Count; i++) {
            var floor = _floors[i];
            floor.gameObject.SetActive(true);
            floor.EmitParticlesAtBase();
            yield return new WaitForSeconds(intialStep);

            _currentHeight = floor.TopPos.y - transform.position.y;
            _projectedTop += floor.MaxHeight * yScale;
            if (UIManager.i) UIManager.i.TowerIndicator.UpdateValue(ID, _currentHeight, _projectedTop, 0);
        }

        List<FloorController> completeFloors = new List<FloorController>();

        for (int i = 0; i < GetNumStepsTotal(); i++) {
            if (!_growing) _growing = true;
            _currentHeight = _floors[^1].TopPos.y - transform.position.y;

            if (IsComplete) yield break;
            while (_stunTimeLeft > 0) yield return null;

            //if (_builder._testTower) yield break;

            var incompleteFloors = _floors.Where(x => !completeFloors.Contains(x)).ToList();
            var selected = incompleteFloors[Random.Range(0, incompleteFloors.Count)];
            if (selected.Complete) completeFloors.Add(selected);
            else selected.IncrementTargetExpansion(_individualFloorExpansionTime);

            var step = _secondToFullExpansion / GetNumStepsTotal();
            yield return new WaitForSeconds(step * (_abdandoned ? 0.5f : 1));
        }
        if (!_builder._testTower) GameManager.i.LoseGame();
    }

    private int GetNumStepsTotal()
    {
        int total = 0; 
        foreach (var floor in _floors) total += floor.SectionCount - 1;
        return total;
    }

    public float CheckProgress(float y)
    {
        if (IsComplete) return 1;
        var progress = Mathf.InverseLerp(transform.position.y, MaxHeight, y);
        return progress;
    }

    public void SetColor(ColorPaletteData palette)
    {
        print("setting color: " + palette.name);
        foreach (var f in _floors) f.SetPalette(palette);
    }

    public void Complete()
    {
        if (_completePalette) foreach (var f in _floors) f.ResetAndChangePalette(_builder.Palette, _completePalette);
        StopAllCoroutines();
        SetColor(_completePalette);

        IsComplete = true;
        _completeSound.Play();
        //GameManager.i.StunAllTowers();
        GameManager.i.AbandonAllTowers();
        GameManager.i.AddFloorsToTowers(ID);

        _currentHeight = _floors[^1].TopPos.y - transform.position.y;
        UIManager.i.TowerIndicator.UpdateValue(ID, _currentHeight, _currentHeight, 0);

    }
}