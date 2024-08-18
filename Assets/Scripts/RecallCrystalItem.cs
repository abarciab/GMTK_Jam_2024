using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallCrystalItem : InventoryItem
{
    [SerializeField] private GameObject indicatorParticlesPrefab;
    private ParticleSystem indicatorParticleSystem;
    private Vector3 recallPosition = Vector3.zero;

    void Start()
    {
        indicatorParticleSystem = Instantiate(indicatorParticlesPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        indicatorParticleSystem.Stop();
    }

    public override void LeftClick()
    {
        if(recallPosition != Vector3.zero)
        {
            print("Recalling to: " + recallPosition);
            GameManager.i.Player.transform.position = recallPosition;
            recallPosition = Vector3.zero;

            indicatorParticleSystem.Stop();

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

        indicatorParticleSystem.transform.position = recallPosition;
        indicatorParticleSystem.Play();

        useSound.Play();

        base.RightClick();
    }

    public override void Equip()
    {
        if(recallPosition != Vector3.zero) indicatorParticleSystem.Play();
        base.Equip();
    }

    public override void Unequip()
    {
        indicatorParticleSystem.Stop();
        base.Equip();
    }
}
