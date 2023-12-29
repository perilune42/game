using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour
{
    Ship ship;
    [SerializeField]
    TMP_Text healthLabel, actionLabel, damageLabel, armorLabel, armorDamageLabel, armorLevelLabel;
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
        GameEvents.instance.onHitShip += SetHealthBars;
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

    void PreviewDamage(Ship ship, DamageData damage)
    {
        if (ship == this.ship || (ship == null && damage.Equals(DamageData.none)))
        {
            if (damage.healthDamage > 0)
            {
                healthBar.SetLevel(((float)ship.shipStatus.health - damage.healthDamage) / ship.shipStatus.maxHealth);
                
                damageLabel.enabled = true;
                damageLabel.text = (-damage.healthDamage).ToString();
            }
            else if (damage.healthDamage == 0)
            {
                healthBar.SetLevel(((float)this.ship.shipStatus.health) / this.ship.shipStatus.maxHealth);
                healthBarPreview.SetLevel((float)this.ship.shipStatus.health / this.ship.shipStatus.maxHealth);
                damageLabel.enabled = false;
            }

            if (damage.armorDamage > 0)
            {
                armorBar.SetLevel(((float)ship.shipStatus.armorPoints - damage.armorDamage) / ship.shipStatus.maxArmorPoints );

                armorDamageLabel.enabled = true;
                armorDamageLabel.text = (-damage.armorDamage).ToString();
            }
            else if (damage.armorDamage == 0)
            {
                armorBar.SetLevel(((float)this.ship.shipStatus.armorPoints) / this.ship.shipStatus.maxArmorPoints);
                armorBarPreview.SetLevel((float)this.ship.shipStatus.armorPoints / this.ship.shipStatus.maxArmorPoints);
                armorDamageLabel.enabled = false;

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
        
        /*
        if (healthBar.GetLevel() == healthBarPreview.GetLevel() && 
            Mathf.Abs(healthBar.GetLevel() - (float)ship.shipStatus.health / ship.shipStatus.maxHealth) > 0.01f)
        {
            healthBar.SetLevel((float)ship.shipStatus.health / ship.shipStatus.maxHealth);
            armorBar.SetLevel((float)ship.shipStatus.armorPoints / ship.shipStatus.maxArmorPoints);
            healthBarPreview.SetLevel((float)ship.shipStatus.health / ship.shipStatus.maxHealth);
            armorBarPreview.SetLevel((float)ship.shipStatus.armorPoints / ship.shipStatus.maxArmorPoints);
        }
        */
        
        actionLabel.text = ship.actions.ToString();

        if (ship.team == TurnHandler.instance.currentTeam)
        {
            if (ship.isSelected) hudBackground.color = activeColor;
            else hudBackground.color = inactiveColor;
        }
        else { hudBackground.color = enemyInactiveColor; }

    }

    void SetHealthBars(Ship otherShip, HitType hitType, int value)
    {
        healthBar.SetLevel((float)ship.shipStatus.health / ship.shipStatus.maxHealth);
        armorBar.SetLevel((float)ship.shipStatus.armorPoints / ship.shipStatus.maxArmorPoints);
        healthBarPreview.SetLevel((float)ship.shipStatus.health / ship.shipStatus.maxHealth);
        armorBarPreview.SetLevel((float)ship.shipStatus.armorPoints / ship.shipStatus.maxArmorPoints);
    }
}
