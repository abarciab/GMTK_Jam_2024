using System.Collections.Generic;
using UnityEngine;

public enum SpawnLocation { StartTutorial, StartCity, EndTutorial, StartTowers, Testing, Calvadero, Anoria, Dunvar, Thalvenhart, StartingRoom}

[System.Serializable]
public class RespawnData
{
    [HideInInspector] public string Name;
    public SpawnLocation Type;
    public GameObject SpawnParent;
}

public class SpawnController : MonoBehaviour
{
    [SerializeField] private SpawnLocation _selected;
    [Space(20)]
    [SerializeField] private List<RespawnData> _spawnPoints = new List<RespawnData>();

    public void MoveToTutorialStart() => MovePlayer(SpawnLocation.StartTutorial);
    public void MoveToTutorialEnd() => MovePlayer(SpawnLocation.EndTutorial);

    private void OnValidate()
    {
        foreach (var p in _spawnPoints) p.Name = p.Type.ToString();
    }

    private void Awake()
    {
#if !UNITY_EDITOR
        _selected = SpawnLocation.StartGame;
#endif
        foreach (var p in _spawnPoints) if (p.SpawnParent) p.SpawnParent.SetActive(p.Type == _selected);
    }

    public void MovePlayer(SpawnLocation targetLocation)
    {
        foreach (var p in _spawnPoints) if (p.SpawnParent) p.SpawnParent.SetActive(p.Type == targetLocation);
    }
}
