using System.Collections.Generic;
using UnityEngine;

public static class UIUtils
{
    public static void List(GameObject gameObject, int pos, int spacing,Vector2 dir, int initOffset = 0)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        if (dir == Vector2.up || dir == Vector2.down) 
        {
            rectTransform.anchoredPosition = dir * (rectTransform.rect.height + spacing) * pos + dir * initOffset;
        }
        else
        {
            rectTransform.anchoredPosition = dir * (rectTransform.rect.width + spacing) * pos + dir * initOffset;
        }
        
    }

    public static string ToPercent(float val)
    {
        return Mathf.RoundToInt(val * 100) + "%";
    }
}