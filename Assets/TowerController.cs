using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public string Name;

    [Range(0, 3)] public int Index;
    [Range(0, 1), SerializeField] private float _growthProgress;
    [SerializeField] private float _secondToFullExpansion = 60;
    [SerializeField] private Sound _completeSound;

    [HideInInspector] public float MaxHeight;
    [HideInInspector] public bool Complete { get; private set; }

    private List<FloorController> _floors = new List<FloorController>();
    public bool IsCurrentTower;

    private void Start()
    {
        _completeSound = Instantiate(_completeSound);
        GameManager.i.Towers.Add(this);
    }

    public void Initialize(List<FloorController> floors)
    {
        _floors = floors;
        StartCoroutine(GrowTower());
    }

    private IEnumerator GrowTower()
    {
        List<FloorController> incompleteFloors = new List<FloorController>(_floors);
        int numSteps = _floors.Count * 4;
        float step = _secondToFullExpansion / numSteps;
        for (int i = 0; i < numSteps; i++) {
            while (IsCurrentTower) yield return null;

            var selected = incompleteFloors[Random.Range(0, incompleteFloors.Count)];
            selected.TargetExpansion += 0.25f;
            if (selected.TargetExpansion > 0.9f) incompleteFloors.Remove(selected);

            yield return new WaitForSeconds(step);
        }
    }

    public float CheckProgress(float y)
    {
        if (Complete) return 1;
        var progress = Mathf.InverseLerp(transform.position.y, MaxHeight * transform.localScale.x, y);
        if (progress > 0.98) CompleteTower();
        return progress;
    }

    private void CompleteTower()
    {
        Complete = true;
        _completeSound.Play();
        GameManager.i.CompleteTower();
    }
}