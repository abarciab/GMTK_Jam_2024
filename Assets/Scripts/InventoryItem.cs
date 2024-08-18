using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private Sprite inventorySprite;
    [SerializeField] private string description;
    [SerializeField] private string leftClickText;
    [SerializeField] private string rightClickText;
    [SerializeField] protected Sound useSound;
    [SerializeField] protected Sound errorSound;

    protected enum ItemState { Inventory, Equipped, Dropped }
    protected ItemState itemState = ItemState.Dropped;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        useSound = Instantiate(useSound);
        errorSound = Instantiate(errorSound);
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        switch (itemState)
        {
            case ItemState.Inventory:
                break;
            case ItemState.Equipped:
                break;
            case ItemState.Dropped:
                break;
        }
    }

    public virtual void LeftClick()
    {
        print(gameObject.name + " - Left Click");
    }

    public virtual void RightClick()
    {
        print(gameObject.name + " - Right Click");
    }

    public void Pickup(Transform inventoryParent)
    {
        print(gameObject.name + " - Pickup");
        transform.parent = inventoryParent;

        itemState = ItemState.Inventory;
    }

    public virtual void Equip()
    {
        print(gameObject.name + " - Equipped");

        meshRenderer.enabled = true;

        itemState = ItemState.Equipped;
    }
    public virtual void Unequip()
    {
        print(gameObject.name + " - Unequipped");

        meshRenderer.enabled = false;

        itemState = ItemState.Inventory;
    }

    public void Drop()
    {
        print(gameObject.name + " - Dropped");

        if(itemState == ItemState.Equipped) Unequip();

        transform.parent = null;

        meshRenderer.enabled = true;

        itemState = ItemState.Dropped;
    }

    public Sprite GetSprite()
    {
        return inventorySprite;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        player?.SetClosestItem(this);
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        player?.ClearItem(this);
    }
}
