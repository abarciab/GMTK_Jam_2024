using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField] private GameObject _platform;
    [SerializeField] private float _offset;

    private Transform _player;

    private bool _testabove;

    private void Start()
    {
        _player = GameManager.i.Player.transform;
    }

    void Update()
    {
        _testabove = _player.position.y >= (transform.position.y + _offset);
        _platform.SetActive(_testabove);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + Vector3.up * _offset, 0.2f);
    }
}
