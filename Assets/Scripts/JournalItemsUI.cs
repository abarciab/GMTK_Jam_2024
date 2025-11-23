using System.Collections.Generic;
using UnityEngine;

public class JournalItemsUI : MonoBehaviour
{
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private Transform _gridParent;
    [SerializeField] private JournalInspectedItemDisplay _selectedItemDisplay;
    [SerializeField] private SingleButtonSelector _buttonSelector;

    private readonly List<Transform> _spawnedItems = new List<Transform>();

    public void Initialize(List<JournalItem> items)
    {
        gameObject.SetActive(true);
        BuildList(items);
    }

    private void BuildList(List<JournalItem> items)
    {
        _selectedItemDisplay.gameObject.SetActive(false);

        foreach (var item in _spawnedItems) if (item) Destroy(item.gameObject);
        _spawnedItems.Clear();

        foreach (var i in items) {
            var obj = Instantiate(_itemPrefab, _gridParent);
            var ui = obj.GetComponent<JournalInspectedItemUI>();
            ui.Initialize(i, _selectedItemDisplay);
            _spawnedItems.Add(obj.transform);
        }

        _buttonSelector.Initialize(true);

    }
}
