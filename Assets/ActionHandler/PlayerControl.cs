using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    public PlayerAction currentAction = PlayerAction.None;
    //public PlayerAction pendingAction = PlayerAction.None;
    public Ship selectedShip;
    public TMP_Text actionLabel;
    HexGrid hexGrid;
    public HexCell lastSelectedCell = null;
    List<HexCell> tempColoredCells = new List<HexCell>();
    public ShipList shipList;
    public TurnHandler turnHandler;
    CamMover camMover;

    HexDirection? pendingDirection;
    int prevSpeed;
    HexDirection prevMoveDir;

    void Awake()
    {
        prevSpeed = selectedShip.speed;
        prevMoveDir = selectedShip.moveDir;
        hexGrid = FindObjectOfType<HexGrid>();
        turnHandler = FindObjectOfType<TurnHandler>();
        camMover = FindObjectOfType<CamMover>();   
    }

    public void Init()
    {
        SwitchShip(0);
        //turnHandler.Init();
    }
    public void SetCurrentAction(PlayerAction newAction, bool keep = false)
    {
        if (newAction != currentAction)
        {
            pendingDirection = null;
            selectedShip.positionPreview.Hide();
            if (tempColoredCells != null)
            {
                foreach (var cell in tempColoredCells)
                {
                    cell.ColorDefault();
                }
            }
            tempColoredCells.Clear();
            if (currentAction != newAction && currentAction != PlayerAction.None && !keep) //switching between actions
            {
                selectedShip.speed = prevSpeed; //reset previews
                selectedShip.moveDir = prevMoveDir;
                selectedShip.GetNextTile();
                selectedShip.positionPreview.Hide();
            }
            if (newAction == PlayerAction.None)
            {
                actionLabel.text = "";

            }
            if (newAction == PlayerAction.Pass)
            {
                actionLabel.text = "Pass";
                selectedShip.positionPreview.PreviewAt(selectedShip.GetNextTile(), selectedShip.headingDir);
                tempColoredCells.Add(hexGrid.GetCellAtPos(selectedShip.GetNextTile()));
            }
            if (newAction == PlayerAction.Rotate)
            {
                actionLabel.text = "Rotate";

                tempColoredCells.Add(hexGrid.GetCellAtPos(HexVector.FromDirection(HexDirection.N) + selectedShip.pos));
                tempColoredCells.Add(hexGrid.GetCellAtPos(HexVector.FromDirection(HexDirection.NE) + selectedShip.pos));
                tempColoredCells.Add(hexGrid.GetCellAtPos(HexVector.FromDirection(HexDirection.SE) + selectedShip.pos));
                tempColoredCells.Add(hexGrid.GetCellAtPos(HexVector.FromDirection(HexDirection.S) + selectedShip.pos));
                tempColoredCells.Add(hexGrid.GetCellAtPos(HexVector.FromDirection(HexDirection.SW) + selectedShip.pos));
                tempColoredCells.Add(hexGrid.GetCellAtPos(HexVector.FromDirection(HexDirection.NW) + selectedShip.pos));

                foreach (var cell in tempColoredCells)
                {
                    cell.ColorHighlight();
                }

            }
            if (newAction == PlayerAction.Boost && currentAction != PlayerAction.Boost)
            {
                actionLabel.text = "Boost";
                selectedShip.Boost(selectedShip.accel);
                selectedShip.positionPreview.PreviewAt(selectedShip.GetNextTile(), selectedShip.headingDir);
                tempColoredCells.Add(hexGrid.GetCellAtPos(selectedShip.GetNextTile()));
                foreach (var cell in tempColoredCells)
                {
                    cell.ColorHighlight();
                }
            }

            hexGrid.hexMesh.RecolorMesh();
            currentAction = newAction;
        }
    }

    public void InteractCell(HexCoordinates coordinates)
    {
        switch (currentAction) {
            case PlayerAction.None:
                SwitchShipAtPos(coordinates);
                break;
            case PlayerAction.Rotate:
                if (currentAction == PlayerAction.Rotate)
                {

                    HexCoordinates selectedDirection = coordinates - selectedShip.pos;
                    HexDirection? targetDirection;
                    if (selectedDirection == HexVector.FromDirection(HexDirection.N)) targetDirection = HexDirection.N;
                    else if (selectedDirection == HexVector.FromDirection(HexDirection.NE)) targetDirection = HexDirection.NE;
                    else if (selectedDirection == HexVector.FromDirection(HexDirection.SE)) targetDirection = HexDirection.SE;
                    else if (selectedDirection == HexVector.FromDirection(HexDirection.S)) targetDirection = HexDirection.S;
                    else if (selectedDirection == HexVector.FromDirection(HexDirection.SW)) targetDirection = HexDirection.SW;
                    else if (selectedDirection == HexVector.FromDirection(HexDirection.NW)) targetDirection = HexDirection.NW;
                    else targetDirection = null;

                    if (targetDirection == pendingDirection && targetDirection != null) Confirm();

                    else
                    {
                        pendingDirection = targetDirection;
                        if (pendingDirection != null)
                        {
                            selectedShip.positionPreview.PreviewAt(selectedShip.GetNextTile(selectedShip.rotateSpeed), targetDirection ?? HexDirection.N);
                        }
                        
                    }


                }
                break;
            case PlayerAction.Boost:
                if (coordinates == selectedShip.GetNextTile())
                {
                    Confirm();
                }
                break;

    }
    }


    public void Confirm()
    {
        
        switch (currentAction) {
            case PlayerAction.Pass:
                {
                    UpdateShipPos(selectedShip);
                    SetCurrentAction(PlayerAction.None);

                }
                break;
            case PlayerAction.Rotate:


                if (pendingDirection != null)
                {
                    selectedShip.Rotate(pendingDirection ?? HexDirection.N);
                    UpdateShipPos(selectedShip, selectedShip.rotateSpeed);
                    SetCurrentAction(PlayerAction.None, true);
                }
                break;

            case PlayerAction.Boost:
                UpdateShipPos(selectedShip);
                SetCurrentAction(PlayerAction.None, true);
                break;
            

            
        }
        
        prevSpeed = selectedShip.speed;
        prevMoveDir = selectedShip.moveDir;
        selectedShip.positionPreview.Hide();
        hexGrid.hexMesh.RecolorMesh();
        selectedShip.GetNextTile();
        shipList.UpdateCards();
    }

    public void CycleShip()
    {
        SwitchShip(shipList.ships[(selectedShip.id + 1) % shipList.ships.Count]);
    }
    public void SwitchShip(Ship ship)
    {
        if (ship.team == shipList.team)
        {
            selectedShip.isSelected = false;
            selectedShip = ship;
            ship.isSelected = true;
            prevSpeed = selectedShip.speed;
            prevMoveDir = selectedShip.moveDir;
            HexCell cell = hexGrid.GetCellAtPos(ship.pos);
            cell.ColorSelectShip();
            camMover.MoveTo(cell.transform.position);
            if (lastSelectedCell != null && lastSelectedCell.coordinates != cell.coordinates) lastSelectedCell.ColorDefault();
            lastSelectedCell = cell;
            shipList.UpdateCards();
            hexGrid.hexMesh.RecolorMesh();
            
        }
    }

    public void SwitchShip(int id)
    {
        SwitchShip(shipList.ships[id]);
    }

    void SwitchShipAtPos(HexCoordinates coordinates)
    {
        if (hexGrid.GetCellAtPos(coordinates).containedShip != null)
        {
            SwitchShip(hexGrid.GetCellAtPos(coordinates).containedShip);
        }
    }


    void UpdateShipPos(Ship ship, int actions = 1)
    {
        hexGrid.GetCellAtPos(ship.pos).ColorDefault();
        ship.UseAction(actions);
        
        lastSelectedCell = hexGrid.GetCellAtPos(ship.pos);
        hexGrid.hexMesh.RecolorMesh();
    }

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }
}
