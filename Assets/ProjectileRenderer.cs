using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRenderer : MonoBehaviour
{
    static float timeDelay = 0.1f;

    public void Shoot(Vector3 pos, ProjectileTrail projPrefab, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            ProjectileTrail newProjectile = Instantiate(projPrefab);
            newProjectile.transform.SetParent(transform, false);
            newProjectile.target = pos;
            newProjectile.StartTimer(i * timeDelay);
        }
    }
}
