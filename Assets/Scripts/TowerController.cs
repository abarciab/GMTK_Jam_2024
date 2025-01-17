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
    [SerializeField] private bool _isCurrentTower;
    [SerializeField] private bool _displayOnly;
    [SerializeField] private float _startOffsetTime = 1;

    private List<FloorController> _floors = new List<FloorController>();
    private TowerBuilder _builder;
    private float _percentExpanded;
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
        _initialBuildTime *= Random.Range(0.8f, 1.2f);
        _startOffsetTime *= Random.Range(0.8f, 1.2f);
        _secondToFullExpansion *= Random.Range(0.8f, 1.2f);
        _completeSound = Instantiate(_completeSound);
        if (!_builder._testTower && GameManager.i) GameManager.i.RegisterTower(this);
    }

    private void Update()
    {
        if (IsComplete || !_growing || !UIManager.i) return;

        _stunTimeLeft -= Time.deltaTime;
        UIManager.i.TowerIndicator.UpdateValue(ID, _percentExpanded, (_stunTimeLeft / _maxStunTime));
        if (_floors.Where(x => x.gameObject.activeInHierarchy).ToList().Count > 0) UIManager.i.TowerIndicator.UpdateTowerStatus(ID, _isCurrentTower, CheckProgress(GameManager.i.Player.transform.position.y));

        if (!_isCurrentTower && _stunTimeLeft < 0) {
            _timeAlone += Time.deltaTime;
            if (_timeAlone > GameManager.i.AbandonedTowerTimer) Abandon();
        }
    }

    public void Stun(float stunTime)
    {
        _stunTimeLeft = Mathf.Max(_stunTimeLeft, stunTime);
        _maxStunTime = GameManager.i.TowerStunTime;
        _abdandoned = false;
        UIManager.i.TowerIndicator.SetAbdandoned(ID, false);
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
        if (_abdandoned) GameManager.i.StunAllTowers();

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
        yield return new WaitForSeconds(_startOffsetTime);
        float intialStep = _initialBuildTime / _floors.Count;
        for (int i = 0; i < _floors.Count; i++) {
            var floor = _floors[i];
            floor.gameObject.SetActive(true);
            floor.EmitParticlesAtBase();
            yield return new WaitForSeconds(intialStep);
        }

        List<FloorController> incompleteFloors = new List<FloorController>(_floors);
        int numSteps = 0;
        foreach (var floor in _floors) numSteps += floor.SectionCount - 1;

        float step = _secondToFullExpansion / numSteps;
        for (int i = 0; i < numSteps; i++) {
            _growing = true;
            if (IsComplete) yield break;

            _percentExpanded = (i+1) / ((float)numSteps);
            while (_stunTimeLeft > 0) yield return null;

            var selected = incompleteFloors[Random.Range(0, incompleteFloors.Count)];
            if (!selected.Complete) selected.IncrementTargetExpansion(_individualFloorExpansionTime);
            else incompleteFloors.Remove(selected);

            yield return new WaitForSeconds(step * (_abdandoned ? 0.5f : 1));
        }
        GameManager.i.LoseGame();
    }

    public float CheckProgress(float y)
    {
        if (IsComplete) return 1;
        var progress = Mathf.InverseLerp(transform.position.y, MaxHeight, y);
        return progress;
    }

    public void Complete()
    {
        IsComplete = true;
        _completeSound.Play();
    }
}