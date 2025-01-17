using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private GameObject _windChargeParent;
    [SerializeField] private TextMeshProUGUI _windChargeCount;

    private void Start()
    {
        ShowWindCharges(0);
    }

    public void ShowWindCharges(int count)
    {
        _windChargeCount.text = count.ToString();
        _windChargeCount.gameObject.SetActive(count > 1);
        _windChargeParent.SetActive(count > 0);
    }
}
