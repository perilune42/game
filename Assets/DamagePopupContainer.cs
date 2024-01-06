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

    public void AddPopup(HitType hitType, DamageData damage, CritType crit)
    {
        if (hitType == HitType.Hit)
        {
            Add(damage.healthDamage.ToString(), crit);
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

    void Add(string text, CritType crit = CritType.none)
    {
        if (damagePopups.Count == 0)
        {
            StartCoroutine(StartCountdown());
        }
        TMP_Text newPopup = Instantiate(damagePopupPrefab);
        newPopup.transform.SetParent(transform, false);
        newPopup.text = text;
        if (crit != CritType.none)
        {
            newPopup.color = Color.yellow;
            newPopup.text += " " + crit.ToString();
        }
        damagePopups.Add(newPopup);
        UIUtils.List(newPopup.gameObject, damagePopups.Count, 5, Vector2.up);
    }

    IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(3);
        foreach (TMP_Text damagePopup in damagePopups)
        {
            Destroy(damagePopup.gameObject);
        }
        damagePopups.Clear();
    }
}
