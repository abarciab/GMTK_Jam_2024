using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MyBox;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public enum CardinalDirection { South, East, North, West };

[SelectionBase]
public class FloorController : MonoBehaviour
{
    public string Name;
    [SerializeField] private CardinalDirection _exitSide;

    [Header("References")]
    [SerializeField] private Transform _bridgePointParent;
    [SerializeField] private List<FloorSection> _sections = new List<FloorSection>();

    [Header("expansion")]
    [SerializeField, Range(0, 1)] private float _expansionProgress;
    [SerializeField] private AnimationCurve _expansionCurve;
    [SerializeField] private float _expansionTime = 1;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _dustParticles;
    [SerializeField] private int _dustBurstCount = 250;
    [SerializeField] private ParticleSystem _brickParticles;
    [SerializeField] private int _brickBurstCount = 50;

    [Header("Sounds")]
    [SerializeField] private Sound _slidingSound;
    [SerializeField] private Sound _growingSound1;
    [SerializeField] private Sound _growingSound2;

    [Header("Bridge")]
    [SerializeField] private GameObject _bridgePrefab;
    [SerializeField] private float _bridgeChance;

    [Header("LOD")]
    [SerializeField] private List<Renderer> _allRenderers = new List<Renderer>();

    [Header("Buttons")]
    [SerializeField] private bool _findFloorSections;

    [HideInInspector] public CardinalDirection ExitSide => _exitSide;
    [HideInInspector] public Vector3 TopPos => GetTopPos();
    [HideInInspector] public int SectionCount => _sections.Count;
    [HideInInspector] public bool Complete => _targetExpansion == 1;
    [HideInInspector] public bool HasBridge;
    [HideInInspector] public FloorController PreviousFloor;

    private FloorSection _highestActiveSection => _sections.Where(x => x.gameObject.activeInHierarchy).Last();
    private List<TowerController> _connectedTowers = new List<TowerController>(); 
    private float _targetExpansion;
    private bool _manuallyExtended;
        
    private void OnValidate()
    {
        if (_findFloorSections) {
            _findFloorSections = false;
            FindSections(); 
        }
        UpdateModel();
    }

    private void Start()
    {
        _connectedTowers.Add(GetComponentInParent<TowerController>());
        if (!_manuallyExtended) _targetExpansion = _expansionProgress = 0;
    }

    private Vector3 GetTopPos()
    {
        if (!gameObject.activeInHierarchy && PreviousFloor) return PreviousFloor.TopPos;
        else if (_sections.Where(x => x.gameObject.activeInHierarchy).ToList().Count > 0) return _highestActiveSection.TopPos;
        return transform.position;
    }

    public void SetPalette(ColorPaletteData palette) {
        foreach (Transform child in transform) SetMaterials(child, palette);
    }

    private void SetMaterials(Transform obj, ColorPaletteData palette) {
        var renderer = obj.GetComponent<MeshRenderer>();
        if (renderer) {
            var materials = new List<Material>(renderer.sharedMaterials);
            for (int i = 0; i < materials.Count; i++) {
                materials[i] = palette.GetMaterial(materials[i]);
            }
            renderer.sharedMaterials = materials.ToArray();
        }
        foreach (Transform child in obj) SetMaterials(child, palette);
    }

    private void FindSections() {
        _sections.Clear();
        foreach (Transform child in transform) {
            var section = child.GetComponent<FloorSection>();
            if (section) _sections.Add(section);
        }
    }

    public void DisableBridgePoints()
    {
        if (_bridgePointParent != null) Destroy(_bridgePointParent.gameObject);
    }

    private void FindRenderers(Transform _current)
    {
        var rend = _current.GetComponent<Renderer>();
        if (rend && !_allRenderers.Contains(rend)) _allRenderers.Add(rend);
        foreach (Transform child in _current) FindRenderers(child);
    }

    private void Update()
    {
        if (PreviousFloor) transform.position = PreviousFloor.TopPos;
        _expansionProgress = Mathf.Lerp(_expansionProgress, _targetExpansion, 2 * Time.deltaTime);
        _slidingSound.SetPercentVolume(Mathf.Abs(_targetExpansion - _expansionProgress) > 0.1f ? 1 : 0, 0.05f);
        UpdateModel();
    }

    private void OnEnable()
    {
        if (PreviousFloor) transform.position = PreviousFloor.TopPos;
        _expansionProgress = 0;
        UpdateModel();
    }

    [ButtonMethod]
    public void ExtendToFull()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        AnimateToFull();
        _manuallyExtended = true;
    }

    private void AnimateToFull()
    {
        StopAllCoroutines();
        _expansionProgress = _targetExpansion = 1;
        UpdateModel();
    }

    public void Collapse()
    {
        StopAllCoroutines();
        _expansionProgress = _targetExpansion = 0;
        UpdateModel();
    }

    public void Initialize()
    {
        if (GameManager.i && _bridgePointParent && _bridgePointParent.childCount > 1) {
            var bridgePoints = new List<Transform>();
            foreach (Transform child in _bridgePointParent) bridgePoints.Add(child);
            bridgePoints = bridgePoints.OrderBy(x => Vector3.Distance(GameManager.i.MiddlePoint, x.transform.position)).ToList();
            for (int i = 1; i < bridgePoints.Count; i++) bridgePoints[i].gameObject.SetActive(false);
        }
    }

    private void ReplaceMaterials(Transform current,  Dictionary<Material, Material> matDict)
    {
        var renderer = current.GetComponent<Renderer>();
        if (renderer) {
            var updatedMats = new List<Material>();
            for (int i = 0; i < renderer.sharedMaterials.Length; i++) {
                var originalMat = renderer.sharedMaterials[i];
                if (matDict.ContainsKey(originalMat)) updatedMats.Add(matDict[originalMat]);
                else updatedMats.Add(originalMat);
            }
            renderer.sharedMaterials = updatedMats.ToArray();
        }
        foreach (Transform child in current) ReplaceMaterials(child, matDict);
    }

    public List<Material> GetUniqueMaterials()
    {
        List<Material> mats = new List<Material>();
        //if (_rootParent) SearchForUniqueMaterial(_rootParent, mats);
        return new List<Material>(mats);
    }

    private void SearchForUniqueMaterial(Transform current, List<Material> mats)
    {
        var render = current.GetComponent<Renderer>();
        if (render) {
            foreach (var m in render.sharedMaterials) if (!mats.Contains(m)) mats.Add(m);
        }
        foreach (Transform child in current) SearchForUniqueMaterial(child, mats);
    }

    public void SetTargetExpansion(float newTarget)
    {
        _targetExpansion = newTarget;

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
        var index = Mathf.RoundToInt(progress * _sections.Count);
        var particlesY = _sections[index].transform.position.y;

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

    private void UpdateModel()
    {

        foreach (var section in _sections) {
            if (!section) {
                FindSections();
                UpdateModel();
                return;
            }
            section.transform.position = transform.position;
        }

        var minScale = new Vector3(0, 1, 0);
        for (int i = 1; i < _sections.Count; i++) {
            var max = i / (float) _sections.Count;
            var min = (i-1) / (float) _sections.Count;
            var progress = Mathf.InverseLerp(min, max, _expansionProgress);

            bool isOuter = i % 2 == 0;
            var section = _sections[i];
            var previous = _sections[i - 1];
            var next = i < _sections.Count-1 ? _sections[i + 1] : null;
            if (!section) continue;

            var highPos = previous.TopPos;
            var lowPos = isOuter ? _sections[i - 2].TopPos : transform.position;
            if (!isOuter) {
                var height = section.TopPos.y - section.BottomPos.y;
                lowPos = previous.TopPos + Vector3.down * height;
            }

            section.transform.position = Vector3.Lerp(lowPos, highPos, progress);
            if (isOuter) section.ScaleExtras(Vector3.one);
            else section.ScaleExtras(Vector3.Lerp(minScale, Vector3.one, progress)); 

            if (isOuter && previous.TopPos.y > section.BottomPos.y) {
                var diff = previous.TopPos.y - section.BottomPos.y;
                previous.transform.position += Vector3.down * diff;
            }
            if (isOuter) previous.gameObject.SetActive(previous.TopPos.y > _sections[i - 2].TopPos.y + 0.5f);
        }
    }

    private float Ease(float a, float b, float progress)
    {
        var curvedProgress = _expansionCurve.Evaluate(progress);
        return Mathf.Lerp(a, b, curvedProgress);
    }

    public Transform GetClosestBridgePoint(float y)
    {
        var pos = transform.position;
        pos.y = y;
        return GetClosestBridgePoint(pos);
    }

    public Transform GetClosestBridgePoint(Vector3 pos)
    {
        if (!_bridgePointParent || _bridgePointParent.childCount == 0) return null;

        var options = new List<Transform>();
        foreach (Transform child in _bridgePointParent) options.Add(child); 
        options = options.Where(x => x.gameObject.activeInHierarchy).OrderBy(x => Vector3.Distance(x.position, pos)).ToList();
        if (options.Count == 0) return null;
        return options[0];
    }

    /*
    [ButtonMethod]
    private void DecrementTargetExpansion()
    {
        if (_floorSections.Count != 5) return;
        if (TargetExpansion > 0) SetTargetExpansion(TargetExpansion - 0.25f);
    }
    */

    public void FinishInteriorProgress() {
        
    }

    [ButtonMethod]
    public void IncrementTargetExpansion(float time = -1)
    {
        if (time < 0) time = _expansionTime;
        StopAllCoroutines();
        _targetExpansion += 2 / (float)_sections.Count;
        _targetExpansion = Mathf.Clamp01(_targetExpansion);
        StartCoroutine(AnimateExpansion(time));

        /*if (_floorSections.Count != 5) return;
        if (TargetExpansion < 1f) SetTargetExpansion(TargetExpansion + 0.25f);
        if (TargetExpansion > 0.5f && (Random.Range(0, 1f) < _bridgeChance)) BuildBridge();*/
    }

    private IEnumerator AnimateExpansion(float time = -1) {
        if (time < 0) time = _expansionTime;
        float startProgress = _expansionProgress;
        float timePassed = 0;
        while (timePassed < time) {
            var progress = _expansionCurve.Evaluate(timePassed / time);
            _expansionProgress = Mathf.Lerp(startProgress, _targetExpansion, progress);
            UpdateModel();
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _expansionProgress = _targetExpansion;
        UpdateModel();
    }
    
    [ButtonMethod]
    public void BuildBridge()
    {
        if (HasBridge) {
            print("cannot build, already has");
            return;
        }

        var end = GameManager.i.GetFloorAtY(_connectedTowers, transform.position.y);
        if (end == null) {
            print("cannot build, no valid end found");
            return;
        }
        end.GetComponentInChildren<Lever>().gameObject.SetActive(false);
        Destroy(end.GetComponentInChildren<InterestPoint>().gameObject);
        _connectedTowers.Add(end.GetComponentInParent<TowerController>());
        HasBridge = end.HasBridge = true;
        StartCoroutine(WaitThenBuildBridge(end));
    }

    private IEnumerator WaitThenBuildBridge(FloorController end)
    {
        yield return new WaitForSeconds(1);
        var bridge = Instantiate(_bridgePrefab).GetComponent<BridgeController>();
        bridge.Initialize(GetClosestBridgePoint(end.transform.position), end.GetClosestBridgePoint(transform.position));
    }
}
