using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class WeaponButton : MonoBehaviour
{
    public TMP_Text weaponNameLabel;
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
    }

    public void SetPosition(int pos)
    {
        rectTransform.anchoredPosition = new Vector2( horizSpacing + (rectTransform.rect.width + horizSpacing) * pos, 0);
    }

    public void Init()
    {
        weaponNameLabel.text = weapon.weaponName;
        button.onClick.AddListener(() => buttonController.SelectWeapon(weapon));
    }


   /* public void SetActive(bool active)
    {
        if (active)
        {
            image.color = shipList.activeColor;

        }
        else
        {
            image.color = shipList.defaultColor;
        }
    }
   */
    // Update is called once per frame
    void Update()
    {

    }
}
