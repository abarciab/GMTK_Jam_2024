using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This component, when attatched to a trigger collider, prevents the towerStartTimer from decrementing while the player is inside any one of these
/// when the player has exited all of them, they all delete themselves
/// </summary>
[RequireComponent(typeof(CapsuleCollider))]
public class SafeZoneCollider : MonoBehaviour
{
    public bool PlayerInside { get; private set; }

    private List<SafeZoneCollider> _otherSafeZones = new List<SafeZoneCollider>();
    private CapsuleCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        _otherSafeZones = FindObjectsByType<SafeZoneCollider>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
    }

    private void Update()
    {
        if (PlayerInside) GameManager.i.CountingDownTowerStart = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.timeSinceLevelLoad < 0.5f) return;
        if (other.GetComponent<PlayerController>()) {
            print("player entered" + gameObject.name);
            CheckDestroyAll();
            PlayerInside = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerController>()) PlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>()) {
            
            var dist = Utils.XZDistance(transform.position, other.transform.position);
            if (dist < _collider.radius * 0.95) return;

            print("player left" + gameObject.name);
            PlayerInside = false;
            CheckDestroyAll();
        }
    }

    private void OnDestroy()
    {
        GameManager.i.CountingDownTowerStart = true;
    }

    private void CheckDestroyAll()
    {
        var playerOutsideAll = _otherSafeZones.Where(x => x.PlayerInside).Count() == 0;
        if (playerOutsideAll) {
            foreach (var zone in _otherSafeZones) {
                Destroy(zone.gameObject);
            }
        }
    }
}
