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

    // fetches the correct dialogue option
    //public abstract DialogueEntry FetchDialogue(Ghost.GhostName ghostName);

    // adds the key for a certain response to the list of "seen" stuff
    //public abstract void UnlockKeys(int[] keys);
}