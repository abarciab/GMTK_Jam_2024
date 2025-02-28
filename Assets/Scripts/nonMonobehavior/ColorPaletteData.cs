using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialPairData
{
    [HideInInspector] public string Name;
    [HideInInspector] public Material InputMaterial;
    [OverrideLabel("Material")]public Material OutputMaterial;
}

[CreateAssetMenu(fileName = "New Palette", menuName = "Palettes/Palette")]
public class ColorPaletteData : ScriptableObject
{
    [SerializeField] private MaterialListData _inputData;
    [SerializeField] private List<MaterialPairData> _pairs = new List<MaterialPairData>();
    private MaterialListData GetInputData => _inputData; 
    private List<MaterialPairData> GetPairs => new List<MaterialPairData>(_pairs); 

    public ColorPaletteData() {}
    public ColorPaletteData(ColorPaletteData original)
    {
        _inputData = original.GetInputData;
        _pairs = original.GetPairs;
    }

    private void OnValidate() {
        if (!_inputData) return;

        while (_pairs.Count < _inputData.Materials.Count) _pairs.Add(new MaterialPairData());
        while (_pairs.Count > _inputData.Materials.Count) _pairs.RemoveAt(_pairs.Count);

        for (int i = 0; i < _pairs.Count; i++) {
            var pair = _pairs[i];
            pair.InputMaterial = _inputData.Materials[i];
            if (pair.OutputMaterial) pair.Name = pair.InputMaterial.name + ": " + pair.OutputMaterial.name;
            else pair.Name = pair.InputMaterial.name + ": ";
        }
    }

    public Material GetMaterial(Material input) {
        foreach (var pair in _pairs) {
            if (pair.InputMaterial == input) return pair.OutputMaterial ? pair.OutputMaterial : pair.InputMaterial;
        }
        return input;
    }


    public Material ReverseMaterial(Material input)
    {
        foreach (var pair in _pairs) {
            if (pair.OutputMaterial == input) return pair.InputMaterial;
        }
        return input;
    }
}
