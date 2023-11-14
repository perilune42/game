using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipHud : MonoBehaviour
{
    Ship ship;
    [SerializeField]
    TMP_Text healthLabel;
    [SerializeField]
    TMP_Text actionLabel;
    [SerializeField]
    TMP_Text damageLabel;
    [SerializeField]
    DamagePopupContainer damagePopupContainer;

    [SerializeField] Color activeColor, inactiveColor, enemyActiveColor, enemyInactiveColor;
    [SerializeField] Image hudBackground;
    [SerializeField] ProgressBar healthBar, healthBarPreview;

    private void Start()
    {
        ship = GetComponentInParent<Ship>();
        GameEvents.instance.onUpdateUI += UpdateLabels;
        GameEvents.instance.onPreviewDamage += PreviewDamage;
        GameEvents.instance.onHitShip += DamagePopup;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onPreviewDamage -= PreviewDamage;
        GameEvents.instance.onUpdateUI -= UpdateLabels;
        GameEvents.instance.onHitShip -= DamagePopup;
    }

    /*private void Update()
    {
        if (Vector2.Distance(Input.mousePosition, transform.position) < 50)
        {
           hudBackground.enabled = false;
        } 
        else if (!hudBackground.enabled)
        {
            hudBackground.enabled = true;
        }
    }*/
    void DamagePopup(Ship ship, HitType hitType ,int value)
    {
        if (ship == this.ship)
        {
            damagePopupContainer.AddPopup(hitType, value);
        }
    }

    void PreviewDamage(Ship ship, int damage)
    {
        if (ship == this.ship || ship == null)
        {
            if (damage > 0)
            {
                healthBar.SetLevel(((float)ship.shipStatus.health - damage) / ship.shipStatus.maxHealth);
                
                damageLabel.enabled = true;
                damageLabel.text = (-damage).ToString();
            }
            else if (damage == 0)
            {
                healthBar.SetLevel(((float)this.ship.shipStatus.health) / this.ship.shipStatus.maxHealth);
                healthBarPreview.SetLevel((float)this.ship.shipStatus.health / this.ship.shipStatus.maxHealth);
                damageLabel.enabled = false;

            }
        }
    }

    void UpdateLabels()
    {
        if (ship.isDestroyed)
        {
            gameObject.SetActive(false);
            return;
        }
        healthLabel.text = ship.shipStatus.health.ToString() + "/" + ship.shipStatus.maxHealth;
        if (healthBar.GetLevel() == healthBarPreview.GetLevel())
        {
            healthBar.SetLevel((float)ship.shipStatus.health / ship.shipStatus.maxHealth);
            healthBarPreview.SetLevel((float)ship.shipStatus.health / ship.shipStatus.maxHealth);
        }
        actionLabel.text = ship.actions.ToString();

        if (ship.team == TurnHandler.instance.currentTeam)
        {
            if (ship.isSelected) hudBackground.color = activeColor;
            else hudBackground.color = inactiveColor;
        }
        else { hudBackground.color = enemyInactiveColor; }

    }
}
