using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTalk : MonoBehaviour
{
    public List<string> dialogueOptions;

    public TMPro.TextMeshProUGUI textBox;

    private Coroutine talk;

    private void Start()
    {
        textBox.gameObject.SetActive(false);
    }

    public IEnumerator Talk()
    {
        yield return new WaitForSeconds(Random.Range(0f, 5f));
        while (dialogueOptions.Count > 0)
        {
            yield return new WaitForSeconds(Random.Range(45f, 75f));
            int rand = Random.Range(0, dialogueOptions.Count);
            string newDialogue = dialogueOptions[rand];
            int i = 0;
            textBox.gameObject.SetActive(true);
            while (i < newDialogue.Length)
            {
                textBox.text += newDialogue[i];
                i++;
                yield return new WaitForSeconds(0.07f);
                if (char.IsPunctuation(newDialogue[i - 1]))
                {
                    yield return new WaitForSeconds(.13f);
                }
            }
            yield return new WaitForSeconds(5f);
            textBox.text = "";
            textBox.gameObject.SetActive(false);
        }
        yield return null;
    }

    public void StopRandomTalking()
    {
        StopCoroutine(talk);
        textBox.text = "";
        textBox.gameObject.SetActive(false);
    }

    public void StartRandomTalking()
    {
        talk = StartCoroutine(Talk());
    }

    public bool talking = false;

    public IEnumerator SayLine(string s)
    {
        Debug.Log("saying line " + s);
        string newDialogue = s;
        int i = 0;
        textBox.gameObject.SetActive(true);
        while (i < newDialogue.Length)
        {
            textBox.text += newDialogue[i];
            i++;
            yield return new WaitForSeconds(0.07f);
            if (char.IsPunctuation(newDialogue[i - 1]))
            {
                yield return new WaitForSeconds(.13f);
            }
        }
        yield return new WaitForSeconds(2f);
        textBox.text = "";
        textBox.gameObject.SetActive(false);
        talking = false;
        yield return null;
    }
}
