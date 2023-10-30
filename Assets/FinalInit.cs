using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerControl.instance.SwitchShip(0);
    }

}
