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

    private void Start()
    {
        _openSound = Instantiate(_openSound);
        _closeSound = Instantiate(_closeSound);
        _settings.SetActive(false);
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (!_openSound.Instantialized) _openSound = Instantiate(_openSound);
        _openSound.Play();
    }

    private void OnDisable()
    {
        _closeSound.Play();
    }

    public void Hide()
    {
        if (_settings.activeInHierarchy) _settings.GetComponent<Animator>().SetTrigger("Exit");    
        _animator.SetTrigger("Exit");
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
