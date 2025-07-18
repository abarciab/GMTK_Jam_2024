using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField] private List<string> lines = new List<string>();

    [ButtonMethod]
    private void startConversation()
    {
        UIManager.i.Dialogue.StartDialogue(lines);
    }
}
