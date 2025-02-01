using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TowerStatusUI : MonoBehaviour
{
    [SerializeField] private List<TowerStatusIndictor> _indicators = new List<TowerStatusIndictor>();
    [SerializeField] private Sound _abadonedSound;
    [SerializeField] private List<Vector2> _projectedHeights = new List<Vector2>();
    [SerializeField] private float _maxBarHeight = 100;

    public void Complete(int ID) => _indicators[ID].Complete();

    private void Start()
    {
        _abadonedSound = Instantiate(_abadonedSound);
        foreach (var indicator in _indicators) {
            indicator.SetPlayerPos(0);
            indicator.SetValue(0);
            indicator.SetStunTime(0);
            indicator.StopClimbing();
        }
    }

    public void SetAbdandoned(int ID, bool abandoned)
    {
        _indicators[ID].SetAbandoned(abandoned);
        _abadonedSound.Play();
    }

    public void UpdateValue(int ID, float currentHeight, float projectedMaxHeight, float stunTimePercent)
    {
        if (ID < 0 || ID >= _indicators.Count) return;

        var progressPercent = projectedMaxHeight > 0 ? currentHeight / projectedMaxHeight : 0;

        _indicators[ID].SetValue(progressPercent);

        while (ID >= _projectedHeights.Count) _projectedHeights.Add(Vector2.zero);
        var value = _projectedHeights[ID];
        value.x = projectedMaxHeight;
        _projectedHeights[ID] = value;
        UpdateHeightRankings();

        if (projectedMaxHeight > 0) _indicators[ID].SetHeightTarget(_maxBarHeight * _projectedHeights[ID].y);
        else _indicators[ID].SetHeight(0);

        _indicators[ID].SetStunTime(stunTimePercent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void UpdateHeightRankings()
    {
        var max = _projectedHeights.OrderByDescending(x => x.x).FirstOrDefault().x;
        if (max == 0) return;

        for (int i = 0; i < _projectedHeights.Count; i++) {
            var percentOfTallest = _projectedHeights[i].x / max;
            var data = _projectedHeights[i];
            data.y = percentOfTallest;
            _projectedHeights[i] = data;
        }
    }

    public void UpdateTowerStatus(int ID, bool current, float playerProgress)
    {
        if (current) {
            for (int i = 0; i < _indicators.Count; i++) {
                if (i != ID) _indicators[i].StopClimbing();
            }
            if (ID >= 0 && ID < _indicators.Count) _indicators[ID].SetPlayerPos(playerProgress);
        }
        else _indicators[ID].StopClimbing();
    }
}