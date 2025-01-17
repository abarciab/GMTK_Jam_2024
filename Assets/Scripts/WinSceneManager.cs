using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneManager : MonoBehaviour
{
    public static WinSceneManager i;
    [SerializeField] private Fade _fade;
    [SerializeField] private Sound _music;

    private void Start()
    {
        _music = Instantiate(_music);
        _music.Play();
    }

    public async void Restart()
    {
        _fade.Appear();

        int time = Mathf.RoundToInt(_fade.FadeTime * 1000);
        while (time > 0) {
            await Task.Delay(10);
            _music.SetPercentVolume(0, 0.2f);
            time -= 10;
        }

        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += UnloadCurrent;
    }

    private void UnloadCurrent(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.loadedSceneCount > 0) SceneManager.UnloadSceneAsync(3);
        SceneManager.sceneLoaded -= UnloadCurrent;
    }
}
