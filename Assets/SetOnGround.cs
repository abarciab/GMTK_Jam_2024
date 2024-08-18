using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOnGround : MonoBehaviour
{
    [ButtonMethod]
    private void PlaceDown()
    {
        bool didHit = Physics.Raycast(transform.position, Vector2.down, out var hitData);
        if (!didHit) return;

        var pos = transform.position;
        pos.y = hitData.point.y;
        transform.position = pos;
     }
}
