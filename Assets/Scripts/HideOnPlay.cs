using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    private void Awake()
    {
        if (!GameManager.i) gameObject.SetActive(false);
    }
}
