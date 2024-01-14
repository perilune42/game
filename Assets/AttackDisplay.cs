using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text hitChanceLabel, critChanceLabel;
    public static AttackDisplay instance;

    private void Awake()
    {
        instance = this; 
    }

    public void Clear()
    {
        hitChanceLabel.text = string.Empty;
        critChanceLabel.text = string.Empty;
    }
    public void ShowHitChance(float chance, float evadedChance)
    {
        hitChanceLabel.text = $"{UIUtils.ToPercent(chance)} / {UIUtils.ToPercent(evadedChance)}";
    }
    public void ShowCritChance(float critChance)
    {
        critChanceLabel.text = UIUtils.ToPercent(critChance);
    }

}
