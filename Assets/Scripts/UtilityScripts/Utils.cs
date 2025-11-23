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

    public static float XZDistance(Vector3 pos1, Vector3 pos2)
    {
        var pos1XZ = new Vector2(pos1.x, pos1.z);
        var pos2XZ = new Vector2(pos2.x, pos2.z);
        return Vector2.Distance(pos1XZ, pos2XZ);
    }
}
