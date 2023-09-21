using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingProjectileDisplay : MonoBehaviour
{
    [SerializeField] ProjDisplayElement displayElementPrefab;
    List<ProjDisplayElement> elements = new List<ProjDisplayElement>();
    Ship ship;
    public bool frozen = false;

    private void Awake()
    {
        GameEvents.instance.onUpdateUI += InitList;
        GameEvents.instance.onFreezeProjDisplay += SetFrozen;
    }

    void SetFrozen(bool frozen)
    {
        this.frozen = frozen;
    }

    void InitList()
    {
        if (!frozen)
        {
            ship = PlayerControl.instance.selectedShip;
            if (ship.shipStatus.incomingProjectiles.Count == 0)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);

            foreach (ProjDisplayElement element in elements)
            {
                Destroy(element.gameObject);
            }
            
            elements.Clear();



            int i = 0;
            foreach (KineticProjectile projectile in ship.shipStatus.incomingProjectiles)
            {
                ProjDisplayElement displayElement = Instantiate(displayElementPrefab);
                displayElement.transform.SetParent(transform, false);
                elements.Add(displayElement);
                displayElement.Init(projectile, i);
                i++;
            }
        }
    }
}
