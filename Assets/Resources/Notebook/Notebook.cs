using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
public class Notebook : MonoBehaviour
{
    [SerializeField] UIDocument notebook;
    [SerializeField] UIDocument clues;
    bool cluesOpen = false;
    VisualElement notebookRoot;
    VisualElement cluesRoot;
    Ghost.GhostName currentGhostName;
    GhostMask.MaskType currentMaskType;
    private GameObject player;
    private PointAndClick pointAndClick;
    public Dictionary<Ghost.GhostName, GhostMask.MaskType> ghostData = new() {
        {Ghost.GhostName.Eugene, GhostMask.MaskType.Fish},
        {Ghost.GhostName.Claire, GhostMask.MaskType.Horns},
        {Ghost.GhostName.Mark, GhostMask.MaskType.Lion},
        {Ghost.GhostName.Ed, GhostMask.MaskType.Standard},
        {Ghost.GhostName.Amanda, GhostMask.MaskType.Deer},
        {Ghost.GhostName.Tilda, GhostMask.MaskType.Peacock}
    };
    public Dictionary<GhostMask.MaskType, Ghost.GhostName> ghostGuesses = new() {
        {GhostMask.MaskType.Fish, Ghost.GhostName.None},
        {GhostMask.MaskType.Horns, Ghost.GhostName.None},
        {GhostMask.MaskType.Lion, Ghost.GhostName.None},
        {GhostMask.MaskType.Standard, Ghost.GhostName.None},
        {GhostMask.MaskType.Deer, Ghost.GhostName.None},
        {GhostMask.MaskType.Peacock, Ghost.GhostName.None}
    };
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("MainCharacter");
        pointAndClick = player.GetComponent<PointAndClick>();
        notebookRoot = notebook.rootVisualElement;
        cluesRoot = clues.rootVisualElement;
        cluesRoot.style.opacity = 0;
        notebookRoot.SetEnabled(false);
        cluesRoot.SetEnabled(false);
        notebookRoot.style.opacity = 0;
        var closeButton = cluesRoot.Q<Button>("Close");
        notebookRoot.Q<Button>("Verify").clicked += () => verifyGuesses();
        closeButton.clicked += () => closeClues();
        notebookRoot.Q<Button>("Close").clicked += () => fadeOut();
        cluesRoot.Q<EnumField>("UnmaskChoice").Init(Ghost.GhostName.None);
        cluesRoot.Q<EnumField>("UnmaskChoice").RegisterValueChangedCallback(evt => {
            ghostGuesses[currentMaskType] = (Ghost.GhostName)evt.newValue;
        });
        notebookRoot.style.display = DisplayStyle.None;
        cluesRoot.style.display = DisplayStyle.None;
        //fadeIn();
    }
    void openNotebook() {
        print("open notembook");
        GroupBox ghostList = notebookRoot.Q<GroupBox>("GroupBox");
        cluesRoot.style.display = DisplayStyle.None;
        ghostList.Clear();
        notebookRoot.SetEnabled(true); 
        foreach (var entry in ghostData) {
            Button ghostButton = new Button();
            ghostButton.style.flexGrow = 2;
            ghostButton.style.height = Length.Percent(80); // or new StyleLength(new Length(80, LengthUnit.Percent))
    
            // For images in UI Toolkit, set background-image
            ghostButton.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>($"Sprites/Ghost/{entry.Key}_card"));
            ghostButton.style.backgroundColor = new Color(0,0,0,0);
            ghostButton.style.borderLeftWidth = 0;
            ghostButton.style.borderRightWidth = 0;
            ghostButton.style.borderTopWidth = 0;
            ghostButton.style.borderBottomWidth = 0;
    
            // Optional: Scale the image properly
            ghostButton.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            ghostButton.clicked += () => openClues(entry.Key, entry.Value);
            ghostList.Add(ghostButton);
        }
    }

    void openClues(Ghost.GhostName ghostName, GhostMask.MaskType maskType) {
        print("open clues");
        notebookRoot.style.display = DisplayStyle.None;
        cluesRoot.style.display = DisplayStyle.Flex;
        cluesRoot.style.opacity = 1;
        currentGhostName = ghostName;
        currentMaskType = maskType;
        cluesRoot.SetEnabled(true);
        cluesRoot.Q<EnumField>("UnmaskChoice").Init(ghostGuesses[maskType]);
        Label ghostNameLabel = cluesRoot.Q<Label>("Name");
        ghostNameLabel.text = maskType.ToString();
        VisualElement ghostImage = cluesRoot.Q<VisualElement>("VisualElement");
        ghostImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>($"Sprites/Ghost/{ghostName}_card"));
        ghostImage.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
    }

    void verifyGuesses()
    {
        int correctGuesses = 0;
        foreach (var entry in ghostGuesses)
        {
            if (ghostData.ContainsKey(entry.Value) &&ghostData[entry.Value] == entry.Key) {
                correctGuesses += 1;
            }
        }
        if (correctGuesses == ghostData.Count) {
            //Player has won
            Debug.Log("All guesses correct!");
        }
        else
        {
            StartCoroutine(ShakeButton(notebookRoot.Q<Button>("Verify")));
        }
    }

    IEnumerator ShakeButton(Button button, float duration = 0.4f, float intensity = 5f)
{
    float elapsed = 0f;
    Vector3 originalPosition = button.transform.position;
    
    while (elapsed < duration)
    {
        float x = UnityEngine.Random.Range(-1f, 1f) * intensity;
        float y = UnityEngine.Random.Range(-1f, 1f) * intensity;
        
        button.style.translate = new Translate(x, y);
        
        elapsed += Time.deltaTime;
        yield return null;
    }
    
    button.style.translate = new Translate(0, 0);
}

    void closeClues() {
        print("Close clues");
        cluesRoot.style.display = DisplayStyle.None;
        fadeIn();
    }

    public void fadeIn()
    {
        openNotebook();
        pointAndClick.isTalking = true;
        cluesRoot.style.opacity = 0;
        notebookRoot.style.display = DisplayStyle.Flex;
        notebookRoot.SetEnabled(false);
        notebookRoot.experimental.animation.Start(
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
        2000).OnCompleted(() => {
            notebookRoot.SetEnabled(true);
            
        });
        
        
    }
    public void fadeOut()
    {
        cluesRoot.style.opacity = 0;
        notebookRoot.style.display = DisplayStyle.Flex;
        notebookRoot.SetEnabled(false);
        notebookRoot.experimental.animation.Start(
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
        2000).OnCompleted(() => {
            pointAndClick.isTalking = false;
            notebookRoot.style.display = DisplayStyle.None;
        });
        
        
    }
    /*
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
        root.SetEnabled(false);    }*/
    // Update is called once per frame
    void Update()
    {
        
    }
}
