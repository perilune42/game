using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    public HexGrid hexGrid;
    public HexMesh hexMesh;
    HexCoordinates lastCellPos = new HexCoordinates(0,0);
    HexCoordinates coordinates = new HexCoordinates(0, 0);

    public UnityEvent TriggerCycleShip;

    Ship ship;
    PlayerControl playerControl;
    TurnHandler turnHandler;
    // Start is called before the first frame update
    void Awake()
    {
        playerControl = PlayerControl.instance;
        ship = playerControl.selectedShip;
        turnHandler = TurnHandler.instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit) && (!EventSystem.current.IsPointerOverGameObject()))
        {

            InputHover(hit.point);
            if (Input.GetMouseButtonDown(0))
            {
                InputClick(hit.point);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ship.UseAction();
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ship._Debug();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            ship.Boost(ship.accel, 1);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            ship.Boost(ship.accel, 2);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            playerControl.SetCurrentAction(PlayerAction.None);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TriggerCycleShip.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            turnHandler.CycleTeam();
        }
    }
    void InputHover(Vector3 position)
    {
        
        position = transform.InverseTransformPoint(position);
          
        if (HexCoordinates.FromWorldPosition(position) != new HexCoordinates(-1, -1))
        {
            coordinates = HexCoordinates.FromWorldPosition(position);
        }
        HexCell cell = hexGrid.GetCellAtPos(coordinates);
        if (cell != null)
        {
            HoverCell(cell);
        }
    }

    void HoverCell(HexCell cell)
    {
        //highlight hovered cell
        if (cell.containedShip != playerControl.selectedShip) cell.ColorHover();

        if (lastCellPos.ToOffsetCoordinates() != coordinates.ToOffsetCoordinates())
        {
            if (hexGrid.GetCellAtPos(lastCellPos).color == hexGrid.hoverColor) hexGrid.GetCellAtPos(lastCellPos).RevertColor();
            lastCellPos = coordinates;
        }
        //hexMesh.RecolorMesh();
        GameEvents.instance.RecolorMesh();
    }
    void InputClick(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);

        if (HexCoordinates.FromWorldPosition(position) != new HexCoordinates(-1, -1))
        {
            coordinates = HexCoordinates.FromWorldPosition(position);
        }
        HexCell cell = hexGrid.GetCellAtPos(coordinates);
        ClickCell(cell);
    }
    void ClickCell(HexCell cell)
    {
        playerControl.InteractCell(cell.coordinates);
        //Debug.Log("clicked cell");
    }


}
