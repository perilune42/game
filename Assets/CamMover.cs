using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMover : MonoBehaviour
{
    Vector3 targetPos;
    
    AnimConfig config;

    Vector3 velocity = Vector3.zero;
    public bool isMoving = false;
    float delta = 0;
    Vector3 prevPos = Vector3.zero;
    // Start is called efore the first frame update
    void Awake()
    {
        config = FindObjectOfType<AnimConfig>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (targetPos != null && isMoving)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, config.camAnimAccel);
            delta = Mathf.Abs((transform.position - prevPos).magnitude);
        }

        if (isMoving && delta < 0.1)
        {
            Stop();
        }
        prevPos = transform.position;
    }

    public void MoveTo(Vector3 pos)
    {
        isMoving = true;
        targetPos = pos;
    }

    public void Stop()
    {
        isMoving = false;
        delta = -1;
    }



    Vector3 VectorMin(Vector3 a, Vector3 b)
    {
        if (a.magnitude < b.magnitude) return a;
        else return b;
    }
}
