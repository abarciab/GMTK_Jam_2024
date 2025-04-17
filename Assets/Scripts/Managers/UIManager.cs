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
    [SerializeField] private TextMeshProUGUI _timerText;

    [Header("Misc")]
    public DialogueController _dialogue;
    [SerializeField] private GameObject _interactPrompt;
    [SerializeField] private TextMeshProUGUI _interactText;
    [SerializeField] private GameObject _blackBlocker;
    [SerializeField] private GameObject _stunFlash;
    [SerializeField] private GameObject _deathScreen;
    [SerializeField] private GameObject _uiTutorial;
    [SerializeField] private Sound _deathSound;

    [Header("crosshair")]
    [SerializeField] private Animator _crosshairImg;

    [Header("Points Of Interest")]
    [SerializeField] private GameObject _interestIndicator;
    [SerializeField] private GameObject _detailedInterest;
    [SerializeField] private GameObject _simpleInterest;
    [SerializeField] private TextMeshProUGUI _interestDistanceText;
    [SerializeField] private float _edgeBufferDistance;
    [SerializeField] private float _maxPOIDist = 150;

    [Header("SubScripts")]
    [SerializeField] private TowerStatusUI _towerIndicator;
    [SerializeField] private CurrentTowerUI _currentTowerDisplay;
    [SerializeField] private HUDController _hud;

    private GameObject _promptSource;

    private List<(Transform, Sprite)> pointsOfInterest = new List<(Transform, Sprite)>();

    public TowerStatusUI TowerIndicator => _towerIndicator; 
    public CurrentTowerUI CurrentTower => _currentTowerDisplay;
    public HUDController HUD => _hud;
    public void FadeToBlack(float time) => _blackBlocker.SetActive(true);
    public void FadeFromBlack(float time) => _blackBlocker.SetActive(false);
    public void ShowHighScore(int score) => _highScoreText.text = _highScoreTemplateString.Replace("SCORE", (Mathf.Max(0, score - 90)).ToString());
    public void SetStunFlashVisbility(bool visible) => _stunFlash.SetActive(visible);
    public void ShowUITutorial() => _uiTutorial.SetActive(true);
    private void Awake() { i = this; }

    private void Start()
    {
        _deathSound = Instantiate(_deathSound);
        _uiTutorial.SetActive(false );
    }

    private void Update()
    {
        _timerText.text = GetTimeString((int) Time.timeSinceLevelLoad);
        if (InputController.GetDown(Control.DETAILS)) {
            bool detailed = !_detailedInterest.activeInHierarchy;
            _detailedInterest.SetActive(detailed);
            _simpleInterest.SetActive(!detailed);
        }
        _crosshairImg.SetBool("Glide", GameManager.i.Player.CanGlide || GameManager.i.Player.IsGliding);
    }

    private void LateUpdate()
    {
        AdjustPointsOfInterest();
    }

    public void CompleteUITutorial()
    {
        GameManager.i.MenusOpen -= 1;
        _uiTutorial.SetActive(false);
        GameManager.i.StartTowersGrowing();
    }

    public void Die()
    {
        if (_deathScreen.activeInHierarchy) return;
        _deathScreen.SetActive(true);
        _deathSound.Play();
    }

    private void AdjustPointsOfInterest()
    {
        float closestDistance = float.MaxValue;
        (Transform, Sprite) closestPoint = default;
        foreach (var pair in pointsOfInterest)
        {
            var point = pair.Item1;
            float thisDistance = Vector3.Distance(point.position, GameManager.i.Player.transform.position);
            if(thisDistance < closestDistance && thisDistance < _maxPOIDist)
            {
                closestPoint = pair;
                closestDistance = thisDistance;
            }
        }

        _interestIndicator.SetActive(closestPoint != default);
        if (closestPoint == default) return;

        _detailedInterest.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = closestPoint.Item2;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(closestPoint.Item1.position);
        if(screenPos.z < 0f)
        {
            screenPos = new Vector3( Screen.width - _edgeBufferDistance - screenPos.x, Screen.height - _edgeBufferDistance, screenPos.z);
        }

        var x = Mathf.Clamp(screenPos.x, _edgeBufferDistance, Screen.width - _edgeBufferDistance);
        var y = Mathf.Clamp(screenPos.y, _edgeBufferDistance, Screen.height - _edgeBufferDistance);
        var z = _interestIndicator.transform.position.z;
        _interestIndicator.transform.position = new Vector3(x, y, z);

        _interestDistanceText.text = closestDistance.ToString("0") + "m";
    }

    public void SetInteractPromptState(bool enable, GameObject source, string verb = "")
    {
        if (!enable && source != _promptSource) return;
        if (enable && source == _promptSource) return;
        if (string.IsNullOrWhiteSpace(verb)) verb = "interact";

        _interactText.text = "Press E to " + verb;
        _interactPrompt.SetActive(enable);
        if (enable)_promptSource = source;
        else _promptSource = null;
    }

    public void StartNewTower(string towerName, float progress)
    {
        _currentTowerDisplay.Set(towerName, progress);
    }

    private string GetTimeString(int seconds)
    {
        seconds = Mathf.FloorToInt(seconds);
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        string timeString = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        return timeString;
    }

    public void AddPointOfInterest(Transform point, Sprite Icon)
    {
        pointsOfInterest.Add((point, Icon));
    }

    public void RemovePointOfInterest(Transform point)
    {
        for (int i = 0; i < pointsOfInterest.Count; i++) {
            if (pointsOfInterest[i].Item1 == point) {
                pointsOfInterest.RemoveAt(i);
                return;
            }
        }
    }
    
}
