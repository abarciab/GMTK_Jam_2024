using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private TextMeshProUGUI _speakerName;
    [SerializeField] private GameObject _speakerNameParent;
    [SerializeField] private GameObject _playerNameParent;
    [SerializeField] private Animator _instructions;
    [SerializeField] private float _instructionTime = 3;
    [SerializeField] private float _letterTime = 0.05f;

    [SerializeField] private Transform _choiceParent;
    [SerializeField] private GameObject _choicePrefab;

    private List<DialogueChoice> _spawnedChoices = new List<DialogueChoice>();
    private List<LineData> _lines = new List<LineData>();
    private string _textLeft;
    private string _lastSpeaker;
    private int _currentIndex;
    private float _finishTime;
    private bool _displayingChoice;
    private bool _displayingRandom;
    private ConversationData _actualConversation;

    public bool Talking => gameObject.activeInHierarchy;

    private void OnEnable()
    {
        _choiceParent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_displayingChoice) {

            var selected = -1;
            if (Input.GetKeyDown(KeyCode.Alpha1)) selected = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2)) selected = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3)) selected = 2;
            if (Input.GetKeyDown(KeyCode.Alpha4)) selected = 3;

            if (selected != -1 && selected < _spawnedChoices.Count) _spawnedChoices[selected].Select(); 

            return;
        }

        if (_lines.Count > 0 && InputController.GetDown(Control.INTERACT)) Next();

        if (Time.time - _finishTime > _instructionTime) {
            _finishTime = Mathf.Infinity;
            _instructions.gameObject.SetActive(true);
        }
    }

    public void SelectChoice(LineData selected)
    {
        for (int i = 0; i < _lines.Count; i++) {
            if (_lines[i] == selected) _currentIndex = i;
        }

        _choiceParent.gameObject.SetActive(false);
        _mainText.gameObject.SetActive(true);
        _displayingChoice = false;

        CompleteCurrentLine();
    }

    public void StartDialogue(List<LineData> lines)
    {
        _actualConversation = new ConversationData();
        GameManager.i.OpenMenu(true);
        _finishTime = Mathf.Infinity;
        _lastSpeaker = "";
        gameObject.SetActive(true);
        _lines = new List<LineData>(lines);
        _currentIndex = -1;
        StartNextLine();
    }

    public void StartDialogue(List<string> stringLines)
    {
        _displayingChoice = false;
        _choiceParent.gameObject.SetActive(false);
        _mainText.gameObject.SetActive(true);

        var lines = new List<LineData>();
        foreach (var stringLine in stringLines)
        {
            lines.Add(new LineData(stringLine));
        }
        StartDialogue(lines);
    }

    public void Next()
    {
        if (_textLeft.Length > 0) SkipRemainingAnimation();
        else if (_currentIndex < _lines.Count-1) StartNextLine();
        else FinishDialogue();
    }

    private void FinishDialogue()
    {
        gameObject.SetActive(false); 
        GameManager.i.CloseMenu();

        UIManager.i.Journal.AddConversation(_actualConversation);
    }

    private void SkipRemainingAnimation()
    {
        StopAllCoroutines();
        _mainText.text += _textLeft;
        _textLeft = "";

        CompleteCurrentLine();
    }

    private void CompleteCurrentLine()
    {
        if (_currentIndex >= _lines.Count) return;


        var current = _lines[_currentIndex];

        var line = new LineData();
        line.Text =current.Text;
        _actualConversation.Lines.Add(line);

        foreach (var effect in current.Effects) {
            if (StringUtils.Format(effect) == "CEND") _currentIndex = _lines.Count;
            if (StringUtils.Format(effect).Contains("GOTO")) GoTo(effect);
            else ConditionManager.i.DoEffect(effect);
        }

        if (_displayingRandom) {
            while (_currentIndex < _lines.Count && _lines[_currentIndex].Random) _currentIndex += 1;
            _currentIndex -= 1;
            _displayingRandom = false;
        }
    }

    private void GoTo(string effectString)
    {
        effectString = StringUtils.Format(effectString);
        var dest = effectString.Split('|')[1];

        for (int i = 0; i < _lines.Count; i++) {
            if (StringUtils.Format(_lines[i].Label) == dest) _currentIndex = i;
        }
        Next();
    }

    private void StartNextLine()
    {
        _currentIndex += 1;
        _mainText.text = "";
        _textLeft = "";

        var nextLine = _lines[_currentIndex];
        if (nextLine == null || !nextLine.Available()) {
            Next();
            return;
        }

        if (nextLine.Text.Length == 0) {
            CompleteCurrentLine();
            StartNextLine();
            return;
        }

        if (nextLine.Choice) {
            DisplayChoices();
            return;
        }

        if (nextLine.Random && !_displayingRandom) {
            PickRandom();
            return;
        }

        var speaker = _lastSpeaker;
        if (!string.IsNullOrWhiteSpace(nextLine.Speaker)) {
            if (nextLine.Speaker.ToUpper().Trim() == "PLAYER") speaker = "PLAYER";
            else speaker = nextLine.Speaker;
        }
        _lastSpeaker = speaker;

        if (!string.IsNullOrWhiteSpace(speaker)) {
            var isPlayer = speaker == "PLAYER";
            _speakerNameParent.SetActive(!isPlayer);
            _speakerName.text = speaker;
            _playerNameParent.SetActive(isPlayer);

            if (isPlayer) _mainText.alignment = TextAlignmentOptions.TopRight;
            else _mainText.alignment = TextAlignmentOptions.TopLeft;
        }
        else {
            _speakerNameParent.SetActive(false);
            _playerNameParent.SetActive(false);
        }

        if (_instructions.gameObject.activeInHierarchy) _instructions.SetTrigger("Exit");
        _finishTime = Mathf.Infinity;

        StartCoroutine(AnimateText(nextLine.Text));
    }

    private void PickRandom()
    {
        var randomOptions = new List<LineData>();
        var index = _currentIndex;
        while (_lines[index].Random) {
            randomOptions.Add(_lines[index]);
            index++;
            if (index == _lines.Count) break;
        }

        var selected = randomOptions[Random.Range(0, randomOptions.Count)];

        for (int i = 0; i < _lines.Count; i++) {
            if (_lines[i] == selected) _currentIndex = i-1;
        }

        _displayingRandom = true;

        Next();
    }

    private void DisplayChoices()
    {
        _mainText.gameObject.SetActive(false);

        _displayingChoice = true;

        var choiceLines = new List<LineData>();
        var index = _currentIndex;
        while (_lines[index].Choice) {
            choiceLines.Add(_lines[index]);
            index++;
            if (index == _lines.Count) break;
        }

        _choiceParent.gameObject.SetActive(true);
        for (int i = _choiceParent.childCount-1; i >= 0 ; i--) {
            Destroy(_choiceParent.GetChild(i).gameObject);
        }
        _spawnedChoices.Clear();

        foreach (var c in choiceLines) {
            spawnChoice(c);
        }
    }

    private void spawnChoice(LineData choice)
    {
        var newChild = Instantiate(_choicePrefab, _choiceParent).GetComponent<DialogueChoice>();
        newChild.Initialize(choice, this);
        _spawnedChoices.Add(newChild);
    }

    private IEnumerator AnimateText(string line)
    {
        _textLeft = line;
        while (_textLeft.Length > 0) {
            _mainText.text += _textLeft[0];
            _textLeft = _textLeft.Substring(1);
            yield return new WaitForSeconds(_letterTime);
        }
        _finishTime = Time.time;

        CompleteCurrentLine();
    }

}
