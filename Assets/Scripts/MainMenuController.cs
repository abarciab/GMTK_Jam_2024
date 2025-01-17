using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Fade _fade;
    [SerializeField] private MusicPlayer _music;
    [SerializeField] private GameObject _creditsParent;
    [SerializeField] private GameObject _settingsParent;

    private bool _quitting;

    private void Start()
    {
        _fade.Disappear();
        Settings.Initialize();
    }

    public void StartGame()
    {
        StartCoroutine(TransitionToGame());
    }

    public async void Quit()
    {
        if (_quitting) return;
        _quitting = true;

        _music.FadeOutCurrent(_fade.FadeTime);
        _fade.Appear();
        await Task.Delay(Mathf.RoundToInt(_fade.FadeTime * 1000));
        Application.Quit();
    }

    public void SetCredits(bool state)
    {
        _creditsParent.SetActive(state);
    }

    public void SetSettings(bool state)
    {
        _settingsParent.SetActive(state);
    }

    IEnumerator TransitionToGame()
    {
        _music.FadeOutCurrent(_fade.FadeTime);
        _fade.Appear();
        yield return new WaitForSeconds(_fade.FadeTime + 0.5f);
        Destroy(AudioManager.i.gameObject);
        SceneManager.LoadScene(1);
    }

}
