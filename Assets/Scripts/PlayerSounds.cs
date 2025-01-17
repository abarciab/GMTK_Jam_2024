using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerSoundKey { JUMP, JUMP_LAND, OPEN_GLIDER, WIND_LOOP, GLIDER_LAND, STUNNED_BUZZ, BOUNCE, WIND_CHARGE}

[System.Serializable]
public class PlayerSoundData
{
    [HideInInspector] public string Name;
    public Sound Sound;
    public PlayerSoundKey Key;
}

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private List<PlayerSoundData> _sounds = new List<PlayerSoundData>();

    public void OnValidate() {
        foreach (var s in _sounds) if (s.Sound) s.Name = s.Key.ToString().ToLower().Replace("_", " ")   ;
    }

    private void Start() {
        _sounds = _sounds.Where(x => x.Sound != null).ToList();
        foreach (var s in _sounds) s.Sound = Instantiate(s.Sound);
    }

    public Sound Get(PlayerSoundKey key) {
        foreach (var s in _sounds) if (s.Key == key) return s.Sound;
        Debug.LogError("failed to find sound with key: " + key);
        return null;
    }
}
