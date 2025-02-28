using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeChildMaterials : MonoBehaviour
{
    [SerializeField] private bool _update;
    [SerializeField] private List<Material> _materials; 

    private void OnValidate()
    {
        if (_update) UpdateMaterials();
    }

    private void UpdateMaterials()
    {
        _update = false;
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers) r.sharedMaterial = _materials[Random.Range(0, _materials.Count)];
    }
}
