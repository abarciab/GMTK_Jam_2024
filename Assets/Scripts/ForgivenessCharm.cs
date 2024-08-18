using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.Searcher;
using UnityEngine;

public class ForgivenessCharm : InventoryItem
{
    [SerializeField] private GameObject indicator;
    private float saveTimer = 0.1f;
    private List<Vector3> positions = new List<Vector3>();
    private Transform indicatorTransform;
    private ParticleSystem indicatorParticleSystem;

    void Start()
    {
        indicatorTransform = Instantiate(indicator).transform;
        indicatorParticleSystem = indicatorTransform.GetComponent<ParticleSystem>();
        indicatorParticleSystem.Stop();
    }

    public override void LeftClick()
    {
        GameManager.i.Player.transform.position = positions[0];

        base.LeftClick();
    }

    private void Update()
    {
        if((itemState == ItemState.Equipped || itemState == ItemState.Inventory) && saveTimer <= 0f)
        {
            positions.Add(GameManager.i.Player.transform.position);
            saveTimer = 0.1f;

            if(positions.Count * saveTimer > 4f)
            {
                positions.RemoveAt(0);
                indicatorTransform.position = positions[0] + Vector3.up;
            }
        }
        else
        {
            saveTimer -= Time.deltaTime;
        }
    }

    public override void Equip()
    {
        indicatorParticleSystem.Play();
        base.Equip();
    }

    public override void Unequip()
    {
        indicatorParticleSystem.Stop();
        base.Equip();
    }
}
