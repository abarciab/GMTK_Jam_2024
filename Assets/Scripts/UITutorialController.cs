using System.Collections.Generic;
using UnityEngine;

public class UITutorialController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _sections;
    [SerializeField] private bool _skip;

    private void OnEnable()
    {
        for (int i = 0; i < _sections.Count; i++) {
            _sections[i].SetActive(i == 0);
        }

        if (_skip) Complete();
    }

    private void Complete()
    {
        gameObject.SetActive(false);
        UIManager.i.CompleteUITutorial();
    }
}
