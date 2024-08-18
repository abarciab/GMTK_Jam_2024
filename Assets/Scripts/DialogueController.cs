using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private float _letterTime = 0.05f;

    private string _textLeft;
    private List<string> _lines;
    private int _currentIndex;

    private void Update()
    {
        if (_lines.Count > 0 && InputController.GetDown(Control.INTERACT)) Next();
    }

    public void Next()
    {
        if (_textLeft.Length > 0) FinishCurrentText();
        else if (_currentIndex < _lines.Count-1) StartNextLine();
        else FinishDialogue();
    }

    private void FinishDialogue()
    {
        gameObject.SetActive(false);
    }

    private void FinishCurrentText()
    {
        StopAllCoroutines();
        _mainText.text += _textLeft;
        _textLeft = "";
    }

    private void StartNextLine()
    {
        _currentIndex += 1;
        _mainText.text = "";
        StartCoroutine(AnimateText(_lines[_currentIndex]));
    }

    private IEnumerator AnimateText(string line)
    {
        _textLeft = line;
        while (_textLeft.Length > 0) {
            _mainText.text += _textLeft[0];
            _textLeft = _textLeft.Substring(1);
            yield return new WaitForSeconds(_letterTime);
        }
    }

    public void StartDialogue(List<string> lines)
    {
        gameObject.SetActive(true);
        _lines = new List<string>(lines);
        _currentIndex = -1;
        StartNextLine();
    }
}
