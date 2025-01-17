using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerStatusUI : MonoBehaviour
{
    [SerializeField] private List<TowerStatusIndictor> _indicators = new List<TowerStatusIndictor>();
    [SerializeField] private Sound _abadonedSound;

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

    public void UpdateValue(int ID, float value, float stunTimePercent)
    {
        if (ID < 0 || ID >= _indicators.Count) return;

        _indicators[ID].SetValue(value);
        _indicators[ID].SetStunTime(stunTimePercent);
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