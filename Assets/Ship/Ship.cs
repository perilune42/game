using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Ship : MonoBehaviour
{
    public Team team = Team.Player;
    public int speed = 4;
    public int accel = 4;
    public HexDirection moveDir = HexDirection.N;
    public HexDirection headingDir = HexDirection.N;
    public HexCoordinates pos = new HexCoordinates(5,0);
    public PositionPreview positionPreview;
    public int rotateSpeed = 1;
    public int actions = 4;
    public string shipName = "Ship";
    public int id;
    public int uid;
    public bool isSelected = false;

    public ShipStatus shipStatus;

    public Weapon[] weapons;

    
    

    HexGrid hexGrid;
    [HideInInspector]
    public HexCell cell;
    HexCoordinates nextTile;
    PathShower pathShower;
    ObjectMover objectMover;

    float slowThreshold = 1f;
    float fastThreshold = 2f;
    float turningFactor = 0.5f;

    private void Awake()
    {
        hexGrid = GetComponentInParent<HexGrid>();
        pathShower = GetComponentInChildren<PathShower>();
        positionPreview = GetComponentInChildren<PositionPreview>();
        objectMover = GetComponent<ObjectMover>();
        shipStatus = GetComponent<ShipStatus>();
        weapons = GetComponentsInChildren<Weapon>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cell = hexGrid.GetCellAtPos(pos);
        cell.containedShip = this;
        transform.position = cell.transform.position;
        GetNextTile();
    }
    void Update() {

    }
    
    public void Rotate(HexDirection newHeading)
    {
        headingDir = newHeading;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 60 * (int)headingDir, transform.rotation.eulerAngles.z);
        UseAction(rotateSpeed);
    }
    public void Rotate(int newHeading)
    {
        Rotate((HexDirection)newHeading);
        //Action();
    }

    public void Boost(int force, int length = 1)
    {
        //cruising speed = accel
        if (headingDir == moveDir) speed += force;
        else if (Math.Abs(headingDir - moveDir) == 3) speed -= force;
        else
        {
            if (speed == 0) 
            {
                speed = force;
                moveDir = headingDir;
            }

            
            else if (GetSpeedLevel() == 1) //speed 1 - direct turning
            {
                if (Math.Abs(moveDir - headingDir) == 1 || Math.Abs(moveDir - headingDir) == 5) //60 deg
                {
                    moveDir = headingDir;
                    speed += (int)Math.Ceiling(force * turningFactor);
                }
                else if (moveDir - headingDir == -2 || moveDir - headingDir == 4) //CW 120 deg
                {
                    moveDir = headingDir;
                    speed = accel;
                }
                else if (moveDir - headingDir == 2 || moveDir - headingDir == -4) //CCW 120 deg
                {
                    moveDir = headingDir;
                    speed = accel;
                }
            }
            else if (GetSpeedLevel() == 2)  //speed 2 - double boost required for turning, uses 2 actions
            {
                if (Math.Abs(moveDir - headingDir) == 1 || Math.Abs(moveDir - headingDir) == 5) //60 deg
                {
                    speed += (int)Math.Ceiling(force * turningFactor);
                }
                else if (moveDir - headingDir == -2 || moveDir - headingDir == 4) //CW 120 deg
                {
                    moveDir = (HexDirection)((int)(moveDir + 1) % 6);
                    speed = accel;
                }
                else if (moveDir - headingDir == 2 || moveDir - headingDir == -4) //CCW 120 deg
                {
                    moveDir = (HexDirection)((int)(moveDir + 5) % 6);
                    speed = accel;
                }
            }
            
            else  //speed 3 - cannot turn
            {
                if (Math.Abs(moveDir - headingDir) == 1 || Math.Abs(moveDir - headingDir) == 5) //60 deg
                {
                    speed += (int)Math.Ceiling(force * turningFactor);
                }
                else if (moveDir - headingDir == -2 || moveDir - headingDir == 4) //CW 120 deg
                {
                    speed -= (int)Math.Ceiling(force * turningFactor);
                }
                else if (moveDir - headingDir == 2 || moveDir - headingDir == -4) //CCW 120 deg
                {
                    speed -= (int)Math.Ceiling(force * turningFactor);
                }
            }
        }
        if (speed < 0) {
            speed = -speed;
            moveDir = (HexDirection)((int)(moveDir + 3) % 6);
        }
        GetNextTile();
    }

    public HexCoordinates GetNextTile(int actions)
    {
        nextTile = pos + HexCoordinates.FromDirection(moveDir) * speed * actions;
        pathShower.Show(nextTile);
        return nextTile;
    }

    public HexCoordinates GetNextTile()
    {
        return GetNextTile(1);
    }

    public int GetSpeedLevel()
    {
        if (speed < accel * slowThreshold) return 1;
        else if (speed < accel * fastThreshold) return 2;
        else return 3;
    }

    public void Pass(int actions)
    {
        UseAction(actions);
    }

    void UseAction()
    {
        pos = pos + HexCoordinates.FromDirection(moveDir) * speed;

        cell.containedShip = null;
        cell = hexGrid.GetCellAtPos(pos);
        cell.containedShip = this;

        if(actions == 4)
        {
            shipStatus.RollProjectiles();
        }

        Debug.Log("Ship Moved");
        //transform.position = cell.transform.position;
        
        GetNextTile();
        pathShower.Hide();
        actions--;
    }

    public void TriggerMoveAnim()
    {
        objectMover.MoveTo(pos);
    }
    public void UseAction(int actions)
    {
        for (int i = 0; i < actions; i++)
        {
            UseAction();
        }
    }



    public bool ActionAvailable(ShipAction action)
    {
        switch (action) {
            case ShipAction.Rotate:
                return actions >= rotateSpeed;
            case ShipAction.Boost:
                return actions > 0;
            case ShipAction.None:
                return actions > 0;
            case ShipAction.Pass:
                return actions > 0;
            case ShipAction.DirectTargetShip:
                return actions > 0;
            case ShipAction.Evade:
                return actions == 4;
            default:
                return false;
        }
        
    }

    public void PostMove()
    {
        pathShower.UnHide();
        pathShower.Show(GetNextTile());
        cell = hexGrid.GetCellAtPos(pos);
        GridController.instance.SetActiveCell(cell);
        GameEvents.instance.RecolorMesh();
    }

    void UpdatePath()
    {

    }

    public void _Debug()
    {
        Rotate(((int)headingDir + 1) % 6);
        //Rotate(HexDirection.SE);
        //Debug.Log("Hello");
    }


}
