using MyBox;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [HideInInspector] public PlayerController Player;

    private float _highScore;
    private TowerController _currentTower;
    private int _towersLeft;

    private float _playerY => Player.transform.position.y;  
    private float _towerProgress => _currentTower == null ? 0 : _currentTower.CheckProgress(_playerY);

    private void Start()
    {
        _fade.Disappear();
        HideMouse();
        _towersLeft = _totalTowerCount;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0) HideMouse();
        if (InputController.GetDown(Control.PAUSE)) TogglePause();

        CalculateHighScore();
        if (_currentTower) {
            UIManager.i.ShowCurrentTowerProgress(_towerProgress);
            if (_currentTower.Complete) UIManager.i.CompleteTower(_currentTower.Index);
        }
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
    }

    public void UpdateCurrentTower(TowerController newTower)
    {
        if (newTower == null) {
            _currentTower = newTower;
            UIManager.i.HideTowerProgress();
            return;
        }

        if (newTower != _currentTower) {
            _currentTower = newTower;
            UIManager.i.StartNewTower(newTower.Name, _towerProgress);
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
