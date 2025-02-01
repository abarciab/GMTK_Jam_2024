using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTowerFloor : MonoBehaviour
{
    [SerializeField] private Ladder _rope;
    [SerializeField] private GameObject _model;
    [SerializeField] private float _maxSpeed = 15;
    [SerializeField] private float _minPlayerSpinDist = 10;
    [SerializeField] private GameObject _pointOfInterest;

    private Transform _player;

    private void Awake()
    {
        if (_pointOfInterest && _pointOfInterest.gameObject) Destroy(_pointOfInterest.gameObject);
        _pointOfInterest = null;
    }

    private void Start()
    {
        _player = GameManager.i.Player.transform;
    }

    private void Update()
    {
        float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_player.position.x, _player.position.z));
        float progress = Mathf.InverseLerp(_minPlayerSpinDist, _minPlayerSpinDist + 10, dist);
        _model.transform.localEulerAngles += Vector3.up * Time.deltaTime * (_maxSpeed * progress);
    }

    public void UnrollRope()
    {
        _rope.Uncoil();
    }

    public void DestroyRope()
    {
        if (_rope && _rope.gameObject) Destroy(_rope.gameObject);
    }

    public void Unlock()
    {
        gameObject.SetActive(true);
        _model.SetActive(true);
        UnrollRope();
    }
}
