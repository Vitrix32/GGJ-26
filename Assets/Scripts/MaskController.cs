using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct MaskPair
{
    public GhostMask.MaskType Key;
    public Sprite Value;
}

public class MaskController : MonoBehaviour
{
    public SpriteRenderer playerMask;

    public List<MaskPair> masks;

    public GhostMask.MaskType currentlyWearing;

    public List<Button> MaskButtons;

    // Start is called before the first frame update
    void Start()
    {
        MaskButtons[0].onClick.AddListener(delegate { swapMasks(0); });
        MaskButtons[1].onClick.AddListener(delegate { swapMasks(1); });
        MaskButtons[2].onClick.AddListener(delegate { swapMasks(2); });
        MaskButtons[3].onClick.AddListener(delegate { swapMasks(3); });
        MaskButtons[4].onClick.AddListener(delegate { swapMasks(4); });
        MaskButtons[5].onClick.AddListener(delegate { swapMasks(5); });
        MaskButtons[6].onClick.AddListener(delegate { swapMasks(6); });
        MaskButtons[7].onClick.AddListener(delegate { swapMasks(7); });
    }

    private void swapMasks(int maskNum)
    {
        MaskPair maskToPutOn = masks[maskNum];
        currentlyWearing = maskToPutOn.Key;
        playerMask.sprite = maskToPutOn.Value;
    }
}
