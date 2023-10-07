using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum HitType
{
    Hit, Miss
}

public class DamagePopupContainer : MonoBehaviour
{
    List<TMP_Text> damagePopups = new List<TMP_Text>();
    [SerializeField] TMP_Text damagePopupPrefab;

    private void Awake()
    {
        
    }

    public void AddPopup(HitType hitType, int value)
    {
        if (hitType == HitType.Hit)
        {
            Add(value.ToString());
        }
        else if (hitType == HitType.Miss)
        {
            Add("miss");
        }
    }

    public void AddMiss()
    {
        Add("miss");
    }

    void Add(string text)
    {
        if (damagePopups.Count == 0)
        {
            StartCoroutine(StartCountdown());
        }
        TMP_Text newPopup = Instantiate(damagePopupPrefab);
        newPopup.transform.SetParent(transform, false);
        newPopup.text = text;
        damagePopups.Add(newPopup);
        UIUtils.List(newPopup.gameObject, damagePopups.Count, 5, Vector2.up);
    }

    IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(2);
        foreach (TMP_Text damagePopup in damagePopups)
        {
            Destroy(damagePopup.gameObject);
        }
        damagePopups.Clear();
    }
}
