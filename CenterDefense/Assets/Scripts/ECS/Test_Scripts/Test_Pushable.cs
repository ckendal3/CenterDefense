using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Test_Pushable : MonoBehaviour, IPushable
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyImpulseForce(float force, Vector3 position, float radius)
    {
        rb.AddExplosionForce(force, position, radius);
    }
}

public interface IPushable
{
    void ApplyImpulseForce(float force, Vector3 position, float radius);
}

[RequireComponent(typeof(Rigidbody))]
public class Pushable : MonoBehaviour, IPushable
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyImpulseForce(float force, Vector3 position, float radius)
    {
        rb.AddExplosionForce(force, position, radius);
    }
}

