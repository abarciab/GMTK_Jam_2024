using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class ChargedPlatform : MonoBehaviour
{
    [SerializeField] private float _onTime;
    [SerializeField] private float _offTime;
    [SerializeField] private float _startOffset;
    [SerializeField] private Animator _spikes;
    [SerializeField] private Sound _chargeSound;

    [Header("Colors")]
    [SerializeField] private MeshRenderer _colorChangeRenderer;
    [SerializeField] private Material _idleMat1;
    [SerializeField] private Material _idleMat2;
    [SerializeField] private Material _idleMat3;
    [SerializeField] private Material _chargedMat1;
    [SerializeField] private Material _chargedMat2;
    [SerializeField] private Material _chargedMat3;

    private bool _charged;
    private float _currentCountdown;

    private void Start()
    {
        _currentCountdown = _offTime + _startOffset;
        _chargeSound = Instantiate(_chargeSound);
        _chargeSound.PlaySilent(transform);
    }

    private void Update()
    {
        _chargeSound.SetPercentVolume(_charged ? 1 : 0, 0.3f);

        _currentCountdown -= Time.deltaTime;
        if (_currentCountdown < 0) ToggleCharged();
    }

    private void ToggleCharged()
    {
        _charged = !_charged;
        SetCharged(_charged);
    }

    private void SetCharged(bool charged)
    {
        _spikes.SetBool("Charged", charged);

        var mats = _colorChangeRenderer.sharedMaterials.ToList();
        mats[0] = charged? _chargedMat3 : _idleMat3;
        mats[1] = charged? _chargedMat2 : _idleMat2;
        mats[2] = charged? _chargedMat1 : _idleMat1;
        _colorChangeRenderer.SetSharedMaterials(mats);

        _currentCountdown = charged ? _onTime : _offTime;
    }

    private void OnTriggerStay(Collider other) => Check(other);
    private void OnTriggerEnter(Collider other) => Check(other);

    private void Check(Collider other)
    {
        if (_charged && other.GetComponent<PlayerController>()) GameManager.i.Player.Shock();
    }
}
