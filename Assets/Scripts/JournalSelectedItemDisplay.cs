using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public enum ScrollDir {FORWARD, BACKWARD };

public class JournalSelectedItemDisplay : MonoBehaviour
{

    [SerializeField] private GameObject _pagePrefab;
    [SerializeField] private Transform _pageParent;
    [SerializeField] private int _characterWidthLimit = 53;
    [SerializeField] private int _pageLineCount = 32;
    [SerializeField] private float _maxRotation = 8;

    private List<JournalPageController> _spawnedPages = new List<JournalPageController>();
    private float _lastSign = 1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) ScrollPages(ScrollDir.FORWARD);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ScrollPages(ScrollDir.BACKWARD);
    }

    private void ScrollPages(ScrollDir dir)
    {
        if (_spawnedPages.Count < 2) return;

        var bottomPage = _pageParent.GetChild(0).GetComponent<JournalPageController>();
        var topPage = _pageParent.GetChild(_pageParent.transform.childCount - 1).GetComponent<JournalPageController>();

        if (dir == ScrollDir.FORWARD) {
            topPage.Cycle(JournalPageController.CycleTarget.TO_BOTTOM);
        }
        if (dir == ScrollDir.BACKWARD) {
            bottomPage.Cycle(JournalPageController.CycleTarget.TO_TOP);
        }
    }

    private List<string> SplitIntoPages(string content)
    {
        var pages = new List<string>() { "" };
        int chars = 0;
        int lines = 0;
        string currentLine = "";
        for (int i = 0; i < content.Length; i++) {
            chars += 1;
            var letter = content[i];
            currentLine += content[i];
            if (letter != '\n' && chars < _characterWidthLimit) continue;
                
            pages[^1] += currentLine;
            chars = 0;
            currentLine = "";
            lines += 1;
            if (lines <= _pageLineCount) continue;
            
            lines = 0;
            pages.Add("");
        }

        return pages;
    }

    public void DisplayItem(JournalItem item)
    {
        foreach (var p in _spawnedPages) Destroy(p.gameObject);
        _spawnedPages.Clear();

        var pages = SplitIntoPages(item.Content);
        for (int i = 0; i < pages.Count; i++) {
            SpawnPage(pages[i], i);
        }
    }

    private void SpawnPage(string content, int pageNum)
    {
        var angleInput = Mathf.Sqrt(_maxRotation);
        var newPage = Instantiate(_pagePrefab, _pageParent).GetComponent<JournalPageController>();
        newPage.Initialize(content, pageNum);

        var zRot = Mathf.Pow(Random.Range(0, angleInput), 2);
        if (Random.Range(0, 1f) < 0.75f) zRot *= _lastSign * -1;
        _lastSign = Mathf.Sign(zRot);

        newPage.transform.localEulerAngles += Vector3.forward * zRot;
        newPage.transform.SetAsFirstSibling();

        _spawnedPages.Add(newPage);

    }
}
