using UnityEngine;

public class CityController : MonoBehaviour
{
    [SerializeField] private ColorPaletteData _basePaette;
    [SerializeField] private Gradient _randomizedOptions;
    [SerializeField] private float _randomizeAmount;

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
}
