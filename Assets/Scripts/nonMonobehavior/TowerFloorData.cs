using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerFloorData", menuName = "Tower/TowerFloorData")]
public class TowerFloorData : ScriptableObject
{
    public GameObject floorPrefab;
    [Range(0f, 1f)] public float difficulty;
    
}
