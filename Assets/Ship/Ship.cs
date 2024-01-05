using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Ship : MonoBehaviour
{
    public Team team = Team.Player;
    public int speed;
    public int thrust;
    public HexDirection moveDir = HexDirection.N;
    public HexDirection headingDir = HexDirection.N;
    public HexCoordinates pos = new HexCoordinates(5,0);
    public PositionPreview positionPreview;
    public int rotateSpeed = 1;
    public int actions;
    public int maxActions = -1;
    public string shipName = "Ship";
    public int id;
    public int uid;
    public bool isSelected = false;
    public bool isDestroyed = false;
    

    public AILogic AILogic = null;
    public ShipStatus shipStatus;
    public ShipDataSO shipData;


    public Weapon[] weapons;

    
    

    HexGrid hexGrid;
    [HideInInspector]
    public HexCell cell;
    HexCoordinates nextTile;
    PathShower pathShower;
    ObjectMover objectMover;
    MeshRenderer meshRenderer;

    float slowThreshold = 1f;
    float fastThreshold = 2f;
    float turningFactor = 0.5f;

    private void Awake()
    {
        thrust = shipData.thrust;
        hexGrid = HexGrid.instance;
        actions = GameConfig.turnActions;
        pathShower = GetComponentInChildren<PathShower>();
        positionPreview = GetComponentInChildren<PositionPreview>();
        objectMover = GetComponent<ObjectMover>();
        shipStatus = GetComponent<ShipStatus>();
        weapons = GetComponentsInChildren<Weapon>();
        AILogic = GetComponent<AILogic>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (maxActions == -1)
        {
            maxActions = shipData.defaultMaxActions;
        }
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
        PassAction(rotateSpeed);
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
                    speed = thrust;
                }
                else if (moveDir - headingDir == 2 || moveDir - headingDir == -4) //CCW 120 deg
                {
                    moveDir = headingDir;
                    speed = thrust;
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
                    speed = thrust;
                }
                else if (moveDir - headingDir == 2 || moveDir - headingDir == -4) //CCW 120 deg
                {
                    moveDir = (HexDirection)((int)(moveDir + 5) % 6);
                    speed = thrust;
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
        shipStatus.isEvading = true;
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
        if (speed < thrust * slowThreshold) return 1;
        else if (speed < thrust * fastThreshold) return 2;
        else return 3;
    }




    public void EndTurnMove()
    {
        foreach (Weapon weapon in weapons)
        {
            weapon.PassTurn();
        }
        pos = pos + HexCoordinates.FromDirection(moveDir) * speed;

        cell.containedShip = null;
        cell = hexGrid.GetCellAtPos(pos);
        cell.containedShip = this;

        GetNextTile();
        pathShower.Hide();
    }

    public void TriggerMoveAnim()
    {
        objectMover.MoveTo(pos);
    }

    public void GiveActions()
    {
        actions = maxActions;
    }
    public void PassAction(int actions)
    {
        for (int i = 0; i < actions; i++)
        {
            UseAction();
        }
    }


    public void Evade()
    {
        shipStatus.isEvading = true;
        PassAction(1);
    }

    public void Brace()
    {
        shipStatus.RollProjectiles();
    }

    void UseAction()
    {

        if (shipStatus.IsUnderAttack())
        {
            shipStatus.RollProjectiles();
        }

        GetNextTile();
        actions--;
        GameEvents.instance.UseCommandPoint();
    }

    bool IsFirstAction()
    {
        return actions == maxActions;
    }

    public bool ActionAvailable(ControlAction action)
    {
        if (shipStatus.IsUnderAttack())
        {
            switch (action)
            {
                case ControlAction.Evade:
                    return actions > 0;
                case ControlAction.Brace:
                    return true;
                case ControlAction.Boost:
                    return actions > 0;
            }
        }

        switch (action)
        {
            case ControlAction.Rotate:
                return actions >= rotateSpeed;
            case ControlAction.Boost:
                return actions > 0;
            case ControlAction.None:
                return actions > 0;
            case ControlAction.Pass:
                return actions > 0;
            case ControlAction.DirectTargetShip:
                return actions > 0;
            default:
                return false;
        }
        
        
    }

    public bool ActionAvailable()
    {
        return actions > 0;
    }

    public void PostMove()
    {
        pathShower.UnHide();
        pathShower.Show(GetNextTile());
        cell = hexGrid.GetCellAtPos(pos);
        GridController.instance.SetActiveCell(cell);
        GameEvents.instance.RecolorMesh();
    }

    public void Destroy()
    {
        isDestroyed = true;
        meshRenderer.gameObject.SetActive(false);
        pathShower.Hide();
        GameEvents.instance.ShipDestroyed();
    }

    public void _Debug()
    {
        Rotate(((int)headingDir + 1) % 6);
        //Rotate(HexDirection.SE);
        //Debug.Log("Hello");
    }

    public static float Distance(Ship ship1, Ship ship2)
    {
        return HexCoordinates.Distance(ship1.pos, ship2.pos);
    }

}
