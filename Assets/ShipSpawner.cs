using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShipSpawner : MonoBehaviour
{
    public static ShipSpawner instance;

    [SerializeField]
    List<Ship> shipPresets = new List<Ship>();
    [SerializeField] TMP_Dropdown teamDropdown, presetDropdown, directionDropdown;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnPresetShip(HexCoordinates pos)
    {
        Team team = (Team)teamDropdown.value;
        Ship shipToSpawn = shipPresets[presetDropdown.value];

        Debug.Log("Spawning a " + team + " " + shipToSpawn.name + "at " + pos.ToString());
        Ship spawnedShip = Instantiate(shipToSpawn, HexGrid.instance.transform);
        spawnedShip.pos = pos;
        spawnedShip.team = team;

        HexDirection dir = (HexDirection)directionDropdown.value;

        if (dir != HexDirection.None)
        {
            spawnedShip.speed = spawnedShip.thrust;
            spawnedShip.headingDir = dir;
            spawnedShip.moveDir = dir;
        }
        else
        {
            spawnedShip.speed = 0;
            spawnedShip.headingDir = HexDirection.N;
            spawnedShip.moveDir = HexDirection.N;
        }
        

        TurnHandler.instance.enemyShipList.Init(); //inefficient, should optimize
        TurnHandler.instance.playerShipList.Init();
    }

    public void SelectSpawnPostiion()
    {
        PlayerControl.instance.SetCurrentAction(ControlAction.DevSpawnShip);
    }

    void Start()
    {
        presetDropdown.ClearOptions();
        for (int i = 0; i < shipPresets.Count; i++)
        {
            presetDropdown.options.Add(new TMP_Dropdown.OptionData());
            presetDropdown.options[i].text = shipPresets[i].name;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
