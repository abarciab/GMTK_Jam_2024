using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallCrystalItem : InventoryItem
{
    private Vector3 recallPosition = Vector3.zero;
    public override void LeftClick()
    {
        if(recallPosition != Vector3.zero)
        {
            print("Recalling to: " + recallPosition);
            GameManager.i.Player.transform.position = recallPosition;
            recallPosition = Vector3.zero;

            useSound.Play();
        }
        else
        {
            print("Recall position not set!");
            errorSound.Play();
        }

        base.LeftClick();
    }

    public override void RightClick()
    {
        print("Recall Position Set: " + transform.position);
        recallPosition = transform.position;

        useSound.Play();

        base.RightClick();
    }
}
