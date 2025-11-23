using MyBox;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class GuardianMessageData
{
    [TextArea(2, 10)] public List<string> Message;
}

public class GuardianMessage : MonoBehaviour
{
    [SerializeField] private Sound _drumSound;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private List<GuardianMessageData> _messages = new();
    [SerializeField] private float _letterDisplayTime = 0.05f;
    [SerializeField] private GuardianMessagesRoom _roomController;
    [SerializeField] private TriggerOnStart _startRoomTrigger;

    private float _letterDisplayCooldown = 0f;
    private int _currentMessageIndex = 0;
    private int _currentLineIndex = 0;
    private int _currentLetterIndex = 0;
    private string _currentLine = "";
    private bool _doneAnimating = false;

    private void OnEnable()
    {
        _roomController.gameObject.SetActive(true);
        FindFirstObjectByType<GameManager>().OpenMenu();
        FindFirstObjectByType<CameraController>().SetCinematic(true);
        ShowNextMessage();
    }

    public void ShowNextMessage()
    {
        _currentMessageIndex = 0; 
        _currentLineIndex = -1;
        ShowNextLine();
    }

    private void SkipAnimation()
    {
        _doneAnimating = true;
        _text.text = _currentLine;
        _currentLetterIndex = _currentLine.Length;
    }

    private void ShowNextLine()
    {
        if (!_drumSound.Instantialized) _drumSound = Instantiate(_drumSound);
        _drumSound.Play();
        _roomController.Activate();

        _currentLineIndex += 1;

        if (_currentLineIndex >= _messages[_currentMessageIndex].Message.Count) {
            _messages.RemoveAt(_currentMessageIndex);
            gameObject.SetActive(false);
            _roomController.gameObject.SetActive(false);

            print("Done with guardian message");
            FindFirstObjectByType<GameManager>().CloseMenu();
            var cam = FindFirstObjectByType<CameraController>();
            cam.SetCinematic(false);
            cam.transform.position = FindFirstObjectByType<PlayerController>().transform.position;
            _startRoomTrigger.ManualTrigger();

            return;
        }

        _currentLetterIndex = 0;
        _currentLine = _messages[_currentMessageIndex].Message[_currentLineIndex];
        _doneAnimating = false;
        _text.text = "";
    }

    private void Update()
    {
        _letterDisplayCooldown -= Time.deltaTime;
        if (!_doneAnimating && _letterDisplayCooldown <= 0f && _currentLetterIndex < _currentLine.Length) {
            _currentLetterIndex++;
            _text.text = _currentLine[.._currentLetterIndex];
            _letterDisplayCooldown = _letterDisplayTime;

            if (_currentLetterIndex == _currentLine.Length) {
                _doneAnimating = true;
            }
        }

        if (InputController.GetDown(Control.INTERACT)) {
            if (_doneAnimating) ShowNextLine();
            else SkipAnimation();
        }
    }
}
