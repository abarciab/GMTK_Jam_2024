using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MyBox;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum CardinalDirection { South, East, North, West };

public class FloorController : MonoBehaviour
{
    [SerializeField] private Transform _top;
    [SerializeField] private CardinalDirection _exitSide;

    [SerializeField] private Transform _rootParent;
    [SerializeField, ReadOnly] private List<Transform> _floorSections = new List<Transform>();
    [SerializeField, ReadOnly] private List<Transform> _hiddenExtras = new List<Transform>();

    [Header("expansion")]
    [SerializeField, Range(0, 1)] private float _expansionProgress;
    [SerializeField, ReadOnly] private List<float> _expandedOffsets = new List<float>();
    [SerializeField, ReadOnly] private List<float> _compressedOffsets = new List<float>(); 

    [Header("Particles")]
    [SerializeField] private ParticleSystem _dustParticles;
    [SerializeField] private int _dustBurstCount = 250;
    [SerializeField] private ParticleSystem _brickParticles;
    [SerializeField] private int _brickBurstCount = 50;

    [Header("Sounds")]
    [SerializeField] private Sound _slidingSound;


    [HideInInspector] public FloorController PreviousFloor;
    [HideInInspector] public CardinalDirection ExitSide => _exitSide;
    [HideInInspector] public Vector3 TopPos => !gameObject.activeInHierarchy && PreviousFloor ? PreviousFloor.TopPos : _top.position;

    [HideInInspector] public float TargetExpansion;

    private void OnValidate()
    {
        if (_floorSections.Count == 5) UpdateModel();
    }

    private void Start()
    {
        _slidingSound = Instantiate(_slidingSound);
        _slidingSound.PlaySilent(transform);
    }

    private void Update()
    {
        if (PreviousFloor) transform.position = PreviousFloor.TopPos;
        _expansionProgress = Mathf.Lerp(_expansionProgress, TargetExpansion, 2 * Time.deltaTime);
        _slidingSound.SetPercentVolume(TargetExpansion - _expansionProgress > 0.1f ? 1 : 0, 0.05f);
        UpdateModel();
    }

    private void OnEnable()
    {
        //InitializeScale(_dustParticles.transform);
        //InitializeScale(_brickParticles.transform);

        if (PreviousFloor) transform.position = PreviousFloor.TopPos;
        _expansionProgress = 0;
        UpdateModel();
    }

    private void InitializeScale(Transform transform)
    {
        var originalParent = transform.parent;
        transform.SetParent(null);
        transform.localScale = Vector3.one;
        transform.SetParent(originalParent);
    }

    [ButtonMethod]
    public void IncrementTargetExpansion()
    {
        if (_floorSections.Count != 5) return;
        if (TargetExpansion < 1f) SetTargetExpansion(TargetExpansion += 0.25f);
    }

    public void SetTargetExpansion(float newTarget)
    {
        TargetExpansion = newTarget;

        PositionParticlesByProgress(newTarget);
        EmitParticles();
    }

    public void EmitParticlesAtBase()
    {
        PositionParticlesByProgress(0);
        EmitParticles();
    }


    private void PositionParticlesByProgress(float progress)
    {
        float particlesY = 0;
        if (progress < 0.25f) particlesY = _floorSections[1].transform.position.y;
        else if (progress < 0.5f) particlesY = _floorSections[2].transform.position.y;
        else if (progress < 0.5f) particlesY = _floorSections[3].transform.position.y;
        else if (progress < 0.5f) particlesY = _floorSections[4].transform.position.y;
        var pos = _dustParticles.transform.position;
        pos.y = particlesY;
        _dustParticles.transform.position = pos;
        _brickParticles.transform.position = pos;
    }

    private void EmitParticles()
    {
        _dustParticles.Emit(_dustBurstCount);
        _brickParticles.Emit(_brickBurstCount);
    }

    [ButtonMethod]
    private void InitializeFromRoot()
    {
        if (_rootParent.childCount != 9) {
            Debug.LogError("Incorrect number of children in heirarchy. Consult template");
            return;
        }

        _hiddenExtras.Clear();
        _floorSections.Clear();

        _floorSections.Add(_rootParent);
        _floorSections.Add(_rootParent.GetChild(1));
        _floorSections.Add(_rootParent.GetChild(3));
        _floorSections.Add(_rootParent.GetChild(5));
        _floorSections.Add(_rootParent.GetChild(7));

        _hiddenExtras.Add(_rootParent.GetChild(2));
        _hiddenExtras.Add(_rootParent.GetChild(6));

        var objects = new List<Transform>();
        foreach (Transform child in _rootParent) objects.Add(child);

        for (int i = 0; i < objects.Count; i++) {
            if (i % 2 != 0 && i > 1) objects[i].SetParent(objects[i - 2]);
            if (i % 2 == 0 && i > 0) objects[i].SetParent(objects[i - 1]);
        }
        _top.SetParent(objects[objects.Count - 2]);

        print("heirarchy successfully rearranged. Next, set the compressed and expanded positions of the sections.");
    }

    private void UpdateModel()
    {
        if (_floorSections.Count != 5 || _hiddenExtras.Count != 2 || _compressedOffsets.Count + _expandedOffsets.Count != 10) return;

        float quarterProgress = Mathf.InverseLerp(0f, 0.25f, _expansionProgress);
        float secondCurrentOffset = Mathf.Lerp(_compressedOffsets[1], _expandedOffsets[1], quarterProgress);
        _floorSections[1].localPosition = Vector3.up * secondCurrentOffset;

        float extras1Progress = Mathf.InverseLerp(0.25f, 0.5f, _expansionProgress);
        var extra1Scale = new Vector3(extras1Progress, 1, extras1Progress);
        _hiddenExtras[0].localScale = extra1Scale;

        float halfProgress = Mathf.InverseLerp(0f, 0.5f, _expansionProgress);
        float thirdCurrentOffset = Mathf.Lerp(_compressedOffsets[2], _expandedOffsets[2], halfProgress);
        _floorSections[2].localPosition = Vector3.up * thirdCurrentOffset;

        float thirdQuarterProgress = Mathf.InverseLerp(0.5f, 0.75f, _expansionProgress);
        float fourthCurrentOffset = Mathf.Lerp(_compressedOffsets[3], _expandedOffsets[3], thirdQuarterProgress);
        _floorSections[3].localPosition = Vector3.up * fourthCurrentOffset;

        float extras2Progress = Mathf.InverseLerp(0.75f, 1f, _expansionProgress);
        var extra2Scale = new Vector3(extras2Progress, 1, extras2Progress);
        _hiddenExtras[1].localScale = extra2Scale;

        float secondHalfProgress = Mathf.InverseLerp(0.5f, 1f, _expansionProgress);
        float fifthCurrentOffset = Mathf.Lerp(_compressedOffsets[4], _expandedOffsets[4], secondHalfProgress);

        _floorSections[4].localPosition = Vector3.up * fifthCurrentOffset;
    }

    [ButtonMethod]
    private void SetExpandedPositions()
    {
        _expandedOffsets.Clear();

        for (int i = 0; i < _floorSections.Count; i++) {
            _expandedOffsets.Add(_floorSections[i].localPosition.y);
        }
    }

    [ButtonMethod]
    private void SetCompressedPositions()
    {
        _compressedOffsets.Clear();
        foreach (var s in _floorSections) _compressedOffsets.Add(s.localPosition.y);
    }

}
