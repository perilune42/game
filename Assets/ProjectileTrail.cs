using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrail : MonoBehaviour
{
    public Vector3 target { private get; set; }
    public float speed = 20;


    private void Update()
    {

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position,target) < 10)
        {
            Destroy(gameObject,2);
        }

    }
}
