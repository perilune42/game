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
        Debug.Log("Set Level To " + level);
        Debug.Log("width = " + level * initialWidth);
        rectTransform.sizeDelta = new Vector2(level * initialWidth, rectTransform.rect.height);
    }
}
