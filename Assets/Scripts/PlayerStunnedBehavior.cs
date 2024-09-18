using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerStunnedBehavior : MonoBehaviour
{
    [SerializeField] private float _stunTime = 2;
    private float _stunTimeLeft;
    private PlayerController _controller;

    private void Start()
    {
        _controller = GetComponent<PlayerController>();
        _controller.Sounds.Get(PlayerSoundKey.STUNNED_BUZZ).PlaySilent();
    }

    private void OnEnable()
    {
        UIManager.i.SetStunFlashVisbility(true);
        _stunTimeLeft = _stunTime;
    }

    private void Update()
    {
        _controller.Sounds.Get(PlayerSoundKey.STUNNED_BUZZ).SetPercentVolume(1, 0.5f);
        _controller.ApplyFallingGravity();
        _stunTimeLeft -= Time.deltaTime;
        if (_stunTimeLeft < 0) _controller.ChangeState(PlayerState.WALK);
    }

    private void OnDisable()
    {
        _controller.Sounds.Get(PlayerSoundKey.STUNNED_BUZZ).SetPercentVolume(0);
        UIManager.i.SetStunFlashVisbility(false);
    }
}