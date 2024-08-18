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

    [HideInInspector] public float MaxHeight => _floors[_floors.Count-1].TopPos.y;
    [HideInInspector] public bool Complete { get; private set; }

    private List<FloorController> _floors = new List<FloorController>();

    private void Start()
    {
        _initialBuildTime *= Random.Range(0.8f, 1.2f);
        _secondToFullExpansion *= Random.Range(0.8f, 1.2f);
        _completeSound = Instantiate(_completeSound);
        GameManager.i.Towers.Add(this);
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
        _floors = floors;
        foreach (var f in _floors) f.gameObject.SetActive(false);
        _floors[0].gameObject.SetActive(true);
        //_floors[_floors.Count-1].gameObject.SetActive(true);
    }

    public void StartGrowing()
    {
        StartCoroutine(GrowTower());
    }

    private IEnumerator GrowTower()
    {
        float intialStep = _initialBuildTime / _floors.Count;
        for (int i = 1; i < _floors.Count; i++) {
            //Debug.LogError("testerror");
            _floors[i].gameObject.SetActive(true);
            _floors[i].EmitParticlesAtBase();
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