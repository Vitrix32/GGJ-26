using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMask : MonoBehaviour
{
    public enum MaskType
    {
        Standard,
        Fish
    }

    [SerializeField]
    private MaskType mask;
    public MaskType Mask
    {
        get
        {
            return mask;
        }
        set
        {
            if (mask == value)
            {
                return;
            }
            mask = value;
            SetMaskSprite(value);
        }
    }

    private void SetMaskSprite(MaskType newMask)
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Sprites/Ghost/Ghost_Mask_{newMask}");
    }

    private void Start()
    {
        SetMaskSprite(Mask);
    }

}
