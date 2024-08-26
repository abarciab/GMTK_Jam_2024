using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New MaterialList", menuName = "Palettes/MaterialList")]
public class MaterialListData : ScriptableObject
{
    public List<Material> Materials = new List<Material>();
}
