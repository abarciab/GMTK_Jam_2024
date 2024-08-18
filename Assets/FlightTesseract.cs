using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightTesseract : InventoryItem
{
    public override void LeftClick()
    {
        GameManager.i.Player.GetComponent<Rigidbody>().AddForce(Vector3.up * 100f, ForceMode.Impulse);
        useSound.Play();

        base.LeftClick();
    }

    public override void RightClick()
    {
        GameManager.i.Player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        errorSound.Play();

        base.RightClick();
    }
}
