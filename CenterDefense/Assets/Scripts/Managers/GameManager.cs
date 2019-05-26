using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int stageIndex = 1;

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
        StartNewStage();
    }

    private void Update()
    {
        if (BarricadeObject == null && TurretObject == null)
        {
            Debug.Log("Game over");

            if (menuDisplayed == false)
            {
                menuObject.SetActive(true);
                menuDisplayed = true;

//#if UNITY_EDITOR
//                World.Active.EntityManager.DestroyEntity(World.Active.EntityManager.GetAllEntities());
//#endif
            }
        }
        else if (spawnerObject == null)
        {
            StartNewStage();
        }
    }

    private void StartNewStage()
    {
        SpawnObject();
        stageIndex += 1;
    }

    private void SpawnObject()
    {
        if(spawnerObject != null)
        {
            Destroy(spawnerObject);
        }
        Spawner_Mono spawner;


        spawnerObject = new GameObject("Spawner");
        spawnerObject.AddComponent<Spawner_Mono>().Initialize(stageIndex, spawnVolumes);
    }
}
