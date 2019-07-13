using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIUpdater : MonoBehaviour
{
    public Text healthText;

    private void OnEnable()
    {
        GameEvents.onHealthEvent += UpdateText;
    }

    private void OnDisable()
    {
        GameEvents.onHealthEvent -= UpdateText;
    }

    /// <summary>
    /// Update the text of the text component based on a the current value and max value
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maxValue"></param>
    private void UpdateText(float value, float maxValue)
    {
        healthText.text = Mathf.Clamp((value / maxValue) * 100, 0, 100).ToString();
    }
}
