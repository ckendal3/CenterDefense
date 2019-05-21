using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject TurretObject;
    public GameObject BarricadeObject;

    [SerializeField]
    public GameObject menuObject;
    private bool menuDisplayed = false;

    private void Update()
    {
        if(BarricadeObject == null && TurretObject == null)
        {
            Debug.Log("Game over");

            if(menuDisplayed == false)
            {
                menuObject.SetActive(true);
                menuDisplayed = true;
            }
        }
    }
}
