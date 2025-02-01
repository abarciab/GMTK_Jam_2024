using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputController))]
public class GameManager : MonoBehaviour
{
    public static GameManager i;

    public Transform Camera;
    public float TowerStunTime = 30;
    public float AbandonedTowerTimer = 45;
    [SerializeField] private float _abandonRecoveryStunTime = 30;

    [SerializeField] private int _totalTowerCount = 4;

    [SerializeField] pauseMenuController _pauseMenu;
    [SerializeField] Fade _fade;
    [SerializeField] MusicPlayer _music;
    [SerializeField] private GameObject _flood;
    [SerializeField] private float _trigggerFloodStartHeight = 15;
    [SerializeField] private GameObject _windChargePrefab;
    [SerializeField] private float _towerStartTimeGap;

    [HideInInspector] public PlayerController Player;
    [HideInInspector] public Vector3 MiddlePoint;

    private float _highScore;
    private TowerController _currentTower;
    private int _towersLeft;
    private bool _lostGame;
    private bool _startedGame;
    private bool _fading;
    private List<Transform> _respawnPoints = new List<Transform>();
    private List<float> _towerTimes = new List<float>();

    public int WindCharges { get; private set; }
    public List<TowerController> Towers { get; private set; } = new List<TowerController>();
    private float _playerY => Player.transform.position.y;  
    private float _playerTowerProgress => _currentTower == null ? 0 : _currentTower.CheckProgress(_playerY);
    public float GetStartTime(int ID) => _towerTimes[ID] * _towerStartTimeGap;

    private void Awake()
    {
        i = this;
        Settings.Initialize();

        _towerTimes = new List<float> { 0, 1, 2, 3 };
        _towerTimes = _towerTimes.Shuffle().ToList();
    }

    private void Start()
    { 
        _fade.Disappear();
        _towersLeft = _totalTowerCount;
        SetMouseState(false);
        _flood.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0) SetMouseState(false);
        if (InputController.GetDown(Control.PAUSE)) TogglePause();

#if UNITY_EDITOR
        if (InputController.GetDown(Control.DEBUG)) AddCharge(5);
#endif

        CalculateHighScore();

        if (_lostGame && InputController.GetDown(Control.RESPAWN)) {
            StartCoroutine(FadeThenLoadScene(1));
        }
    }

    public void AddSingleCharge() => AddCharge();

    public void AddCharge(int count = 1)
    {
        WindCharges += count;
        UIManager.i.HUD.ShowWindCharges(WindCharges);
    }

    public void AddRespawnPoint(Transform point)
    {
        _respawnPoints.Add(point);  
    }

    public void RemoveCharge()
    {
        WindCharges -= 1;
        UIManager.i.HUD.ShowWindCharges(WindCharges);
        if (_respawnPoints.Count > 0) RespawnPickup();
    }

    private async void RespawnPickup()
    {
        await Task.Delay(1000);

        if (_respawnPoints.Count == 0) return;
        var point = _respawnPoints[0];
        _respawnPoints.RemoveAt(0);
        var newPickup = Instantiate(_windChargePrefab, point.parent);
        newPickup.transform.localPosition = point.localPosition;
        newPickup.transform.localScale = point.localScale;
        Destroy(point.gameObject);
        newPickup.GetComponent<PikcupItem>().Respawn();
    }

    public void RegisterTower(TowerController _tower)
    {
        Towers.Add(_tower);
        SetMiddle();
    }

    private void SetMiddle()
    {
        var total = new Vector3();
        foreach (var t in Towers) total += transform.position;
        MiddlePoint = total/Towers.Count;
    }

    public FloorController GetFloorAtY(List<TowerController> invalid, float y)
    {
        var validTowers = Towers.Where(x => !invalid.Contains(x)).ToList();
        if (validTowers.Count == 0) return null;
        return validTowers[Random.Range(0, validTowers.Count)].GetFloorAtY(y);
    }

    public void StartTowersGrowing()
    {
        _startedGame = true;
        Settings.CompletedTutorial = true;
        foreach (var t in Towers) t.StartGrowing();
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
        foreach (var t in Towers) if (t.MaxHeight < height-20) height = t.MaxHeight;
        return height;
    }

    public void CompleteTower(TowerController tower = null)
    {
        if (tower == null) tower = _currentTower;
        if (tower == null) return;

        _towersLeft -= 1;
        tower.Complete();
        UIManager.i.TowerIndicator.Complete(tower.ID);
        if (_towersLeft == 0) StartCoroutine(WaitThenEndGame());
        else UIManager.i.HUD.MakeTowersHarder();
    }

    private IEnumerator WaitThenEndGame()
    {
        yield return new WaitForSeconds(1.5f);
        EndGame();
    }

    private void CalculateHighScore()
    {
        if (!_startedGame) return;

        var currentScore = Player.transform.position.y;
        if (Mathf.FloorToInt(currentScore) > _highScore) {
            _highScore = currentScore;
            UIManager.i.ShowHighScore(Mathf.FloorToInt(_highScore));
        }
        if (currentScore > _trigggerFloodStartHeight) {
            print("starting flood");
            _flood.SetActive(true);
        }
    }

    public void StunAllTowers()
    {
        foreach (var t in Towers) t.Stun(_abandonRecoveryStunTime);
    }

    public void AbandonAllTowers()
    {
        foreach (var t in Towers) t.ForceAbandon();
    }

    public void AddFloorsToTowers(int ID)
    {
        foreach (var t in Towers) if (t.ID != ID) t.AddFloors();
    }

    public void UpdateCurrentTower(TowerController newTower)
    {
        if (newTower == null) {
            if (_currentTower) _currentTower.SetCurrent(false);
            _currentTower = newTower;
            if (_currentTower) _currentTower.SetCurrent(true);
            UIManager.i.CurrentTower.Set("", 0);
            _music.CurrentSong = 0;
            return;
        }

        if (newTower != _currentTower) {
            if (_currentTower != null) _currentTower.SetCurrent(false);
            _currentTower = newTower;
        }

        newTower.SetCurrent(true);
        UIManager.i.CurrentTower.Set(newTower.Name, _playerTowerProgress);
        _music.CurrentSong = Mathf.FloorToInt((_playerTowerProgress + 0.2f) * 4);
    }

    private void SetMouseState(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    void TogglePause()
    {
        if (Time.timeScale == 0) Resume();
        else Pause();
    }


    public void Resume()
    {
        _pauseMenu.Hide();
        Time.timeScale = 1;
        AudioManager.i.Resume();
        SetMouseState(false);
    }

    public void Pause()
    {
        _pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0;
        AudioManager.i.Pause();
        SetMouseState(true);
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
        UIManager.i.Die();
    }


    [ButtonMethod]
    public void EndGame()
    {
        Resume();
        StartCoroutine(FadeThenLoadScene(3));
    }

    IEnumerator FadeThenLoadScene(int num)
    {
        if (_fading) yield break;
        _fading = true;

        _fade.Appear(); 
        _music.FadeOutCurrent(_fade.FadeTime);
        yield return new WaitForSeconds(_fade.FadeTime + 0.5f);
        Destroy(AudioManager.i.gameObject);
        SetMouseState(true);
        SceneManager.LoadScene(num);
    }

}
