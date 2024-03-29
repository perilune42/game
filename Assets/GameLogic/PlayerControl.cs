using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    public static PlayerControl instance;

    public ControlAction currentAction = ControlAction.None;
    //public PlayerAction pendingAction = PlayerAction.None;
    public Ship selectedShip;
    [SerializeField]
    TMP_Text actionLabel;  //event
    [SerializeField]
    HexGrid hexGrid;
    //List<HexCell> tempColoredCells = new List<HexCell>();
    public ShipList shipList;
    

    public Ship targetedShip;
    Ship lastTargetedShip;
    float lastTargetedShipTime;

    Weapon selectedWeapon;


    public HexDirection? pendingDirection;
    public HexCoordinates? pendingCell;
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


    public bool HasShipsUnderAttack()
    {
        foreach (Ship ship in shipList.activeShips)
        {
            if (ship.shipStatus.IsUnderAttack()) return true;
        }
        return false;
    }

    public void SetCurrentAction(ControlAction newAction, bool keep = false, Weapon weapon = null) //keep: keeping any changes to speed and direction
    {
        if (!TurnHandler.instance.PlayerControllable() && newAction != ControlAction.None) return;
        AttackDisplay.instance.Clear();
        GameEvents.instance.PreviewDamage(null, DamageData.none);
        selectedShip.shipStatus.isEvading = false;

        if ((newAction != currentAction || weapon != null))
        {
            if (selectedWeapon is IRanged k) k.HideRange();
            selectedWeapon = null;
            pendingDirection = null;
            pendingCell = null;
            selectedShip.positionPreview.Hide();
            foreach (var cell in hexGrid.cells)
            {
                cell.isHighlighting = false;
                cell.isTargeted = false;
            }
            //tempColoredCells.Clear();
            if (currentAction != newAction && currentAction != ControlAction.None && !keep) //switching between actions
            {
                selectedShip.speed = prevSpeed; //reset previews
                selectedShip.moveDir = prevMoveDir;
                selectedShip.GetNextTile();
                selectedShip.positionPreview.Hide();
            }
            if (newAction == ControlAction.None)
            {
                actionLabel.text = "";

                

            }
            if (newAction == ControlAction.Pass)
            {
                actionLabel.text = "Pass";
                selectedShip.positionPreview.PreviewAt(selectedShip.GetNextTile(), selectedShip.headingDir);
                hexGrid.GetCellAtPos(selectedShip.GetNextTile()).isHighlighting = true;
            }
            if (newAction == ControlAction.Rotate)
            {
                actionLabel.text = "Rotate";

                hexGrid.GetCellAtPos(HexCoordinates.FromDirection(HexDirection.N) + selectedShip.pos).isHighlighting = true;
                hexGrid.GetCellAtPos(HexCoordinates.FromDirection(HexDirection.NE) + selectedShip.pos).isHighlighting = true;
                hexGrid.GetCellAtPos(HexCoordinates.FromDirection(HexDirection.SE) + selectedShip.pos).isHighlighting = true;
                hexGrid.GetCellAtPos(HexCoordinates.FromDirection(HexDirection.S) + selectedShip.pos).isHighlighting = true;
                hexGrid.GetCellAtPos(HexCoordinates.FromDirection(HexDirection.SW) + selectedShip.pos).isHighlighting = true;
                hexGrid.GetCellAtPos(HexCoordinates.FromDirection(HexDirection.NW) + selectedShip.pos).isHighlighting = true;

            }
            if (newAction == ControlAction.Boost && currentAction != ControlAction.Boost)
            {
                actionLabel.text = "Boost";
                selectedShip.Boost();  //event
                selectedShip.positionPreview.PreviewAt(selectedShip.GetNextTile(), selectedShip.headingDir);
                hexGrid.GetCellAtPos(selectedShip.GetNextTile()).isHighlighting = true;

            }
            if (newAction == ControlAction.Translate)
            {
                actionLabel.text = "Translate";
                HexCoordinates[] translatableCells = selectedShip.GetTranslationAvailableTiles();
                foreach (HexCoordinates cell in translatableCells)
                {
                    hexGrid.GetCellAtPos(cell).isHighlighting = true;
                }
            }

            if (newAction == ControlAction.Evade)
            {
                actionLabel.text = "Evade";
                selectedShip.shipStatus.isEvading = true;
            }
            if (newAction == ControlAction.Brace)
            {
                actionLabel.text = "Brace";
            }

            if (newAction == ControlAction.DirectTargetShip)
            {
                actionLabel.text = "Firing " + weapon.weaponName;
                selectedWeapon = weapon;
                if (weapon is IRanged w) w.DisplayRange();
            }
            if (newAction == ControlAction.DevSpawnShip)
            {
                actionLabel.text = "Select Spawn Position";
            }
            currentAction = newAction;
            targetedShip = null;
            GameEvents.instance.UpdateUI();
            GameEvents.instance.RecolorMesh();
            
        }

    }


    public void InteractCell(HexCoordinates coordinates)
    {

        switch (currentAction) {
            case ControlAction.Rotate:
                if (currentAction == ControlAction.Rotate)
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
                            selectedShip.positionPreview.PreviewAt(selectedShip.GetNextTile(selectedShip.shipStatus.rotateSpeed), targetDirection ?? HexDirection.N);
                        }
                        GameEvents.instance.UpdateUI();
                    }


                }
                return;
            case ControlAction.Pass:
            case ControlAction.Boost:
                if (coordinates == selectedShip.GetNextTile())
                {
                    Confirm();
                }
                return;
            case ControlAction.Translate:
                if (selectedShip.GetTranslationAvailableTiles().Contains(coordinates))
                {
                    if (coordinates == pendingCell && pendingCell != null) Confirm();
                    else
                    {
                        hexGrid.GetCellAtPos(pendingCell ?? coordinates).isTargeted = false;
                        pendingCell = coordinates;
                        hexGrid.GetCellAtPos(coordinates).isTargeted = true;
                        GameEvents.instance.UpdateUI();
                    }
                }
                GameEvents.instance.RecolorMesh();
                return;
            case ControlAction.DevSpawnShip:
                ShipSpawner.instance.SpawnPresetShip(coordinates);
                break;

                
        }
        if (hexGrid.GetCellAtPos(coordinates).containedShip != null &&
            !hexGrid.GetCellAtPos(coordinates).containedShip.isDestroyed)
        {
            targetedShip = hexGrid.GetCellAtPos(coordinates).containedShip;
            TargetShip(targetedShip);
        }
        
    }

    public void TargetShip(Ship targetedShip)
    {
        if (targetedShip == null) return;

        if (Time.time - lastTargetedShipTime < AnimConfig.doubleClickInterval &&
            targetedShip == lastTargetedShip)
        {
            GameEvents.instance.CamMoveTo(targetedShip.transform.position);
        }

        lastTargetedShip = targetedShip;
        lastTargetedShipTime = Time.time;
        

        if (!TurnHandler.instance.PlayerControllable()) return;
        if (currentAction == ControlAction.DirectTargetShip)
        { 
            if (targetedShip != selectedShip)
            {
                this.targetedShip = targetedShip;
                GameEvents.instance.PreviewDamage(null, DamageData.none) ;
                actionLabel.text = "Targeting " + targetedShip.shipName;
                if (selectedWeapon is IHasHitChance h) AttackDisplay.instance.ShowHitChance(
                        h.ChanceToHit(targetedShip, HexCoordinates.Distance(selectedShip.pos, targetedShip.pos), false),
                        h.ChanceToHit(targetedShip, HexCoordinates.Distance(selectedShip.pos, targetedShip.pos), true));
                if (selectedWeapon is ITargetsShip w2)
                {
                    GameEvents.instance.PreviewDamage(targetedShip, w2.GetDamage(targetedShip));
                    AttackDisplay.instance.ShowCritChance(targetedShip.shipStatus.GetCritChance(w2));
                }
                
            }
            

        }
        else if (currentAction == ControlAction.None)
        {
            if (targetedShip != selectedShip)
            {
                this.targetedShip = null;
                SwitchShip(targetedShip);
            }
        }
        GameEvents.instance.UpdateUI();
    }

    public void Confirm()
    {
        
        switch (currentAction) {
            case ControlAction.Pass:
                {
                    selectedShip.PassAction(1);
                    
                    SetCurrentAction(ControlAction.None);

                }
                break;
            case ControlAction.Rotate:


                if (pendingDirection != null)
                {
                    selectedShip.Rotate(pendingDirection ?? HexDirection.N);
                    SetCurrentAction(ControlAction.None, true);
                }
                break;

            case ControlAction.Boost:
                selectedShip.PassAction(1);
                SetCurrentAction(ControlAction.None, true);
                break;
            case ControlAction.Translate:
                selectedShip.Translate(pendingCell ?? selectedShip.pos);
                SetCurrentAction(ControlAction.None);
                break;

            case ControlAction.Evade:
                selectedShip.Evade();
                SetCurrentAction(ControlAction.None);
                break;
            case ControlAction.Brace:
                selectedShip.Brace();
                SetCurrentAction(ControlAction.None);
                break;

            case ControlAction.DirectTargetShip:
                if (targetedShip == null || targetedShip == selectedShip) return;
                if (selectedWeapon is ITargetsShip w) w.ShootShip(targetedShip);
                SetCurrentAction(ControlAction.None, true);
                selectedShip.PassAction(1);
                GridController.instance.UpdateShipPos(selectedShip,0.3f);
                break;

        
        }
        

        selectedShip.shipStatus.isEvading = false;
        prevSpeed = selectedShip.speed;
        prevMoveDir = selectedShip.moveDir;
        selectedShip.positionPreview.Hide();
        selectedShip.GetNextTile();

        if (!selectedShip.ActionAvailable()) GameEvents.instance.LockControls(true);
        else GameEvents.instance.LockControls(false);
        //GameEvents.instance.RecolorMesh();
        GameEvents.instance.UpdateUI();
        //UIButtons.instance.ToggleConfirmButton(false);
    }

    public void CycleShip()
    {
        if (selectedShip.team == TurnHandler.instance.currentTeam)
        {
            for (int i = (selectedShip.id + 1) % shipList.activeShips.Count; i < shipList.activeShips.Count; i++)
            {
                if (shipList.activeShips[i % shipList.activeShips.Count].ActionAvailable()) {
                    SwitchShip(shipList.activeShips[i % shipList.activeShips.Count]);
                    return;
                }
            }
        }
        else if (shipList.activeShips.Count > 0)
        {
            for (int i = 0; i < shipList.activeShips.Count; i++)
            {
                if (shipList.activeShips[i % shipList.activeShips.Count].ActionAvailable())
                {
                    SwitchShip(shipList.activeShips[i % shipList.activeShips.Count]);
                    return;
                }
            }
        }
    }
    public void SwitchShip(Ship ship)
    {
        if (ship != null)
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

            SetCurrentAction(ControlAction.None);


            GameEvents.instance.RecolorMesh();
            GameEvents.instance.DisplayWeapons(ship);
            GameEvents.instance.UpdateUI();
        }
        
        if (ship.team == TurnHandler.instance.currentTeam && TurnHandler.instance.PlayerControllable())
        {
            if (!ship.ActionAvailable()) GameEvents.instance.LockControls(true);
            else GameEvents.instance.LockControls(false);
        }
        else GameEvents.instance.LockControls(true);
    }

    public void SwitchShip(int id)
    {
        SwitchShip(shipList.activeShips[id]);
    }

    void SwitchShipAtPos(HexCoordinates coordinates)
    {
        if (hexGrid.GetCellAtPos(coordinates).containedShip != null)
        {
            SwitchShip(hexGrid.GetCellAtPos(coordinates).containedShip);
        }
    }

   public bool ActionAvailable(ControlAction action)
   { 
        switch (action)
        {
            case ControlAction.Rotate:
            case ControlAction.Boost:
            case ControlAction.Pass:
            case ControlAction.Evade:
            case ControlAction.Translate:
            case ControlAction.Brace:
            case ControlAction.DirectTargetShip:
                return selectedShip.ActionAvailable(action);
            default:
                return false;
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
