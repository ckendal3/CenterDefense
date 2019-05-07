using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : MonoBehaviour 
{
    [SerializeField]
    public Mesh useMesh;

    [SerializeField]
    public Material useMaterial;

    public static MeshManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public Material GetMaterial()
    {
        return useMaterial;
    }

    public Mesh GetMesh()
    {
        return useMesh;
    }

}
