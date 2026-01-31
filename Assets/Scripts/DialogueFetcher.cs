using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueFetcher : MonoBehaviour
{
    [Serializable]
    public class Response {
        public string text;
        public int[] rewardedKeys;
    }

    [Serializable]
    public class Prereqs {
        public string[] valid_masks;
        public int[] keys;
    }

    [Serializable]
    public class DialogueEntry {
        public int dialogueIndex;
        public string ghostName;
        public Prereqs prereqs;
        public string[] ghost_lines;
        public Response[] responses;
    }

    [Serializable]
    public class DialogueData {
        public DialogueEntry[] dialogues;
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

    public DialogueEntry FetchDialogue(Ghost.GhostName ghostName)
    {
        Debug.Log($"Fetching for Ghost: {ghostName}");
        GhostMask.MaskType playerMask = GameObject.FindWithTag("Player").GetComponentInChildren<GhostMask>().Mask;

        DialogueEntry selectedEntry = null;
        foreach (DialogueEntry entry in dialogueData.dialogues)
        {
            // if this is a dialogue option for a different character, skip it
            if (entry.ghostName != ghostName.ToString())
            {
                Debug.Log("Wrong Ghost");
                continue;
            }
            // if you're not wearing the right mask, skip it
            if (!entry.prereqs.valid_masks.Contains(playerMask.ToString())) {
                Debug.Log("Wrong Mask");
                continue;
            }
            // if there are any prereq keys that you don't have, skip it.
            if (entry.prereqs.keys.Except(unlockedKeys).Any())
            {
                Debug.Log("Wrong Keys");
                continue;
            }
            if (selectedEntry is null || entry.dialogueIndex > selectedEntry.dialogueIndex)
            {
                selectedEntry = entry;
            }
        }
        return selectedEntry;
    }

    public void UnlockKeys(int[] keys)
    {
        unlockedKeys.UnionWith(keys);
    }

}
