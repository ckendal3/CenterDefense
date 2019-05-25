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

    private GameObject spawnerObject;
    [SerializeField]
    private GameObject spawnVolumeObject;
    private BoxCollider[] spawnVolumes;

    private void Awake()
    {
        spawnVolumes = spawnVolumeObject.GetComponentsInChildren<BoxCollider>();
    }

    private void Start()
    {
        SpawnObject();
    }

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

    private void SpawnObject()
    {
        if(spawnerObject != null)
        {
            Destroy(spawnerObject);
        }

        spawnerObject = new GameObject("Spawner");
        spawnerObject.AddComponent<Spawner_Mono>().colliders = spawnVolumes;
    }
}
