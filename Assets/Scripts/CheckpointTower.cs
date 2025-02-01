using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointTower : MonoBehaviour
{
    [SerializeField] private List<CheckpointTowerFloor> _floors = new List<CheckpointTowerFloor>();
    [SerializeField] private Sound _checkpointBellSound;

    PlayerController _player;

    private void Start()
    {
        _checkpointBellSound = Instantiate(_checkpointBellSound);
        _player = GameManager.i.Player;
    }

    private void Update()
    {
        var currentlyUnlocked = _floors.Where(x => x.gameObject.activeInHierarchy).ToList();
        foreach (var floor in _floors) {
            if (!floor.gameObject.activeInHierarchy && _player.transform.position.y >  floor.gameObject.transform.position.y && !_player.IsGliding) {

                UIManager.i.HUD.UnlockCheckpoint();
                _checkpointBellSound.Play();

                foreach (var f in currentlyUnlocked) {
                    f.DestroyRope();
                    f.gameObject.SetActive(false);
                }
                
                floor.Unlock();

                foreach (var f in currentlyUnlocked) f.gameObject.SetActive(true);
            }
        }
    }
}
