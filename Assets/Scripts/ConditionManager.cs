using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConditionManager : MonoBehaviour
{
    public static ConditionManager i;

    [SerializeField] private List<string> _conditions = new List<string>();

    private string Format(string condition) => condition.ToUpper().Trim();

    private void Awake()
    {
        i = this;

        for (int i = 0; i < _conditions.Count; i++) {
            _conditions[i] = Format(_conditions[i] );
        }
    }

    public void DoEffect(string effect)
    {
        effect = Format(effect);
        var parts = effect.Split('|');
        if (parts[0] == "SET") Add(parts[1]);
    }

    public bool Check(string condition)
    {
        bool negate = false;
        if (condition[0] == '!') {
            condition = condition.Substring(1);
            negate = true;
        }
        var matched = _conditions.Where(x => x.Equals(Format(condition))).ToList();
        var found = matched.Count > 0;

        return negate ? !found : found;
    }

    public void Add(string condition)
    {
        if (Check(condition)) return;
        _conditions.Add(Format(condition));
    }

    public void Remove(string condition)
    {
        if (!Check(condition)) return;
        _conditions.Remove(Format(condition));
    }
}
