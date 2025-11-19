using System.Collections.Generic;
using UnityEngine;

public class JournalItemsUI : MonoBehaviour
{
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private Transform _gridParent;

    private List<Transform> _spawnedItems = new List<Transform>();

    private void OnEnable()
    {
        BuildList();
    }

    private void BuildList()
    {
        foreach (var item in _spawnedItems) Destroy(item.gameObject);

    }
}
