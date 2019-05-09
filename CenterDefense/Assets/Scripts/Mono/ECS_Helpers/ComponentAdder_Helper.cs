using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Events;
using WDIB.Components;

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
    public void EntityName() => eManager.SetName(newEntity, gameObject.name);
    /// <summary>
    /// Set the entity's name using a passed in string
    /// </summary>
    public void EntityName(string name) => eManager.SetName(newEntity, name);
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
}
