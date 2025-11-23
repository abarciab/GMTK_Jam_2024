using MyBox;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int NumAttempts;
}   

public class SaveManager : MonoBehaviour
{
    public static SaveManager i;

    [SerializeField] private JournalController _journal;
    
    private SaveData _currentData = new SaveData();
    private Dictionary<string, string> _loadedKeys = new Dictionary<string, string>();

    private const string _seperator = "|=|";
    private const string _filePath = "/saves/savegame.txt";

    private const string _numAttempsKey = "NumAttempts";

    public SaveData Data => _currentData;

    private void Awake()
    {
        i = this;

        LoadData();
        BuildSaveData();

        //print("loaded numAttempts: " + _currentData.NumAttempts);

        _currentData.NumAttempts += 1;

    }

    [ButtonMethod]
    private void ResetSave()
    {
        _currentData = new SaveData();
        SaveGame();
        print("reset game save");
    }

    private string LoadKey(string key, string defaultValue = "")
    {
        if (_loadedKeys.ContainsKey(key)) return _loadedKeys[key];
        //print("didn't find key: " + key + " in loaded data");
        return defaultValue;
    }

    private void SaveKey(string key, string value)
    {
        if (_loadedKeys.ContainsKey(key)) _loadedKeys[key] = value;
        else _loadedKeys.Add(key, value);
    }

    private void LoadData()
    {
        //print("loading game");

        var path = Path.Combine(Application.persistentDataPath, "/saves/savegame.txt");
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        if (!File.Exists(path)) {
            File.WriteAllText(path, "");
        }
        
        var loadedString = File.ReadAllText(path);
        var lines = loadedString.Split('\n');

        _loadedKeys = new Dictionary<string, string>();
        foreach (var l in lines) {
            var parts = l.Split(_seperator);
            if (parts.Length == 2) _loadedKeys[parts[0]] = parts[1];
        }
    }

    private void BuildSaveData()
    {
        _currentData = new();
        _currentData.NumAttempts = int.Parse(LoadKey(_numAttempsKey, "0"));
    }

    public void SaveGame()
    {
        print("saving game");

        //toSave:
        // dialogue conditions
        // wins

        SaveKey(_numAttempsKey, _currentData.NumAttempts.ToString());
        SaveJournalInfo();

        var saveString = "";
        foreach (var kvp in _loadedKeys) {
            saveString += kvp.Key + _seperator + kvp.Value + "\n";
        }
        File.WriteAllText(Path.Combine(Application.persistentDataPath, _filePath), saveString);
    }

    private void SaveJournalInfo()
    {
        // journal items
        // journal conversations
        // journal quests
    }
}
