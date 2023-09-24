using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjDisplayCard : MonoBehaviour
{
    public KineticProjectile projectile;
    [SerializeField] TMP_Text projNameLabel, hitChanceLabel;

    public void Init(KineticProjectile projectile, int pos)
    {
        this.projectile = projectile;
        UIUtils.List(gameObject, pos, 5, Vector2.down);
        projNameLabel.text = projectile.name;
        hitChanceLabel.text = UIUtils.ToPercent(projectile.ChanceToHit());
        GameEvents.instance.onUpdateProjDisplay += ShowHit;
    }

    void ShowHit(KineticProjectile projectile, bool hit)
    {
        if(projectile == this.projectile) { StartCoroutine(_ShowHit(hit));  }
        
    }
    IEnumerator _ShowHit(bool hit)
    {
        GameEvents.instance.FreezeProjDisplay(true);
        if (hit) hitChanceLabel.text = "HIT";
        else hitChanceLabel.text = "MISS";
        yield return new WaitForSeconds(1);
        
        
        Destroy(projectile);

        GameEvents.instance.FreezeProjDisplay(false);
        GameEvents.instance.UpdateUI();
    }
}
