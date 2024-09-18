using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Volume sliders")]
    [SerializeField] private Slider _masterVolume;
    [SerializeField] private Slider _musicVolume;
    [SerializeField] private Slider _ambientVolume;
    [SerializeField] private Slider _sfxVolume;

    [Header("sections")]
    [SerializeField] private List<GameObject> _sections = new List<GameObject>();
    [SerializeField] private List<SelectableItem> _tabButtons = new List<SelectableItem>();

    [Header("Misc")]
    [SerializeField] private Slider _sensitivitySlider;

    private void Start()
    {
        _masterVolume.onValueChanged.AddListener((float value) => AudioManager.i.SetMasterVolume(value));
        _musicVolume.onValueChanged.AddListener((float value) => AudioManager.i.SetMusicVolume(value));
        _ambientVolume.onValueChanged.AddListener((float value) => AudioManager.i.SetAmbientVolume(value));
        _sfxVolume.onValueChanged.AddListener((float value) => AudioManager.i.SetSfxVolume(value));
        _sensitivitySlider.onValueChanged.AddListener((float value) => ChangeSesetivity(value));
    }

    private void ChangeSesetivity(float value)
    {
        Settings.SetSensetivity(value);
    }

    private async void OnEnable()
    {
        if (!AudioManager.i) return;
        SetSliderValuesToCurrentSettings();

        await Task.Delay(100);
        foreach (var button in _tabButtons) button.Deselect();
        for (int i = 0; i < _sections.Count; i++) {
            if (_sections[i].activeInHierarchy) _tabButtons[i].SelectSilent();
        }
    }

    void SetSliderValuesToCurrentSettings()
    {
        var volumes = AudioManager.i.Volumes;
        _masterVolume.value = volumes[0];
        _musicVolume.value = volumes[1];
        _ambientVolume.value = volumes[2];
        _sfxVolume.value = volumes[3];
        _sensitivitySlider.value = Settings.MouseSensetivity;
    }

    public void SwitchTo(int index)
    {
        for (int i = 0; i < _sections.Count; i++) _sections[i].SetActive(i == index);
        for (int i = 0; i < _tabButtons.Count; i++) if (i != index) _tabButtons[i].Deselect();
    }

}
