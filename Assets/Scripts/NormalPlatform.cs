using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalPlatform : MonoBehaviour
{
    [SerializeField] private bool _climable = true;

    private void Start()
    {
        var mantle = GetComponentInChildren<Ladder>();
        if (!mantle) return;
        mantle.gameObject.SetActive(_climable);
    }
}
