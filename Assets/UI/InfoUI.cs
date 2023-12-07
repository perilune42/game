using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoUI : MonoBehaviour
{
    [SerializeField] TMP_Text popupLabel;
    [SerializeField] TMP_Text commandPointLabel;

    private void Awake()
    {
        popupLabel = GetComponent<TMP_Text>();
        GameEvents.instance.onDisplayPopup += StartPopup;
        GameEvents.instance.onUpdateUI += UpdateLabels;
    }

    void UpdateLabels()
    {
        commandPointLabel.text = TurnHandler.instance.playerCommandPoints.ToString() + " / " + GameConfig.playerCommandPoints;
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
