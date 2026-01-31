using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueOptionsController : MonoBehaviour
{
    [SerializeField] UIDocument dialogueComponent;
    [SerializeField] VisualTreeAsset dialogueOptionTemplate;

    [SerializeField] VisualTreeAsset dialogueBoxTemplate;

    List<String> testDialogue = new List<string>{"Hello World!", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."};

    VisualElement root;
    ScrollView dialogueList;
    
    int dialogueIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        root = dialogueComponent.rootVisualElement;
        dialogueList = root.Q<ScrollView>("ScrollView");
        CreateDialogueOptions(3);
        CreateDialogueBox();
        testDialogue.Add("But when, as the seasons revolved, the year came in which the gods had ordained that he should return home to Ithaca, not even there was he free from toils, even among his own folk. And all the gods pitied him [20] save Poseidon; but he continued to rage unceasingly against godlike Odysseus until at length he reached his own land. Howbeit Poseidon had gone among the far-off Ethiopians—the Ethiopians who dwell sundered in twain, the farthermost of men, some where Hyperion sets and some where he rises, [25] there to receive a hecatomb of bulls and rams, and there he was taking his joy, sitting at the feast; but the other gods were gathered together in the halls of Olympian Zeus. ");
    }


    void CreateDialogueOptions(int numOptions)
    {
        GroupBox optionsList = root.Q<GroupBox>("QuestionHoldBox");
        for (int i = 0; i < numOptions; i++)
        {
            TemplateContainer option = dialogueOptionTemplate.Instantiate();
            var optionText = option.Q<Button>("Question1");
            optionText.text = "What is your favorite ice cream?";
            optionsList.Add(option);
        }
       
    }

    void CreateDialogueBox()
    {
        TemplateContainer dialogueBox = dialogueBoxTemplate.Instantiate();
        var dialogueLabel = dialogueBox.Q<Label>("Dialogue");
        dialogueLabel.text = testDialogue[dialogueIndex];
        root.Q<GroupBox>("MiddleBox").Insert(0, dialogueBox);
        dialogueBox.Q<Button>("ScrollUp").clicked += ScrollUp;
        dialogueBox.Q<Button>("ScrollDown").clicked += ScrollDown;

    }

    void ScrollUp()
    {
        if (dialogueIndex > 0)
        {
            dialogueIndex--;
            var dialogueBox = root.Q<GroupBox>("MiddleBox").Q<TemplateContainer>();
            var dialogueLabel = dialogueBox.Q<Label>("Dialogue");
            dialogueLabel.text = testDialogue[dialogueIndex];
        }
    }

    void ScrollDown()
    {
        if (dialogueIndex < testDialogue.Count - 1)
        {
            dialogueIndex++;
            var dialogueBox = root.Q<GroupBox>("MiddleBox").Q<TemplateContainer>();
            var dialogueLabel = dialogueBox.Q<Label>("Dialogue");
            dialogueLabel.text = testDialogue[dialogueIndex];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
