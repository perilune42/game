using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrail : MonoBehaviour
{
    public Vector3 target { private get; set; }
    public float speed = 20;
    float time = -1;
    bool started = false;

    public void StartTimer(float time)
    {
        this.time = time;
    }

    private void Update()
    {
        if (!started && time > 0)
        {
            time -= Time.deltaTime;
        }
        else if (time <= 0) started = true;
        if (started) transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position,target) < 10)
        {
            Destroy(gameObject,2);
        }

    }
}
