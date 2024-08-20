using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOnGround : MonoBehaviour
{
    [SerializeField] private float _offset = 0.1f;
    [SerializeField] private bool _save;

    private void OnValidate()
    {
        if (_save) _save = false;
    }

    [ButtonMethod]
    private void PlaceDown()
    {
        bool didHit = Physics.Raycast(transform.position, Vector2.down, out var hitData);
        if (!didHit) return;

        var pos = transform.position;
        pos.y = hitData.point.y + _offset;
        transform.position = pos;
     }
}
