using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public string Name;
    [Range(0, 3)] public int Index;

    [SerializeField] private Sound _completeSound;

    [HideInInspector] public float MaxHeight;
    [HideInInspector] public bool Complete { get; private set; }

    private void Start()
    {
        _completeSound = Instantiate(_completeSound);
        GameManager.i.Towers.Add(this);
    }

    public float CheckProgress(float y)
    {
        if (Complete) return 1;
        var progress = Mathf.InverseLerp(transform.position.y, MaxHeight * transform.localScale.x, y);
        if (progress > 0.98) CompleteTower();
        return progress;
    }

    private void CompleteTower()
    {
        Complete = true;
        _completeSound.Play();
        GameManager.i.CompleteTower();
    }
}
