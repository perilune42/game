using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlButton : MonoBehaviour
{
    public Button button;
    public ControlAction action;
    private void Awake()
    {
        button = GetComponent<Button>();
    }
}
