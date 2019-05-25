using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using WDIB.Components;
using WDIB.Parameters;

[RequireComponent(typeof(ConvertToEntity))]
public class ComponentAdder_Helper : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEvent AddComponentsEvent;
    EntityManager eManager;
    Entity newEntity;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        eManager = dstManager;
        newEntity = entity;

        AddComponentsEvent.Invoke();
    }

    #region Entity Naming
    /// <summary>
    /// Set the entity's name using the attached gameObject's name
    /// </summary>
    public void EntityName()
    {
#if UNITY_EDITOR
        eManager.SetName(newEntity, gameObject.name);
#endif
    }

    /// <summary>
    /// Set the entity's name using a passed in string
    /// </summary>
    public void EntityName(string name)
    {
#if UNITY_EDITOR
        eManager.SetName(newEntity, name);
#endif
    }
#endregion

#region Targeting
    /// <summary>
    /// Indicates this is a target
    /// </summary>
    public void DynamicTargetTag() => eManager.AddComponentData(newEntity, new DynamicTargetTag { });

    /// <summary>
    /// Adds components for locking onto a static target
    /// </summary>
    /// <param name="transform"></param>
    public void TargetStaticPosition(Transform transform)
    {
        eManager.AddComponentData(newEntity, new TargetStaticPositionTag { });
        eManager.AddComponentData(newEntity, new TargetPosition { Value = transform.position });
    }

    /// <summary>
    /// Adds components for locking onto Dynamic targets
    /// </summary>
    /// <param name="transform"></param>
    public void TargetDynamicPosition(Transform transform)
    {
        eManager.AddComponentData(newEntity, new TargetDynamicPositionTag { });
        eManager.AddComponentData(newEntity, new TargetPosition { Value = transform.position });
    }
#endregion

    /// <summary>
    /// Set the entity's health component
    /// </summary>
    public void Health(float health) => eManager.AddComponentData(newEntity, new WDIB.Components.Health { Value = health });

    /// <summary>
    /// Set the entity's lifetime component
    /// </summary>
    public void LifeTime(float life) => eManager.AddComponentData(newEntity, new WDIB.Components.LifeTime { Value = life });

    /// <summary>
    /// Set the entity's speed component
    /// </summary>
    public void Speed(float speed) => eManager.AddComponentData(newEntity, new WDIB.Components.Speed { Value = speed });

    /// <summary>
    /// Tag the entity with a PhysicsEntity shared component
    /// </summary>
    public void PhysicsEntityTag() => eManager.AddSharedComponentData(newEntity, new WDIB.Components.PhysicsEntityTag { });

    public void PlayerControlled(int playerID)
    {
        eManager.AddComponentData(newEntity, new Owner { ID = (ushort) playerID });
        eManager.AddComponentData(newEntity, new WDIB.Components.ControlledByPlayer { lookPosition = math.forward(gameObject.transform.rotation) * 100 });
    }

    public void InputComponent(int inputID)
    {
        eManager.AddComponentData(newEntity, new InputComponent { InputPosition = new float2(gameObject.transform.position.x, gameObject.transform.position.y), ID = (ushort) inputID, PressedDown = false });
    }

    #region Combat
    public void WeaponComponent(int weaponId) // maybe use an enum?
    {
        WeaponData data = WeaponParameters.Instance.weaponSet[weaponId];

        eManager.AddComponentData(newEntity, new WeaponComponent {
                                                                    weaponID = weaponId, timeSinceLastBurstRound = data.timeBetweenBurstRounds, timeSinceLastShot = 0,
                                                                    input = new WeaponInput { bReload = false, bShoot = false }
                                                                });
    }

    public void ProjectileSpawnPoint(Transform projectileSpawnTransform)
    {
        Vector3 spawnHeading = projectileSpawnTransform.position - gameObject.transform.position;
        float spawnDistance = spawnHeading.magnitude;

        eManager.AddComponentData(newEntity, new ProjectileSpawnPoint {
                                                                        distance = spawnDistance,
                                                                        heading = spawnHeading,
                                                                        direction = spawnHeading / spawnDistance
        });
    }

    public void TimeBetweenShots(float time)
    {
        eManager.AddComponentData(newEntity, new TimeBetweenShots { resetValue = time, timeLeft = 0 });
    }
    #endregion

    #region LockAxisRotation
    public void LockAxisRotationX()
    {
        if (eManager.HasComponent<LockAxisRotation>(newEntity))
        {
            bool3 bools = eManager.GetComponentData<LockAxisRotation>(newEntity).Value;
            bools.x = true;

            eManager.SetComponentData(newEntity, new LockAxisRotation { Value = bools });
        }
        else
        {
            eManager.AddComponentData(newEntity, new LockAxisRotation { Value = new bool3(true, false, false) });
        }
    }

    public void LockAxisRotationY()
    {
        if (eManager.HasComponent<LockAxisRotation>(newEntity))
        {
            bool3 bools = eManager.GetComponentData<LockAxisRotation>(newEntity).Value;
            bools.y = true;

            eManager.SetComponentData(newEntity, new LockAxisRotation { Value = bools });
        }
        else
        {
            eManager.AddComponentData(newEntity, new LockAxisRotation { Value = new bool3(false, true, false) });
        }
    }

    public void LockAxisRotationZ()
    {
        if (eManager.HasComponent<LockAxisRotation>(newEntity))
        {
            bool3 bools = eManager.GetComponentData<LockAxisRotation>(newEntity).Value;
            bools.z = true;

            eManager.SetComponentData(newEntity, new LockAxisRotation { Value = bools });
        }
        else
        {
            eManager.AddComponentData(newEntity, new LockAxisRotation { Value = new bool3(false, false, true) });
        }
    }
#endregion

#region LockAxisTranslation
    public void LockAxisTranslationX()
    {
        if(eManager.HasComponent<LockAxisTranslation>(newEntity))
        {
            bool3 bools = eManager.GetComponentData<LockAxisTranslation>(newEntity).Value;
            bools.x = true; 

            eManager.SetComponentData(newEntity, new LockAxisTranslation { Value = bools });
        }
        else
        {
            eManager.AddComponentData(newEntity, new LockAxisTranslation { Value = new bool3(true, false, false) });
        }
    }

    public void LockAxisTranslationY()
    {
        if (eManager.HasComponent<LockAxisTranslation>(newEntity))
        {
            bool3 bools = eManager.GetComponentData<LockAxisTranslation>(newEntity).Value;
            bools.y = true;

            eManager.SetComponentData(newEntity, new LockAxisTranslation { Value = bools });
        }
        else
        {
            eManager.AddComponentData(newEntity, new LockAxisTranslation { Value = new bool3(false, true, false) });
        }
    }

    public void LockAxisTranslationZ()
    {
        if (eManager.HasComponent<LockAxisTranslation>(newEntity))
        {
            bool3 bools = eManager.GetComponentData<LockAxisTranslation>(newEntity).Value;
            bools.z = true;

            eManager.SetComponentData(newEntity, new LockAxisTranslation { Value = bools });
        }
        else
        {
            eManager.AddComponentData(newEntity, new LockAxisTranslation { Value = new bool3(false, false, true) });
        }
    }
#endregion

}

