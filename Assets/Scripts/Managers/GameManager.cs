using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputController))]
public class GameManager : MonoBehaviour
{
    public static GameManager i;
    private void Awake() { i = this; }

    public Transform Camera;
    [SerializeField] private int _totalTowerCount = 4;


    [SerializeField] GameObject _pauseMenu;
    [SerializeField] Fade _fade;
    [SerializeField] MusicPlayer _music;
    [SerializeField] private GameObject _flood;
    [SerializeField] private float _trigggerFloodStartHeight = 15;

    [HideInInspector] public PlayerController Player;
    [HideInInspector] public List<TowerController> Towers = new List<TowerController>();

    private float _highScore;
    private TowerController _currentTower;
    private int _towersLeft;

    private float _playerY => Player.transform.position.y;  
    private float _towerProgress => _currentTower == null ? 0 : _currentTower.CheckProgress(_playerY);
    private bool _lostGame;

    private void Start()
    {
        _fade.Disappear();
        HideMouse();
        _towersLeft = _totalTowerCount;
        _flood.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0) HideMouse();
        if (InputController.GetDown(Control.PAUSE)) TogglePause();

        CalculateHighScore();
    }

    public float GetMaxHeight()
    {
        float height = 0;
        foreach (var t in Towers) if (t.MaxHeight > height) height = t.MaxHeight;
        return height;
    }

    public float GetShortestMaxHeight()
    {
        float height = GetMaxHeight();
        foreach (var t in Towers) if (t.MaxHeight < height) height = t.MaxHeight;
        return height;
    }

    public void CompleteTower()
    {
        _towersLeft -= 1;
        if (_towersLeft == 0) StartCoroutine(WaitThenEndGame());
    }

    private IEnumerator WaitThenEndGame()
    {
        yield return new WaitForSeconds(1.5f);
        EndGame();
    }

    private void CalculateHighScore()
    {
        var currentScore = Player.transform.position.y;
        if (Mathf.FloorToInt(currentScore) > _highScore) {
            _highScore = currentScore;
            UIManager.i.ShowHighScore(Mathf.FloorToInt(_highScore));
        }
        if (currentScore > _trigggerFloodStartHeight) _flood.SetActive(true);
    }

    public void UpdateCurrentTower(TowerController newTower)
    {
        if (newTower == null) {
            if (_currentTower) _currentTower.SetAsCurrentTower(false);
            _currentTower = newTower;
            if (_currentTower) _currentTower.SetAsCurrentTower(true);
            UIManager.i.HideTowerProgress();
            return;
        }
        //else if (_currentTower) _currentTower.IsCurrentTower = false;

        if (newTower != _currentTower) {
            if (_currentTower) _currentTower.SetAsCurrentTower(false);
            _currentTower = newTower;
            _currentTower.SetAsCurrentTower(true);
            UIManager.i.StartNewTower(newTower.Name, _towerProgress);
        }else {
            UIManager.i.ShowCurrentTowerProgress(_towerProgress);
            if (_currentTower.Complete) UIManager.i.CompleteTower(_currentTower.Index);
        }
    }

    private void HideMouse()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    void TogglePause()
    {
        if (Time.timeScale == 0) Resume();
        else Pause();
    }


    public void Resume()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1;
        AudioManager.i.Resume();
    }

    public void Pause()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0;
        AudioManager.i.Pause();
    }

    [ButtonMethod]
    public void LoadMenu()
    {
        Resume();
        StartCoroutine(FadeThenLoadScene(0));
    }

    public void LoseGame()
    {
        if (_lostGame) return;
        _lostGame = true;
        StartCoroutine(FadeThenLoadScene(1));
    }


    [ButtonMethod]
    public void EndGame()
    {
        Resume();
        StartCoroutine(FadeThenLoadScene(2));
    }

    IEnumerator FadeThenLoadScene(int num)
    {
        _fade.Appear(); 
        _music.FadeOutCurrent(_fade.FadeTime);
        yield return new WaitForSeconds(_fade.FadeTime + 0.5f);
        Destroy(AudioManager.i.gameObject);
        SceneManager.LoadScene(num);
    }

}
