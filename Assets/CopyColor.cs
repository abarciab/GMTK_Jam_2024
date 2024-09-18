using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyColor : MonoBehaviour
{
    [SerializeField] private Graphic _lead;
    [SerializeField] private Graphic _follower;

    void Update()
    {
        _follower.color = _lead.color;
    }
}
