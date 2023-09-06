using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    HexCoordinates destination;
    Vector3 targetPos;
    HexGrid hexGrid;
    PathShower pathShower;
    Ship ship;
    Vector3 velocity = Vector3.zero;
    public bool isMoving = false;
    float delta = 0;
    Vector3 prevPos = Vector3.zero;
    // Start is called efore the first frame update
    void Awake()
    {
        hexGrid = FindAnyObjectByType<HexGrid>();
        pathShower = GetComponentInChildren<PathShower>();
        ship = GetComponent<Ship>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (destination != null && isMoving)
        {
            targetPos = hexGrid.GetCellAtPos(destination).transform.position;
            transform.position = VectorMin(Vector3.SmoothDamp(transform.position, targetPos, ref velocity, AnimConfig.moveAnimAccel), Vector3.MoveTowards(transform.position, targetPos, AnimConfig.maxMoveSpeed));
            delta = Mathf.Abs((transform.position - prevPos).magnitude);
        }
        
        if (isMoving && delta < 0.1)
        {
            isMoving = false;
            delta = -1;
            ship.PostMove();
        }
        prevPos = transform.position;
    }

    public void MoveTo(HexCoordinates pos)
    {
        isMoving = true;
        destination = pos;
        pathShower.Hide();
        //Debug.Log("moving to"  + pos.ToString());
    }

    Vector3 VectorMin(Vector3 a, Vector3 b)
    {
        if (a.magnitude < b.magnitude) return a;
        else return b;
    }
}
