using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    RectTransform rectTransform;
    float initialWidth;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialWidth = rectTransform.rect.width;
    }

    public void SetLevel(float level)
    {
        if (rectTransform != null) rectTransform.sizeDelta = new Vector2(level * initialWidth, 0);
    }

    public float GetLevel()
    {
        return rectTransform.sizeDelta.x;
    }
}
