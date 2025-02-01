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

    private RectTransform _rectTransform;
    private float _targetHeight;
    private float _lastSetHeight;

    public void SetValue(float value) => _towerProgressSlider.value = value;
    public void SetHeightTarget(float height) => _targetHeight = height;

    private void Update()
    {
        if (Mathf.Abs(_targetHeight - _lastSetHeight) > 0.01f) SetHeight(Mathf.Lerp(_lastSetHeight, _targetHeight, Time.deltaTime * 20));
    }

    public void SetHeight(float height)
    {
        if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();

        var size = _rectTransform.sizeDelta;
        size.y = height;
        _rectTransform.sizeDelta = size;
        _lastSetHeight = height;
    }

    public void Complete()
    {
        _completeTint.SetActive(true);
        _stunImg.color = new Color(0, 0, 0, 0);
    }
    
    public void SetAbandoned(bool abandoned)
    {
        _abandonGraphic.SetActive(abandoned);
        if (abandoned) _playerPositionSlider.gameObject.SetActive(false);
    }

    public void SetPlayerPos(float value)
    {
        if (_abandonGraphic.activeInHierarchy) return;
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

