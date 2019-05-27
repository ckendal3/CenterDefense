using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using WDIB.Components;

public class Test_GameTimers : MonoBehaviour
{
    private EntityManager eManager;

    [SerializeField]
    public ECounterType countType;

    [SerializeField]
    public float countDownTime = 40;

    private void Awake()
    {
        eManager = World.Active.EntityManager;
    }

    private void Start()
    {
        CreateEntity();
        DestroySelf();
    }

    public void CreateEntity()
    {
        Entity newEnt = eManager.CreateEntity();
        switch (countType)
        {
            case ECounterType.CountUp:
                eManager.AddComponentData(newEnt, new CountUpTime { Value = 0, maxDuration = countDownTime });
                break;
            case ECounterType.CountDown:
                eManager.AddComponentData(newEnt, new CountDownTime { Value = countDownTime });
                break;
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}

public enum ECounterType
{
    CountUp,
    CountDown
}