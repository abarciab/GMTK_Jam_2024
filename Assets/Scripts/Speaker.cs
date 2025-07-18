using MyBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LineData
{
    [HideInInspector] public string Name;
    public string Speaker = "";
    public string Text = "";
    public string Label = "";
    public List<string> Conditions = new List<string>();
    public List<string> Effects = new List<string>();
    public bool Choice;
    public bool Random;

    private const string choiceTag = "[C]";
    private const string randomTag = "[R]";

    public bool Available()
    {
        foreach (var condition in Conditions) if (!ConditionManager.i.Check(condition)) return false;
        return true;
    }

    public LineData() { }

    public LineData(string rawData)
    {
        Name = rawData;

        Speaker = "";

        var parts = rawData.Split(':');
        if (parts.Length > 1) {
            var metaData = parts[0].ToUpper().Trim();

            if (metaData.Contains(choiceTag)) {
                Choice = true;
                metaData.Replace(choiceTag, "");
            }
            if (metaData.Contains(randomTag)) {
                Random = true;
                metaData.Replace(randomTag, "");
            }

            var tags = StringUtils.StripStringTags(ref metaData);
            Conditions = StringUtils.GetConditionsFromTags(tags);

            Speaker = metaData;
        }

        var main = parts[^1];
        
        var effectTags = StringUtils.StripStringTags(ref main);
        Effects = StringUtils.GetEffectsFromTags(effectTags);
        Label = StringUtils.GetLabelFromTags(effectTags);

        Text = main.Trim();
    }
}

[System.Serializable]
public class ConversationData
{
    [HideInInspector] public string Name;
    public List<string> Conditions = new List<string>();
    public List<LineData> Lines = new List<LineData>();

    [SerializeField] private int _numTimes = -1;

    public bool Available()
    {
        foreach (var condition in Conditions) if (!ConditionManager.i.Check(condition)) return false;
        return _numTimes > 0 || _numTimes == -1;
    }

    public void Use()
    {
        if (_numTimes > 0) _numTimes -= 1;
    }

    public ConversationData() {}

    public ConversationData(string rawData)
    {
        Lines = new List<LineData>();
        var lines = rawData.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        if (lines[0].Contains("{")) {
            ParseMetaData(lines[0]);
            lines.RemoveAt(0);
        }

        foreach (var line in lines) Lines.Add(new LineData(line));
        //Lines = Lines.Where(x => x.Text.Length > 0 || x.Label.Length > 0).ToList();

        if (Lines.Count > 0) Name += " || '" + Lines[0].Text + "'";
    }

    private void ParseMetaData(string metaData)
    {
        var tags = StringUtils.GetStringTags(metaData);
        Conditions = StringUtils.GetConditionsFromTags(tags);

        var ints = StringUtils.GetIntTags(metaData);
        if (ints.Count > 0) _numTimes = ints[0];

        Name = "[" + _numTimes + "] | " + string.Join(',', Conditions);
    }
}

public class Speaker : MonoBehaviour
{
    [SerializeField] private TextAsset _textFile;
    [SerializeField] private List<ConversationData> _conversations = new List<ConversationData>();
    [SerializeField] private float _speakDist;

    private Transform _player;

    private void Start()
    {
        ParseTextFile();
        _player = GameManager.i.Player.transform;
    }

    private void Update()
    {
        if (!HasConversationAvailable() || UIManager.i.Dialogue.Talking) return;

        var dist = Vector3.Distance(transform.position, _player.position);
        var ReadyToTalk = dist <= _speakDist && LookedAtByPlayer();

        UIManager.i.SetInteractPromptState(ReadyToTalk, gameObject, "Speak");

        if (ReadyToTalk && InputController.GetDown(Control.INTERACT)) StartConversation();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _speakDist);
    }

    private bool LookedAtByPlayer()
    {
        var lookRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        var didHit = Physics.Raycast(lookRay, out var hitInfo);
        if (!didHit) return false;

        return hitInfo.collider.GetComponentInParent<Speaker>() == this;
    }

    private void StartConversation()
    {
        var selected = SelectNextConversation();
        UIManager.i.Dialogue.StartDialogue(selected.Lines);
    }

    private bool HasConversationAvailable()
    {
        foreach (var conversation in _conversations) if (conversation.Available()) return true;
        return false;
    }

    private ConversationData SelectNextConversation()
    {
        for (int i = 0; i < _conversations.Count; i++) {
            if (_conversations[i].Available()) {
                _conversations[i].Use();
                return _conversations[i];
            }
        }
        return null;
    }

    [ButtonMethod]
    private void ParseTextFile()
    {
        _conversations.Clear();
        var text = _textFile.text;
        var conversations = text.Split("[cStart]").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        foreach (var conversationString in conversations)
        {
            _conversations.Add(new ConversationData(conversationString));
        }

        _conversations = _conversations.Where(x => x.Lines.Count > 0).ToList();

        if (!Application.isPlaying) Utils.SetDirty(this);
    }
}
