using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    // Start is called before the first frame update
    public HexCoordinates coords;
    HexGrid hexGrid;
    void Start()
    {
        hexGrid = GetComponentInParent<HexGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) {
            Debug.Log(coords.ToString());
            Debug.Log(coords.ToOffsetCoordinates());
            Debug.Log(hexGrid.GetCellAtPos(coords).transform.position);
        }
    }
}
