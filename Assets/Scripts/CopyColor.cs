using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyColor : MonoBehaviour
{
    [SerializeField] private Graphic _lead;
    [SerializeField] private Graphic _follower;
    [SerializeField] private bool _useTint;
    [SerializeField, ConditionalField(nameof(_useTint))] private Color _tintColor;
    [SerializeField, ConditionalField(nameof(_useTint)), Range(0, 1)] private float _tintAmount = 0.2f;

    [ButtonMethod]
    void Update()
    {
        if (_useTint) {
            _follower.color = Color.Lerp(_lead.color, _tintColor, _tintAmount);
        }
        else {
            _follower.color = _lead.color;
        }
    }

}
