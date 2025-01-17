using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class Conversation { [TextArea(2, 10)] public List<string> Lines = new List<string>(); }

public class TutorialMentor : MonoBehaviour
{
    [SerializeField] private float _activationDistance;
    [SerializeField] private List<Conversation> _conversations = new List<Conversation>();
    private Transform _player;

    private void Update()
    {
        if (!_player) _player = GameManager.i.Player.transform;

        if (UIManager.i._dialogue.gameObject.activeInHierarchy) return;

        var dist = Vector3.Distance(transform.position, _player.position);
        UIManager.i.SetInteractPromptState(dist < _activationDistance, gameObject, "Listen");

        if (dist > _activationDistance) return;
        else if (InputController.GetDown(Control.INTERACT)){
            UIManager.i._dialogue.StartDialogue(_conversations[0].Lines);
            _conversations.RemoveAt(0);
            if (_conversations.Count == 0) gameObject.SetActive(false);
            UIManager.i.SetInteractPromptState(false, gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _activationDistance);
    }

}
