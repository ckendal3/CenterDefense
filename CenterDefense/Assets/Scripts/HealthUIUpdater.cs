using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIUpdater : MonoBehaviour
{
    public Text healthText;
    public PlayerHealth pHealth;

    public void Update()
    {
        healthText.text = Mathf.Clamp((pHealth.health / pHealth.maxHealth) *100, 0, 100).ToString();
    }
}
