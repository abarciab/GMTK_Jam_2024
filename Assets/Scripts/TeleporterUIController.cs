using UnityEngine;

public class TeleporterUIController : MonoBehaviour
{
    [SerializeField] private float _edgeDistance = 10;
    [SerializeField] private RectTransform _indicatorParent;
    [SerializeField] private CanvasGroup _indicatorCavnasGroup;
    [SerializeField] private RectTransform _arrowParent;
    [SerializeField] private RectTransform _arrowSprite;
    [SerializeField] private AnimationCurve _arrowSpriteSizeCurve;
    [SerializeField] private float _lerpFactor = 20;
    [SerializeField] private float _inactiveAlpha = 0.4f;
    [SerializeField] private float _activeRange = 15;
    
    private TeleportPlatform _currentSource;
    private bool _aimed;
    

    private void Update()
    {
        if (InputController.GetUp(Control.INTERACT)) AttempTeleport();

        if (_currentSource) UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_currentSource.Target.position);

        if (screenPos.z < 0f) {
            screenPos = new Vector3(Screen.width - _edgeDistance - screenPos.x, Screen.height - _edgeDistance, screenPos.z);
        }

        var x = Mathf.Clamp(screenPos.x, _edgeDistance, Screen.width - _edgeDistance);
        var y = Mathf.Clamp(screenPos.y, _edgeDistance, Screen.height - _edgeDistance);
        var z = _indicatorParent.transform.position.z;
        var targetPos = new Vector3(x, y, z);

        var lerpFactorDelta = _lerpFactor * Time.deltaTime;
        _indicatorParent.position = Vector3.Lerp(_indicatorParent.position, targetPos, lerpFactorDelta);
        _arrowParent.position = _indicatorParent.position;

        var rawDist = _indicatorParent.anchoredPosition.magnitude;

        _aimed = rawDist < _activeRange;
        _indicatorCavnasGroup.alpha = Mathf.Lerp(_indicatorCavnasGroup.alpha, _aimed ? 1 : _inactiveAlpha, lerpFactorDelta);

        var dist =  rawDist / 200;
        dist = Mathf.Clamp01(dist);

        _arrowSprite.localScale = Vector3.one * Mathf.Lerp(0, 1, _arrowSpriteSizeCurve.Evaluate(dist));
    }

    public void StartTeleporting(TeleportPlatform source)
    {
        _currentSource = source;
        gameObject.SetActive(true);
    }

    public void CancelTeleport(TeleportPlatform source)
    {
        if (source != _currentSource || _currentSource == null) return;

        gameObject.SetActive(false);
    }

    private void AttempTeleport()
    {
        if (_aimed) _currentSource.Teleport();
        gameObject.SetActive(false);
    }
}
