using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjDisplayCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public IProjectile projectile;
    [SerializeField] TMP_Text projNameLabel, hitChanceLabel;

    public void Init(IProjectile projectile, int pos)
    {
        this.projectile = projectile;
        UIUtils.List(gameObject, pos, 5, Vector2.down);
        projNameLabel.text = projectile.GetName();
        hitChanceLabel.text = UIUtils.ToPercent(projectile.ChanceToHit());
        GameEvents.instance.onUpdateProjDisplay += ShowHit;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameEvents.instance.PreviewDamage(projectile.GetTarget(), projectile.GetDamage());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameEvents.instance.PreviewDamage(null, DamageData.none);
    }

    

    void ShowHit(IProjectile projectile, bool hit)
    {
        if(projectile == this.projectile && this != null)
        {
            GameEvents.instance.FreezeProjDisplay(true);
            StartCoroutine(_ShowHit(hit));  
        }
        
    }
    IEnumerator _ShowHit(bool hit)
    {
        
        if (hit) hitChanceLabel.text = "HIT";
        else hitChanceLabel.text = "MISS";
        yield return new WaitForSeconds(1);
        
        
        projectile.Destroy();

        GameEvents.instance.FreezeProjDisplay(false);
        GameEvents.instance.UpdateUI();
    }
}
