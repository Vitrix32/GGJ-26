using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct dialogueMatch
{
    public string line;
    public GhostMask.MaskType who;
}

public class WinScreenController : MonoBehaviour
{
    public GameObject Ed;
    public GameObject Claire;
    public GameObject Amanda;
    public GameObject Tilda;
    public GameObject Eugene;
    public GameObject Mark;

    public List<dialogueMatch> dialogues;

    int current = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        Ed = GameObject.Find("Ed");
        Claire = GameObject.Find("Claire");
        Amanda = GameObject.Find("Amanda");
        Tilda = GameObject.Find("Tilda");
        Eugene = GameObject.Find("Eugene");
        Mark = GameObject.Find("Mark");

        Ed.GetComponent<GuestBehavior>().StopWalking();
        Claire.GetComponent<GuestBehavior>().StopWalking();
        Amanda.GetComponent<GuestBehavior>().StopWalking();
        Tilda.GetComponent<GuestBehavior>().StopWalking();
        Eugene.GetComponent<GuestBehavior>().StopWalking();
        Mark.GetComponent<GuestBehavior>().StopWalking();

        StartCoroutine(StartCutscene());
    }

    public IEnumerator StartCutscene()
    {
        yield return new WaitForSeconds(3f);
        StartCoroutine(SayLines());
    } 

    public IEnumerator SayLines()
    {
        while (current < dialogues.Count)
        {
            dialogueMatch currentDialogue = dialogues[current];
            GhostMask.MaskType nextSpeaker = currentDialogue.who;
            string currentLine = currentDialogue.line;
            GameObject speaker = null;
            switch (nextSpeaker)
            {
                case (GhostMask.MaskType.Standard):
                    speaker = Ed;
                    break;
                case (GhostMask.MaskType.Deer):
                    speaker = Amanda;
                    break;
                case (GhostMask.MaskType.Fish):
                    speaker = Eugene;
                    break;
                case (GhostMask.MaskType.Horns):
                    speaker = Claire;
                    break;
                case (GhostMask.MaskType.Lion):
                    speaker = Mark;
                    break;
                case (GhostMask.MaskType.Peacock):
                    speaker = Tilda;
                    break;
            }
            RandomTalk rt =  speaker.GetComponent<RandomTalk>();
            rt.talking = true;
            StartCoroutine(rt.SayLine(currentLine));
            while (rt.talking)
            {
                yield return null;
            }
            current++;
        }
        StartCoroutine(UnmaskAll());
    }

    public IEnumerator UnmaskAll()
    {
        Ed.GetComponent<GuestBehavior>().LoseMask();
        Tilda.GetComponent<GuestBehavior>().LoseMask();
        Eugene.GetComponent<GuestBehavior>().LoseMask();
        Amanda.GetComponent<GuestBehavior>().LoseMask();
        Claire.GetComponent<GuestBehavior>().LoseMask();
        Mark.GetComponent<GuestBehavior>().LoseMask();
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("EndScene");
    }
}
