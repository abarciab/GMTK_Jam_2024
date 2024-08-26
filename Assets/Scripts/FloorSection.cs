using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSection : MonoBehaviour
{
    [SerializeField] private Transform _top;
    [SerializeField] private Transform _model;
    [SerializeField] private bool _save;

    [Header("Materials")]
    [SerializeField] private MaterialListData _generics;
    [SerializeField, ReadOnly] private List<MeshRenderer> _nonConformingRenderers = new List<MeshRenderer>();

    [HideInInspector] public Vector3 TopPos => _top ? _top.position : Vector3.zero;
    [HideInInspector] public Vector3 BottomPos => transform.position;

    private void OnValidate() {
        if (_save) _save = false;
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
        if (!_model || _model.childCount == 0) return;
        foreach (Transform child in _model) child.localScale = localScale;
    }
}
