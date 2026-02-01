using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class Dialogue {
    public string text;
    public int justification; //0 = left (ghost) 1 = right (player)
    public Dialogue(string t, int j)
    {
        text = t;
        justification = j;
    }
}
public class DialogueOptionsController : MonoBehaviour
{
    [SerializeField] UIDocument dialogueComponent;
    [SerializeField] VisualTreeAsset dialogueOptionTemplate;

    [SerializeField] VisualTreeAsset dialogueBoxTemplate;

    List<Dialogue> testDialogue = new List<Dialogue>{new Dialogue("Hello World!", 0), new Dialogue("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", 1)};

    DialogueFetcher dialogueFetcher;
    VisualElement root;
    ScrollView dialogueList;
    
    public event Action dialogueFinished;
    int dialogueIndex = -1;
    // Start is called before the first frame update
    void Start()
    {
        root = dialogueComponent.rootVisualElement;
        root.style.opacity = 0;
        root.style.display = DisplayStyle.None;
        CreateDialogueBox();
    }


    public void setupDialogue(DialogueFetcher.Dialogue dialogueEntry)
    {
        CreateDialogueOptions(dialogueEntry.dialogueIndex, dialogueEntry.responses.ToArray());
        dialogueIndex = -1;
        testDialogue.Clear();
        var dialogueBox = root.Q<GroupBox>("MiddleBox").Q<TemplateContainer>();
        dialogueBox.RemoveFromHierarchy();
        Dialogue newDialogue = new Dialogue(dialogueEntry.text, 0);
        testDialogue.Add(newDialogue);
        CreateDialogueBox();
    }

    public void fadeIn()
    {
        root.experimental.animation.Start(
        new StyleValues
        {
            //left = -500,
            opacity = 0
        },
        new StyleValues
        {
            //left = 0,
            opacity = 1
        },
        3000);

        root.SetEnabled(true);
        root.style.display = DisplayStyle.Flex;
    }

    public void fadeOut()
    {
        root.style.opacity = 1;
        root.experimental.animation.Start(
        new StyleValues
        {
            //left = -500,
            opacity = 1
        },
        new StyleValues
        {
            //left = 0,
            opacity = 0
        },
        1000);
        dialogueFinished?.Invoke();
        root.style.display = DisplayStyle.None;
        root.SetEnabled(false);    }

    void addDialogueText(Dialogue text)
    {
        testDialogue.Add(text);
        var dialogueBox = root.Q<GroupBox>("MiddleBox").Q<TemplateContainer>();
        var dialogueLabel = dialogueBox.Q<Label>("Dialogue");
        dialogueIndex = testDialogue.Count - 1;
        dialogueLabel.text = text.text;
        if (text.justification == 0)
        {
            dialogueLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
        }
        else
        {
            dialogueLabel.style.unityTextAlign = TextAnchor.MiddleRight;
        }

    }
    void CreateDialogueOptions(int dialogueIndex, DialogueFetcher.ResponseField[] responses)
    {
        GroupBox optionsList = root.Q<GroupBox>("QuestionHoldBox");
        optionsList.Clear();
        for (int i = 0; i < responses.Length; i++)
        {
            TemplateContainer option = dialogueOptionTemplate.Instantiate();
            var optionText = option.Q<Button>("Question1");
            optionText.text = responses[i].text;
            DialogueFetcher.ResponseField capturedResponse = responses[i];
            optionText.clicked += () => EvaluateDialogueOption(dialogueIndex, capturedResponse);
            optionsList.Add(option);
        }
        TemplateContainer backButton = dialogueOptionTemplate.Instantiate();
        var backText = backButton.Q<Button>("Question1");
        backText.text = "Back";
        backText.clicked += () => fadeOut();
        optionsList.Add(backButton);
       
    }

    void EvaluateDialogueOption(int dialogueId, DialogueFetcher.ResponseField response)
    {
        StartCoroutine(DialogueOptionCoroutine(dialogueId, response));
    }

    private IEnumerator DialogueOptionCoroutine(int dialogueIndex, DialogueFetcher.ResponseField response)
    {
        addDialogueText(new Dialogue(response.text, 1));
        yield return new WaitForSeconds(1f);
        DialogueFetcher.Dialogue newDialogue = DialogueFetcher.Instance.GetFollowUpDialogue(dialogueIndex, response.responseIndex);
        addDialogueText(new Dialogue(newDialogue.text, 0));
        CreateDialogueOptions(newDialogue.dialogueIndex, newDialogue.responses.ToArray());
    }

    void CreateDialogueBox()
    {
        TemplateContainer dialogueBox = dialogueBoxTemplate.Instantiate();
        //dialogueBox.flexGrow = 7;
        print("There is a dialog box being made here >:()");
        dialogueBox.style.height = Screen.height * 0.7f; // 70% of screen height
        // Update on screen resize
        root.RegisterCallback<GeometryChangedEvent>(evt => {
            dialogueBox.style.height = Screen.height * 0.7f;
        });
        var dialogueLabel = dialogueBox.Q<Label>("Dialogue");
        
        root.Q<GroupBox>("MiddleBox").Insert(0, dialogueBox);
        dialogueBox.Q<Button>("ScrollUp").clicked += ScrollUp;
        dialogueBox.Q<Button>("ScrollDown").clicked += ScrollDown;
        StartCoroutine(DialogueScroll());
        

    }
    private IEnumerator DialogueScroll() {
        root.SetEnabled(false);
        while (dialogueIndex < testDialogue.Count-1) {
        var dialogueLabel = root.Q<Label>("Dialogue");
        print(dialogueIndex);
        dialogueIndex += 1;
        dialogueLabel.text = testDialogue[dialogueIndex].text;
        //dialogueLabel.MarkDirtyRepaint();
        if (testDialogue[dialogueIndex].justification == 0)
            {
                dialogueLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            }
            else
            {
                dialogueLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            }
        yield return new WaitForSeconds(3f);
        
        }
        root.SetEnabled(true);
    }

    void ScrollUp()
    {
        if (dialogueIndex > 0)
        {
            dialogueIndex = dialogueIndex - 1;
            var dialogueBox = root.Q<GroupBox>("MiddleBox").Q<TemplateContainer>();
            var dialogueLabel = dialogueBox.Q<Label>("Dialogue");
            dialogueLabel.text = testDialogue[dialogueIndex].text;
            if (testDialogue[dialogueIndex].justification == 0)
            {
                dialogueLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            }
            else
            {
                dialogueLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            }
        }
    }

    void ScrollDown()
    {
        if (dialogueIndex < testDialogue.Count - 1)
        {
            dialogueIndex = dialogueIndex + 1;
            var dialogueBox = root.Q<GroupBox>("MiddleBox").Q<TemplateContainer>();
            var dialogueLabel = dialogueBox.Q<Label>("Dialogue");
            dialogueLabel.text = testDialogue[dialogueIndex].text;
            if (testDialogue[dialogueIndex].justification == 0)
            {
                dialogueLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            }
            else
            {
                dialogueLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            }
        }
    }
}
