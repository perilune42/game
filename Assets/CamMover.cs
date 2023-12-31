using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMover : MonoBehaviour
{
    Vector3 targetPos;

    Vector3 velocity = Vector3.zero;
    public bool isMoving = false;
    public bool isTracking = false;
    int trackCounter;
    float delta = 0;
    Vector3 prevPos = Vector3.zero;
    Transform trackTarget;
    float speedModifier = 1;
    // Start is called efore the first frame update
    void Awake()
    {
        GameEvents.instance.onCamMoveTo += MoveTo;
        GameEvents.instance.onCamTrack += Track;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (trackTarget == null)
        {
            isTracking = false;
            trackCounter = 0;
        }
        if  (isTracking)
        {
            MoveTo(trackTarget.position);
            if(trackCounter > 0) trackCounter--;

        }
        if (targetPos != null && isMoving)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos,
                ref velocity, AnimConfig.camAnimAccel * speedModifier);
            delta = Mathf.Abs((transform.position - prevPos).magnitude);
        }

        if (isMoving && delta < 0.1 && trackCounter == 0)
        {
            Stop();
            delta = 0;
        }
        prevPos = transform.position;
    }

    public void MoveTo(Vector3 pos)
    {
        isMoving = true;
        targetPos = pos;

    }

    public void Track(Transform transform)
    {
        isTracking = true;
        trackTarget = transform;
        speedModifier = 1.5f;
        trackCounter = 10;
        //Debug.Log("Tracking " + transform.gameObject.ToString());
    }

    public void Stop()
    {
        isMoving = false;
        isTracking = false;
        delta = -1;
        speedModifier = 1.5f;
    }



    Vector3 VectorMin(Vector3 a, Vector3 b)
    {
        if (a.magnitude < b.magnitude) return a;
        else return b;
    }
}
