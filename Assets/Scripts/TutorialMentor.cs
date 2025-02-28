using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TutorialMentor : MonoBehaviour
{
    [SerializeField] private float _activationDistance;
    [SerializeField] private TextAsset _dialogueFile;
    [SerializeField] private int _lineID;

    private List<string> _lines = new List<string>();
    private Transform _player;

    private void Start()
    {
        _player = GameManager.i.Player.transform;

        var textFileLines = _dialogueFile.text.Split('\n').ToList();
        textFileLines = textFileLines.Where(x => x.Length > 0).ToList();    

        int currentIndex = -1;
        for (int i = 0; i < textFileLines.Count; i++) {
            var line = textFileLines[i];
            if (line.Contains(":")) currentIndex = int.Parse(line.Split(":")[0].Trim());
            else if (currentIndex == _lineID && line.Length > 1) _lines.Add(line);
        }
    }

    private void Update()
    {
        if (UIManager.i._dialogue.gameObject.activeInHierarchy) return;

        var dist = Vector3.Distance(transform.position, _player.position);
        UIManager.i.SetInteractPromptState(dist < _activationDistance, gameObject, "Read");

        if (dist < _activationDistance && InputController.GetDown(Control.INTERACT)) Activate();        
    }

    private void Activate()
    {
        UIManager.i._dialogue.StartDialogue(_lines);
        UIManager.i.SetInteractPromptState(false, gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _activationDistance);
    }
}
