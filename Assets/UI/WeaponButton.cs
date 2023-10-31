using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class WeaponButton : MonoBehaviour
{
    public TMP_Text weaponNameLabel, ammoLabel, reloadLabel;

    public int horizSpacing = 10;
    public Weapon weapon;
    RectTransform rectTransform;
    UIButtons buttonController;
    Button button;


    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        buttonController = FindObjectOfType<UIButtons>();
        button = GetComponent<Button>();
        GameEvents.instance.onUpdateUI += UpdateLabels;
    }

    public void SetPosition(int pos)
    {
        UIUtils.List(gameObject, pos, horizSpacing, Vector2.right, horizSpacing);
    }

    public void Init()
    {
        weaponNameLabel.text = weapon.weaponName;
        button.onClick.AddListener(() => buttonController.SelectWeapon(weapon));
        UpdateLabels();
    }

    public void UpdateLabels()
    {
        if (weapon == null || button == null) return;
        if (weapon is KineticWeapon k ) //change to interface IUsesAmmo
        {
            ammoLabel.text = k.ammoCount + " / " + k.ammoCapacity;

        }
        if (weapon is IHasCooldown c)
        {
            if (c.GetCooldown() > 0)
            {
                reloadLabel.gameObject.SetActive(true);
                reloadLabel.text = c.GetCooldown().ToString();
            }
            else reloadLabel.gameObject.SetActive(false);
        }
        button.interactable = weapon.CanFire();
    }

}
