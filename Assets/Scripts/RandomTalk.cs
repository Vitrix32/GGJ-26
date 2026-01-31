using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTalk : MonoBehaviour
{
    public List<string> dialogueOptions;

    public TMPro.TextMeshProUGUI textBox;

    private void Start()
    {
        StartCoroutine(Talk());
    }

    public IEnumerator Talk()
    {
        while (dialogueOptions.Count > 0)
        {
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            int rand = Random.Range(0, dialogueOptions.Count);
            string newDialogue = dialogueOptions[rand];
            int i = 0;
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
        }
        yield return null;
    }
}
