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


    
    [SerializeField]
    private TextAsset jsonFile;
    private DialogueData dialogueData;
    private HashSet<int> unlockedKeys;


    private void Start()
    {
       dialogueData = JsonUtility.FromJson<DialogueData>(jsonFile.text); 
    }

    public DialogueEntry FetchDialogue(Ghost.GhostName ghostName)
    {
        GhostMask.MaskType playerMask = GameObject.FindWithTag("Player").GetComponent<GhostMask>().Mask;

        DialogueEntry selectedEntry = null;
        foreach (DialogueEntry entry in dialogueData.dialogues)
        {
            // if this is a dialogue option for a different character, skip it
            if (entry.ghostName != ghostName.ToString())
            {
                continue;
            }
            // if you're not wearing the right mask, skip it
            if (!entry.prereqs.valid_masks.Contains(playerMask.ToString())) {
                continue;
            }
            // if there are any prereq keys that you don't have, skip it.
            if (entry.prereqs.keys.Except(unlockedKeys).Any())
            {
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
