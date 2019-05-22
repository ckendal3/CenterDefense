using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIUpdater : MonoBehaviour
{
    public Text healthText;
    public Health health;

    public void Update()
    {
        healthText.text = Mathf.Clamp((health.currentHealth/ health.MaxHealth) *100, 0, 100).ToString();
    }
}
