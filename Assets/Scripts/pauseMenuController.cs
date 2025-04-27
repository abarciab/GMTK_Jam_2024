using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class pauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _settings;
    [SerializeField] private Sound _openSound;
    [SerializeField] private Sound _closeSound;
    private Animator _animator;

    private void OnEnable()
    {
        if (!_openSound.Instantialized) Initialize();
        _openSound.Play();
        GameManager.i.OpenMenu();
    }

    private void OnDisable()
    {
        _closeSound.Play();
    }

    private void Initialize()
    {
        _openSound = Instantiate(_openSound);
        _closeSound = Instantiate(_closeSound);
        _settings.SetActive(false);
        _animator = GetComponent<Animator>();
        _openSound = Instantiate(_openSound);
    }

    public void Hide()
    {
        if (!gameObject.activeInHierarchy) return;

        if (_settings.activeInHierarchy) _settings.GetComponent<Animator>().SetTrigger("Exit");
        if (!_animator) Initialize();
        _animator.SetTrigger("Exit");
        GameManager.i.CloseMenu();
    }

    public void Resume()
    {
        GameManager.i.Resume();
    }

    public void ExitToMainMenu()
    {
        GameManager.i.LoadMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleSettings()
    {
        _settings.SetActive(!_settings.activeInHierarchy);
    }
}
