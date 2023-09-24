using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponList : MonoBehaviour
{
    public List<Weapon> weapons;
    public PlayerControl playerControl;
    public WeaponButton weaponButtonPrefab;
    List<WeaponButton> buttons = new List<WeaponButton>();

    private void Awake()
    {
        GameEvents.instance.onDisplayWeapons += DisplayWeapons;
    }


    // Start is called before the first frame update
    public void DisplayWeapons(Ship ship)
    {
        foreach (WeaponButton button in buttons) { 
            Destroy(button.gameObject);
        }
        buttons.Clear();
        weapons.Clear();
        int i = 0;
        foreach (Weapon weapon in ship.weapons)
        {
            weapons.Add(weapon);
            WeaponButton button = Instantiate<WeaponButton>(weaponButtonPrefab);
            button.transform.SetParent(transform, false);
            button.weapon = weapon;
            //card.shipList = this;
            button.SetPosition(i);
            button.Init();
            buttons.Add(button);


            i++;
            
        }
    }

    
    // Update is called once per frame
    void Update()
    {

    }
}
