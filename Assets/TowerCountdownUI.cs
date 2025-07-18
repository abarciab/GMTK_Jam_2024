using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TowerCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private string _prefix = "Warriors will awake in ";

    public void UpdateTime(float secondsLeft)
    {
        if (secondsLeft <= 0) {
            gameObject.SetActive(false);
            return;
        }

        var minutes = Mathf.Floor(secondsLeft / 60);
        secondsLeft %= 60;
        var secondsString = secondsLeft < 1 ? Mathf.CeilToInt(secondsLeft) : Mathf.RoundToInt(secondsLeft);
        _titleText.text = _prefix + (minutes > 0 ? minutes + ":" : "") + secondsString;
    }
}
