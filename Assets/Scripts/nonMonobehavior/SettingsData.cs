using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class Settings 
{
    public static float MouseSensetivity { get; private set; }
    private static string sensKey = "MouseSensetivity";

    public static void SetSensetivity(float value)
    {
        MouseSensetivity = value;
        PlayerPrefs.SetFloat(sensKey, value);
    }

    public static void Initialize()
    {
        if (Time.time > 1) return;

        //Debug.Log("initialzing");
        MouseSensetivity = PlayerPrefs.GetFloat(sensKey, 0.05f);
    }
}
