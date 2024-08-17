using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager i;

    [Header("High score")]
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private string _highScoreTemplateString = "High score: SCOREm";

    [Header("Tower progress")]
    [SerializeField] private GameObject _towerProgressParent;
    [SerializeField] private Slider _towerProgresSlider;
    [SerializeField] private TextMeshProUGUI _currentTowerText;
    [SerializeField] private string _currentTowerTemplateString = "Current Tower: NAME";

    [Header("Completed Towers")]
    [SerializeField] private List<SelectableItem> _completedTowerIcons = new List<SelectableItem>();

    [Header("Inventory")]
    [SerializeField] private List<Image> _inventoryImages;
    [SerializeField] private Image _inventorySelectionIndicator;

    private float _targetTowerProgress;


    public void ShowHighScore(int score) => _highScoreText.text = _highScoreTemplateString.Replace("SCORE", score.ToString());
    public void ShowCurrentTowerProgress(float progress) => _targetTowerProgress = progress;
    public void HideTowerProgress() => _towerProgressParent.SetActive(false);
    public void CompleteTower(int index) => _completedTowerIcons[index].SetEnabled(true);

    private void Awake() { i = this; }

    private void Start()
    {
        foreach (var icon in _completedTowerIcons) icon.SetEnabled(false);
        foreach (var image in _inventoryImages) image.enabled = false;
        _inventorySelectionIndicator.enabled = false;
    }

    private void Update()
    {
        _towerProgresSlider.value = Mathf.Lerp(_towerProgresSlider.value, _targetTowerProgress, 8 * Time.deltaTime);
    }

    public void StartNewTower(string towerName, float progress)
    {
        _towerProgressParent.SetActive(true);
        ShowCurrentTowerProgress(progress);
        _currentTowerText.text = _currentTowerTemplateString.Replace("NAME", towerName);
    }

    private string GetTimeString(int seconds)
    {
        seconds = Mathf.FloorToInt(seconds);
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        string timeString = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        return timeString;
    }

    public void SetInventoryImage(Sprite sprite, int index)
    {
        _inventoryImages[index].sprite = sprite;
        _inventoryImages[index].enabled = true;
    }

    public void RemoveInventoryImage(int index)
    {
        _inventoryImages[index].sprite = null;
        _inventoryImages[index].enabled = false;
    }

    public void SelectInventoryImage(int index)
    {
        _inventorySelectionIndicator.transform.position = _inventoryImages[index].transform.position;
        _inventorySelectionIndicator.enabled = true;
    }
    
}
