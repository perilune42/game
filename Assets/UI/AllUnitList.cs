using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllUnitList : MonoBehaviour
{
    public List<Ship> allShips;


    // Start is called before the first frame update
    void Start()
    {
        allShips = FindObjectsOfType<Ship>().ToList<Ship>();
        int i = 0;
        foreach (Ship ship in allShips)
        {
            ship.uid = i;
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
