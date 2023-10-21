using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    public static PlayerControl instance;

    public ShipAction currentAction = ShipAction.None;
    //public PlayerAction pendingAction = PlayerAction.None;
    public Ship selectedShip;
    [SerializeField]
    TMP_Text actionLabel;  //event
    [SerializeField]
    HexGrid hexGrid;
    //List<HexCell> tempColoredCells = new List<HexCell>();
    public ShipList shipList;
    

    Ship targetedShip;
    Weapon selectedWeapon;


    HexDirection? pendingDirection;
    int prevSpeed;
    HexDirection prevMoveDir;

    

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        prevSpeed = selectedShip.speed;
        prevMoveDir = selectedShip.moveDir;
        //hexGrid = FindObjectOfType<HexGrid>();
    }

    public void Init()
    {
        SwitchShip(0);
        //turnHandler.Init();
    }


    public void SetCurrentAction(ShipAction newAction, bool keep = false, Weapon weapon = null) //keep: keeping any changes to speed and direction
    {
        if (TurnHandler.instance.IsAITurn()) return;
        GameEvents.instance.PreviewDamage(null, 0);
        selectedShip.shipStatus.isEvading = false;

        if ((newAction != currentAction || weapon != null))
        {
            if (selectedWeapon is IRanged k) k.HideRange();
            selectedWeapon = null;
            pendingDirection = null;
            selectedShip.positionPreview.Hide();
            foreach (var cell in hexGrid.cells)
            {
                cell.isHighlighting = false;
            }
            //tempColoredCells.Clear();
            if (currentAction != newAction && currentAction != ShipAction.None && !keep) //switching between actions
            {
                selectedShip.speed = prevSpeed; //reset previews
                selectedShip.moveDir = prevMoveDir;
                selectedShip.GetNextTile();
                selectedShip.positionPreview.Hide();
            }
            if (newAction == ShipAction.None)
            {
                actionLabel.text = "";

                

            }
            if (newAction == ShipAction.Pass)
            {
                actionLabel.text = "Pass";
                selectedShip.positionPreview.PreviewAt(selectedShip.GetNextTile(), selectedShip.headingDir);
                hexGrid.GetCellAtPos(selectedShip.GetNextTile()).isHighlighting = true;
            }
            if (newAction == ShipAction.Rotate)
            {
                actionLabel.text = "Rotate";

                hexGrid.GetCellAtPos(HexCoordinates.FromDirection(HexDirection.N) + selectedShip.pos).isHighlighting = true;
                hexGrid.GetCellAtPos(HexCoordinates.FromDirection(HexDirection.NE) + selectedShip.pos).isHighlighting = true;
                hexGrid.GetCellAtPos(HexCoordinates.FromDirection(HexDirection.SE) + selectedShip.pos).isHighlighting = true;
                hexGrid.GetCellAtPos(HexCoordinates.FromDirection(HexDirection.S) + selectedShip.pos).isHighlighting = true;
                hexGrid.GetCellAtPos(HexCoordinates.FromDirection(HexDirection.SW) + selectedShip.pos).isHighlighting = true;
                hexGrid.GetCellAtPos(HexCoordinates.FromDirection(HexDirection.NW) + selectedShip.pos).isHighlighting = true;



            }
            if (newAction == ShipAction.Boost && currentAction != ShipAction.Boost)
            {
                actionLabel.text = "Boost";
                selectedShip.Boost(selectedShip.accel);  //event
                selectedShip.positionPreview.PreviewAt(selectedShip.GetNextTile(), selectedShip.headingDir);
                hexGrid.GetCellAtPos(selectedShip.GetNextTile()).isHighlighting = true;

                selectedShip.shipStatus.isEvading = true;



            }

            if (newAction == ShipAction.Evade)
            {
                actionLabel.text = "Evade";
                selectedShip.shipStatus.isEvading = true;
            }

            if (newAction == ShipAction.DirectTargetShip)
            {
                actionLabel.text = "Firing " + weapon.weaponName;
                selectedWeapon = weapon;
                if (weapon is IRanged w) w.DisplayRange();
            }

            GameEvents.instance.UpdateUI();
            GameEvents.instance.RecolorMesh();
            currentAction = newAction;
        }
    }


    public void InteractCell(HexCoordinates coordinates)
    {

        switch (currentAction) {
            case ShipAction.Rotate:
                if (currentAction == ShipAction.Rotate)
                {

                    HexCoordinates selectedDirection = coordinates - selectedShip.pos;
                    HexDirection? targetDirection;
                    if (selectedDirection == HexCoordinates.FromDirection(HexDirection.N)) targetDirection = HexDirection.N;
                    else if (selectedDirection == HexCoordinates.FromDirection(HexDirection.NE)) targetDirection = HexDirection.NE;
                    else if (selectedDirection == HexCoordinates.FromDirection(HexDirection.SE)) targetDirection = HexDirection.SE;
                    else if (selectedDirection == HexCoordinates.FromDirection(HexDirection.S)) targetDirection = HexDirection.S;
                    else if (selectedDirection == HexCoordinates.FromDirection(HexDirection.SW)) targetDirection = HexDirection.SW;
                    else if (selectedDirection == HexCoordinates.FromDirection(HexDirection.NW)) targetDirection = HexDirection.NW;
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
                return;
            case ShipAction.Boost:
                if (coordinates == selectedShip.GetNextTile())
                {
                    Confirm();
                }
                return;
        }
        if (coordinates != selectedShip.pos && hexGrid.GetCellAtPos(coordinates).containedShip != null)
        {
            targetedShip = hexGrid.GetCellAtPos(coordinates).containedShip;
            TargetShip(targetedShip);
        }
    }

    public void TargetShip(Ship targetedShip)
    {
        if (TurnHandler.instance.IsAITurn()) return;
        if (currentAction == ShipAction.DirectTargetShip)
        { 
            if (targetedShip != selectedShip)
            {
                this.targetedShip = targetedShip;
                GameEvents.instance.PreviewDamage(null, 0);
                actionLabel.text = "Targeting " + targetedShip.shipName;
                if (selectedWeapon is KineticWeapon kinetic) actionLabel.text += "\n" + UIUtils.ToPercent(kinetic.ChanceToHit(targetedShip));
                if (selectedWeapon is ITargetsShip w2) GameEvents.instance.PreviewDamage(targetedShip, w2.CalculateDamage(targetedShip));
                
            }
            GameEvents.instance.CamMoveTo(targetedShip.transform.position);

        }
        else if (currentAction == ShipAction.None)
        {
            if(TurnHandler.instance.currentTeam == targetedShip.team)
            SwitchShip(targetedShip);
            else GameEvents.instance.CamMoveTo(targetedShip.transform.position);
        }
    }

    public void Confirm()
    {
        
        switch (currentAction) {
            case ShipAction.Pass:
                {
                    selectedShip.Pass(1);
                    GridController.instance.UpdateShipPos(selectedShip);
                    SetCurrentAction(ShipAction.None);

                }
                break;
            case ShipAction.Rotate:


                if (pendingDirection != null)
                {
                    selectedShip.Rotate(pendingDirection ?? HexDirection.N);
                    GridController.instance.UpdateShipPos(selectedShip);
                    SetCurrentAction(ShipAction.None, true);
                }
                break;

            case ShipAction.Boost:
                selectedShip.Pass(1);
                GridController.instance.UpdateShipPos(selectedShip);
                SetCurrentAction(ShipAction.None, true);
                break;

            case ShipAction.Evade:
                selectedShip.Pass(1);
                GridController.instance.UpdateShipPos(selectedShip);
                SetCurrentAction(ShipAction.None);
                break;
            
            case ShipAction.DirectTargetShip:
                if (targetedShip == null || targetedShip == selectedShip) return;
                if (selectedWeapon is ITargetsShip w) w.ShootShip(targetedShip);
                SetCurrentAction(ShipAction.None, true);
                selectedShip.Pass(1);
                GridController.instance.UpdateShipPos(selectedShip,0.3f);
                break;

            
        }
        
        selectedShip.shipStatus.isEvading = false;
        prevSpeed = selectedShip.speed;
        prevMoveDir = selectedShip.moveDir;
        selectedShip.positionPreview.Hide();
        selectedShip.GetNextTile();

       
        //GameEvents.instance.RecolorMesh();
        GameEvents.instance.UpdateUI();
    }

    public void CycleShip()
    {
        SwitchShip(shipList.ships[(selectedShip.id + 1) % shipList.ships.Count]);
    }
    public void SwitchShip(Ship ship)
    {
        if (ship.team == shipList.team)
        {
            selectedShip.shipStatus.isEvading = false;
            selectedShip.isSelected = false;
            selectedShip = ship;
            ship.isSelected = true;
            prevSpeed = selectedShip.speed;
            prevMoveDir = selectedShip.moveDir;
            HexCell cell = hexGrid.GetCellAtPos(ship.pos);
            GridController.instance.SetActiveCell(cell);



            shipList.UpdateCards();

            SetCurrentAction(ShipAction.None);

            GameEvents.instance.CamMoveTo(cell.transform.position);

            GameEvents.instance.RecolorMesh();
            GameEvents.instance.DisplayWeapons(ship);
            GameEvents.instance.UpdateUI();
        }
        else
        {
            GameEvents.instance.CamMoveTo(ship.transform.position);
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



    /*
    private void Update()
    {
        if (Time.frameCount % 100 == 0)
        {
            Debug.Log(GridController.instance);
        }
    }
    */
}
