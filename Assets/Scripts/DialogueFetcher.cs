using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueFetcher : MonoBehaviour
{
    [Serializable]
    private class CharacterRootNodes
    {
        public int None;
        public int Eugene;
        public int Claire;
        public int Mark;
        public int Amanda;
        public int Tilda;
        public int Ed;
    }

    [Serializable]
    private class RootNodeGroup
    {
        public CharacterRootNodes Eugene;
        public CharacterRootNodes Claire;
        public CharacterRootNodes Mark;
        public CharacterRootNodes Amanda;
        public CharacterRootNodes Tilda;
        public CharacterRootNodes Ed;
    }

    [Serializable]
    private class Response
    {
        public string text;
        [field: SerializeField] public int[] reveal_keys;
        [field: SerializeField] public int[] clear_keys;
        [field: SerializeField] public int follow_up_index;
    }

    [Serializable]
    private class DialogueEntry
    {
        public int index;
        public string text;
        [field: SerializeField] public int[] reward_keys;
        public List<Response> responses;
    }

    [Serializable]
    private class DialogueData
    {
        [field: SerializeField] public RootNodeGroup root_nodes;
        public List<DialogueEntry> dialogues;
    }

    // This is the class that other files will interface with to get the dialogue information
    public class Dialogue
    {
        public int dialogueIndex;
        public string text;
        public List<ResponseField> responses;
    }
    public class ResponseField
    {
        public string text;
        public int responseIndex;
    }


    public static DialogueFetcher Instance {get; private set;}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

 
    [SerializeField]
    private TextAsset jsonFile;
    private DialogueData dialogueData;
    private HashSet<int> unlockedKeys;


    private void Start()
    {
       dialogueData = JsonUtility.FromJson<DialogueData>(jsonFile.text);
       unlockedKeys = new HashSet<int>();
    }

    public Dialogue FetchRootDialogue(Ghost.GhostName ghostName, GhostMask.MaskType maskType)
    {
        CharacterRootNodes crn = ghostName switch
        {
            Ghost.GhostName.Amanda => dialogueData.root_nodes.Amanda,
            Ghost.GhostName.Claire => dialogueData.root_nodes.Claire,
            Ghost.GhostName.Ed => dialogueData.root_nodes.Ed,
            Ghost.GhostName.Eugene => dialogueData.root_nodes.Eugene,
            Ghost.GhostName.Mark => dialogueData.root_nodes.Mark,
            Ghost.GhostName.Tilda => dialogueData.root_nodes.Tilda,
            _ => null
        };
        int dialogueIndex = maskType switch
        {
            GhostMask.MaskType.Deer => crn.Amanda,
            GhostMask.MaskType.Horns => crn.Claire,
            GhostMask.MaskType.Standard => crn.Ed,
            GhostMask.MaskType.Fish => crn.Eugene,
            GhostMask.MaskType.Lion => crn.Mark,
            GhostMask.MaskType.Peacock => crn.Tilda,
            _ => -1
        };

        return FetchDialogue(dialogueIndex);
    }

    public Dialogue FetchDialogue(int dialogueIndex)
    {
        DialogueEntry de = dialogueData.dialogues.FirstOrDefault(e => e.index == dialogueIndex);

        unlockedKeys.UnionWith(de.reward_keys);

        Dialogue d = new Dialogue();
        d.dialogueIndex = dialogueIndex;
        d.text = de.text;
        d.responses = new List<ResponseField>();
        foreach (var (r, index) in de.responses.Select((value, i) => (value, i)))
        {
            if (unlockedKeys.IsSupersetOf(r.reveal_keys) && !unlockedKeys.IsSupersetOf(r.clear_keys))
            {
                ResponseField rf = new ResponseField();
                rf.text = r.text;
                rf.responseIndex = index;
                d.responses.Add(rf);
            }
        }

        return d;
    }

    public Dialogue GetFollowUpDialogue(int dialogueIndex, int responseIndex)
    {
        DialogueEntry de = dialogueData.dialogues.FirstOrDefault(e => e.index == dialogueIndex);
        Response r = de.responses[responseIndex];
        return FetchDialogue(r.follow_up_index);
    }
}
