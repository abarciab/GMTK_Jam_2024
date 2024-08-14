using UnityEngine;

[System.Serializable]
public class MappedKeyData 
{
    public string DisplayName;
    public Control Control;
    public KeyCode Key;

    public MappedKeyData(Control control = Control.NONE) 
    {
        Control = control;
        Key = KeyCode.None;
    }
}
