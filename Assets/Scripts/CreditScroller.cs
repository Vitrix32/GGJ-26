using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScroller : MonoBehaviour
{
    public float scrollSpeed = 50f;

    private RectTransform rectTransform;

    public float startY;
    public float endY;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        ResetPosition();
    }

    void Update()
    {
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (rectTransform.anchoredPosition.y >= endY)
        {
            ResetPosition();
        }
    }

    void ResetPosition()
    {
        rectTransform.anchoredPosition =
            new Vector2(0, startY);
    }
}
