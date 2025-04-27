using UnityEngine;

public class PointAwayFromCenterOfScreen : MonoBehaviour
{
    private RectTransform _rTransform;

    private void Start()
    {
        _rTransform = GetComponent<RectTransform>();
    }


    void Update()
    {
        var center = Vector2.zero;
        var currPos = _rTransform.anchoredPosition;

        var angleBetween = GetAngle(center, currPos);
        var eulers = new Vector3(0, 0, angleBetween);
        _rTransform.localEulerAngles = eulers;
    }

    private float GetAngle(Vector2 start, Vector2 end)
    {
        var dir = end - start;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }
}
