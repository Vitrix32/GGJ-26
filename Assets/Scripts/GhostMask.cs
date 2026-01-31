using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMask : MonoBehaviour
{
    public enum MaskType
    {
        None,
        Standard,
        Fish,
        Horns,
        Lion
    }

    [SerializeField]
    private MaskType mask;
    virtual public MaskType Mask
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
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (newMask == MaskType.None)
        {
            spriteRenderer.enabled = false;
        }
        else
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = Resources.Load<Sprite>($"Sprites/Ghost/Ghost_Mask_{newMask}");
        }
    }

    private void Start()
    {
        SetMaskSprite(Mask);
    }

}
