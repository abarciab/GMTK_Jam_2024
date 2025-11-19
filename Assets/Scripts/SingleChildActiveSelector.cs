using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SingleChildActiveSelector : MonoBehaviour
{
    [SerializeField] private UnityEvent OnSelect;

    public void Select(int index)
    {
        foreach (Transform child in transform) {
            if (child.GetSiblingIndex() != index) {
                if (child.gameObject.activeInHierarchy) {
                    var animator = child.GetComponent<Animator>();
                    if (animator) animator.SetTrigger("Exit");
                }
            }
            else {
                child.gameObject.SetActive(true);
            }
        }

        OnSelect.Invoke();
    }
}
