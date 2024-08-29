using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInventory : MonoBehaviour
{

    private PlayerController _controller;
    private InventoryItem _currentDroppedItem;
    private int _currentItemIndex = 0;
    private InventoryItem[] _inventoryItems = { null, null, null, null, null, null, null, null, null, null };
    private float _currentItemDist => _currentDroppedItem == null ? Mathf.Infinity : _controller.DistanceTo(_currentDroppedItem.transform.position);
    private InventoryItem _currentItem => _inventoryItems[_currentItemIndex];

    private void Start() {
        _controller = GetComponent<PlayerController>();
        EquipItem(0);
    }

    private void Update() {
        ChangeSelectedItem();
        if (InputController.GetDown(Control.INTERACT) && _currentDroppedItem) PickupDroppedItem();
        else InteractWithCurrentItem();
    }

    public void SetClosestItem(InventoryItem item) {
        if (_currentDroppedItem == null || _currentItemDist < _controller.DistanceTo(item.transform.position)) _currentDroppedItem = item;
    }

    public void ClearItem(InventoryItem item) {
        if (_currentDroppedItem == item) _currentDroppedItem = null;
    }

    private void ChangeSelectedItem() {
        var scrollDir = Input.mouseScrollDelta.y;
        if (InputController.GetDown(Control.NEXT_ITEM) && scrollDir > 0) EquipNextItem();
        if (InputController.GetDown(Control.LAST_ITEM) && scrollDir < 0) EquipPreviousItem();
    }

    private void InteractWithCurrentItem() {
        if (!_currentItem) return;

        if (InputController.GetDown(Control.DROP)) {
            _currentItem.Drop();
            _inventoryItems[_currentItemIndex] = null;
            UIManager.i.RemoveInventoryImage(_currentItemIndex);
        }
        else if (InputController.GetDown(Control.USE_PRIMARY)) _currentItem.LeftClick();
        else if (InputController.GetDown(Control.USE_SECONDARY)) _currentItem.RightClick();
    }

    private void PickupDroppedItem() {
        _currentDroppedItem.Pickup(transform);
        _inventoryItems[_currentItemIndex]?.Drop();
        _inventoryItems[_currentItemIndex] = _currentDroppedItem;
        UIManager.i.SetInventoryImage(_currentDroppedItem.GetSprite(), _currentItemIndex);
        EquipItem(_currentItemIndex);
        _currentDroppedItem = null;
    }

    private void EquipItem(int index) {
        if (_currentItemIndex != index) _inventoryItems[_currentItemIndex]?.Unequip();
        _currentItemIndex = index;
        _inventoryItems[_currentItemIndex]?.Equip();
        UIManager.i.SelectInventoryImage(index);
    }

    private void EquipNextItem() {
        var nextIndex = _currentItemIndex + 1;
        if (nextIndex >= _inventoryItems.Length) nextIndex = 0;
        EquipItem(nextIndex);
    }

    private void EquipPreviousItem() {
        var previousIndex = _currentItemIndex - 1;
        if (previousIndex < 0) previousIndex = _inventoryItems.Length - 1;
        EquipItem(previousIndex);
    }
}
