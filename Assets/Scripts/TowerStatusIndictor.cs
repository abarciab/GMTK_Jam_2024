using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerStatusIndictor : MonoBehaviour
{
    [SerializeField] private Slider _towerProgressSlider;
    [SerializeField] private Slider _playerPositionSlider;
    [SerializeField] private Image _stunImg;
    [SerializeField] private GameObject _completeTint;
    [SerializeField] private GameObject _abandonGraphic;

    public void SetValue(float value) => _towerProgressSlider.value = value;
    public void Complete()
    {
        _completeTint.SetActive(true);
        _stunImg.color = new Color(0, 0, 0, 0);
    }
    
    public void SetAbandoned(bool abandoned)
    {
        _abandonGraphic.SetActive(abandoned);
    }

    public void SetPlayerPos(float value)
    {
        _playerPositionSlider.gameObject.SetActive(true);
        _playerPositionSlider.value = _towerProgressSlider.value * value;
        _abandonGraphic.SetActive(false);
    }

    public void StopClimbing()
    {
        _playerPositionSlider.gameObject.SetActive(false);
    }

    public void SetStunTime(float percent)
    {
        _stunImg.fillAmount = percent;
        _stunImg.gameObject.SetActive(percent > 0.01f);
    }
}

