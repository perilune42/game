using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    TMP_Text popupLabel;

    private void Awake()
    {
        popupLabel = GetComponent<TMP_Text>();
        GameEvents.instance.onDisplayPopup += StartPopup;
    }

    void StartPopup(string message)
    {
        StartCoroutine(DisplayPopup(message));
    }
    IEnumerator DisplayPopup(string message)
    {
        popupLabel.text = message;
        yield return new WaitForSeconds(2);
        popupLabel.text = string.Empty;
    }
}
