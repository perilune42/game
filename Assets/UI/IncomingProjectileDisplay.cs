using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingProjectileDisplay : MonoBehaviour
{
    [SerializeField] ProjDisplayCard cardPrefab;
    List<ProjDisplayCard> cards = new List<ProjDisplayCard>();
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
            foreach (ProjDisplayCard card in cards)
            {
                card.enabled = false;
                Destroy(card.gameObject);
            }
            cards.Clear();
            if (!ship.shipStatus.IsUnderAttack())
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);


            int i = 0;
            foreach (IProjectile projectile in ship.shipStatus.incomingProjectiles)
            {
                ProjDisplayCard card = Instantiate(cardPrefab);
                card.transform.SetParent(transform, false);
                cards.Add(card);
                card.Init(projectile, i);
                i++;
            }
        }
    }
}
