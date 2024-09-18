using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Fade _fade;
    [SerializeField] private MusicPlayer _music;
    [SerializeField] private GameObject _creditsParent;
    [SerializeField] private GameObject _settingsParent;


    private void Start()
    {
        _fade.Disappear();
        Settings.Initialize();
    }

    public void StartGame()
    {
        StartCoroutine(TransitionToGame());
    }

    public void Quit()
    {
        _music.FadeOutCurrent(_fade.FadeTime);
        _fade.Appear();
#if !UNITY_EDITOR
        Invoke(nameof(Application.Quit), _fade.FadeTime);
#endif
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
