using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class FloorSection : MonoBehaviour
{
    [SerializeField] private Transform _top;
    [SerializeField] private Transform _model;
    [SerializeField] private Transform _platforms;
    [SerializeField] private bool _save;
    [SerializeField] private float _extrasScaleTime = 1;
    [SerializeField] private AnimationCurve _scaleCurve;

    [Header("Materials")]
    [SerializeField] private MaterialListData _generics;
    [SerializeField, ReadOnly] private List<MeshRenderer> _nonConformingRenderers = new List<MeshRenderer>();

    [HideInInspector] public Vector3 TopPos => _top ? _top.position : Vector3.zero;
    [HideInInspector] public Vector3 BottomPos => transform.position;

    private void OnValidate() {
        if (_save) _save = false;
    }

    private void OnEnable()
    {
        if (Application.isPlaying) StartCoroutine(LerpExtrasScale());
    }

    private IEnumerator LerpExtrasScale()
    {
        ScaleExtras(Vector3.zero);
        float timePassed = 0;
        while (timePassed < _extrasScaleTime) {
            float progress = _scaleCurve.Evaluate(timePassed / _extrasScaleTime);
            ScaleExtras(progress * Vector3.one);

            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();  
        }
        ScaleExtras(Vector3.one);
    }


    [ButtonMethod]
    public void Setup() {
        if (Application.isPlaying) return;
        if (!_top) {
            _top = Instantiate(new GameObject(), transform).transform;
            _top.gameObject.name = "top";
#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = _top.gameObject;
#endif
        }
        if (!_platforms) {
            _platforms = Instantiate(new GameObject(), transform).transform;
            _platforms.gameObject.name = "platforms";
        }
        if (!_model) 
            _model = transform.childCount > 0 ? transform.GetChild(0) : null;
        if (_model) _model.gameObject.name = "model";
    }

    [ButtonMethod]
    public void CheckMaterials() {
        _nonConformingRenderers.Clear();
        foreach (Transform child in transform) CheckForNonConformingMaterials(child);
        if (_nonConformingRenderers.Count > 0) print("renderers with non-generic materials: " + _nonConformingRenderers.Count);
        else print("only generic materials in use");
    }

    private void CheckForNonConformingMaterials(Transform obj) {
        var renderer = obj.GetComponent<MeshRenderer>();
        if (renderer && !_nonConformingRenderers.Contains(renderer)) {
            foreach (var m in renderer.sharedMaterials) {
                if (!_generics.Materials.Contains(m)) {
                    _nonConformingRenderers.Add(renderer);
                    break;
                }
            }
        }
        foreach (Transform child in obj) CheckForNonConformingMaterials(child);
    }

    public void ScaleExtras(Vector3 localScale) {
        if (_platforms) _platforms.localScale = localScale;
    }
}
