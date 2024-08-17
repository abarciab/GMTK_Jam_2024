using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField] private Transform _testChild;
    [SerializeField] private Transform _testParent;

    [ButtonMethod]
    private void SetParent()
    {
        _testChild.SetParent(_testParent);
    }
    
}
