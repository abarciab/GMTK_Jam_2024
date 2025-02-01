using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private GameObject _windChargeParent;
    [SerializeField] private GameObject _checkpointUnlockedPopup;
    [SerializeField] private GameObject _harderTowersPopup;
    [SerializeField] private TextMeshProUGUI _windChargeCount;
    [SerializeField] private Animator _windChargeAnimator;
    private int _lastCount;

    public void UnlockCheckpoint() => _checkpointUnlockedPopup.SetActive(true);
    public void MakeTowersHarder() => _harderTowersPopup.SetActive(true);

    private void Start()
    {
        ShowWindCharges(0);
    }

    public void ShowWindCharges(int count)
    { 
        _windChargeCount.text = count.ToString();
        _windChargeCount.gameObject.SetActive(count > 1);
        _windChargeParent.SetActive(count > 0);

        if (count > _lastCount) _windChargeAnimator.SetTrigger("Add");
        _lastCount = count;
    }
}
