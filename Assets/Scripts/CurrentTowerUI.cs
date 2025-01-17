using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentTowerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _towerNameText;
    [SerializeField] private TextMeshProUGUI _percentText;
    [SerializeField] private Slider _slider;
    [SerializeField] private string _nameTemplateString = "Current Tower: NAME";
    [SerializeField] private float _sliderSnapiness = 10;

    private float _targetProgress;

    public void Set(string name, float progress)
    {
        SetTowerName(name);
        SetTowerProgress(progress);
    }

    public void SetTowerProgress(float progress)
    {
        _targetProgress = progress;
    }

    public void SetTowerName(string name)  {
        _towerNameText.text = _nameTemplateString.Replace("NAME", name);
        gameObject.SetActive(!string.IsNullOrWhiteSpace(name));
    }

    private void Update()
    {
        if (Mathf.Abs(_targetProgress - _slider.value) > 0.01f) {
            _slider.value = Mathf.Lerp(_slider.value, _targetProgress, Time.deltaTime * _sliderSnapiness);
            _percentText.text = Mathf.RoundToInt(_slider.value * 100) + "%";
        }
    }
}
