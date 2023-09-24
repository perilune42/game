using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRenderer : MonoBehaviour
{


    public void Shoot(Vector3 pos, ProjectileTrail projPrefab)
    {
        ProjectileTrail newProjectile = Instantiate(projPrefab);
        newProjectile.transform.SetParent(transform, false);
        newProjectile.target = pos;
    }
}
