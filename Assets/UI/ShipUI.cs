using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour
{
    Ship ship;
    [SerializeField]
    TMP_Text healthLabel, actionLabel, damageLabel, armorLabel, armorLevelLabel;
    [SerializeField]
    DamagePopupContainer damagePopupContainer;

    [SerializeField] Color activeColor, inactiveColor, enemyActiveColor, enemyInactiveColor;
    [SerializeField] Image hudBackground;
    [SerializeField] ProgressBar healthBar, healthBarPreview, armorBar, armorBarPreview;

    private void Start()
    {
        ship = GetComponentInParent<Ship>();
        GameEvents.instance.onUpdateUI += UpdateLabels;
        GameEvents.instance.onPreviewDamage += PreviewDamage;
        GameEvents.instance.onHitShip += DamagePopup;
        GameEvents.instance.onShipDestroyed += DisableUI;
    }

    private void DisableUI()
    {
        if (ship.isDestroyed)
        { 
        GameEvents.instance.onPreviewDamage -= PreviewDamage;
        GameEvents.instance.onUpdateUI -= UpdateLabels;
        GameEvents.instance.onHitShip -= DamagePopup;
        gameObject.SetActive(false);
        }
    }


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
        armorLabel.text = ship.shipStatus.armorPoints.ToString() + "/" + ship.shipStatus.maxArmorPoints;
        armorLevelLabel.text = ship.shipStatus.armorLevel.ToString();
        if (healthBar.GetLevel() == healthBarPreview.GetLevel())
        {
            healthBar.SetLevel((float)ship.shipStatus.health / ship.shipStatus.maxHealth);
            armorBar.SetLevel((float)ship.shipStatus.armorPoints / ship.shipStatus.maxArmorPoints);
            healthBarPreview.SetLevel((float)ship.shipStatus.health / ship.shipStatus.maxHealth);
            armorBarPreview.SetLevel((float)ship.shipStatus.armorPoints / ship.shipStatus.maxArmorPoints);
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