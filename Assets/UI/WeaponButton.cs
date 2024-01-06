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
    bool locked = false;


    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        buttonController = FindObjectOfType<UIButtons>();
        button = GetComponent<Button>();
        GameEvents.instance.onUpdateUI += UpdateLabels;
        GameEvents.instance.onLockControls += LockControls;
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
        if (weapon is IUsesAmmo a)
        {
            ammoLabel.text = a.GetAmmoCount() + " / " + a.GetAmmoCapacity();
        }
        else if (weapon is ILimitedUse l)
        {
            ammoLabel.text = l.GetRemainingUses().ToString();
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
        if (!locked) button.interactable = weapon.CanFire();
        else button.interactable = false;
    }

    public void LockControls(bool toggle)
    {
        locked = toggle;
        UpdateLabels();

    }

}
