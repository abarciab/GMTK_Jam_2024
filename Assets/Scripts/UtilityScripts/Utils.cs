using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static List<T> EnumToList<T>()
    {
        var array = System.Enum.GetValues(typeof(T));
        var list = new List<T>();
        foreach (var item in array) list.Add((T)item);
        return list;
    }

    public static void SetDirty(Object obj)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(obj);
#endif
    }

}
