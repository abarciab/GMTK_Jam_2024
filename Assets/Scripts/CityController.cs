using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class CityController : MonoBehaviour
{
    [SerializeField] private ColorPaletteData _basePaette;
    [SerializeField] private Gradient _randomizedOptions;
    [SerializeField] private float _randomizeAmount;
    [SerializeField] private List<GameObject> _cityWalls;

    private bool _readyToToggle;

    public void ToggleWallColliders() => ToggleWallColliders(false);

    private void Start()
    {
        ColorCity();
    }

    private void ColorCity()
    {
        var buildings = GetComponentsInChildren<FloorController>(true);
        foreach (var building in buildings) {
            building.SetPalette(_basePaette, _randomizedOptions, _randomizeAmount);
        }
    }

    private void Update()
    {
        if (_readyToToggle) ToggleWallColliders(true);
    }

    private void ToggleWallColliders(bool fromUpdate)
    {
        if (!fromUpdate) _readyToToggle = true;
        else {
            foreach (var wall in _cityWalls) wall.SetActive(!wall.activeInHierarchy);
        _readyToToggle = false;
        }
    }
}
