using TMPro;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class JournalPageController : MonoBehaviour
{

    public enum CycleTarget {NONE, TO_BOTTOM, TO_TOP}

    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private TextMeshProUGUI _pageNum;
    [SerializeField] private Animator _animator;

    private string _content;
    private CycleTarget _cycleTarget;

    public void Initialize(string content, int page)
    {
        _mainText.text = content;
        _content = content;
        _pageNum.text = (page + 1).ToString();
    }

    public void Cycle(CycleTarget target)
    {
        _cycleTarget = target;
        _animator.SetTrigger("Cycle");
    }

    public void ChangleSiblingOrder()
    {
        if (_cycleTarget == CycleTarget.TO_TOP) transform.SetAsLastSibling();
        else if (_cycleTarget == CycleTarget.TO_BOTTOM) transform.SetAsFirstSibling();

        _cycleTarget = CycleTarget.NONE;
    }
}
