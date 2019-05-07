using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTouch : MonoBehaviour
{
    Vector3 inputPos, worldInputPos;
    private bool bInputReceived;
    Camera cam;

    LineRenderer lR;
    [SerializeField]
    Material lineMaterial;

    [SerializeField]
    public Transform bulletSpawn;

    // Start is called before the first frame update
    void Start()
    {
        bInputReceived = false;

        lR = gameObject.AddComponent<LineRenderer>();
        lR.startWidth = .1f;
        lR.endWidth = .1f;

        if(lineMaterial)
        {
            lR.material = lineMaterial;
        }

        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            bInputReceived = true;

            inputPos = Input.mousePosition;           
        }

        // check if touchinput received and apply inputPos

        if(bInputReceived)
        {
            bInputReceived = false;

            worldInputPos = GetInputHitPoint(inputPos);

            // if we didnt hit anything, return
            if(worldInputPos == Vector3.zero)
            {
                return;
            }

            LookAtPosition(new Vector3(worldInputPos.x, gameObject.transform.position.y, worldInputPos.z));
        }
    }

    public Vector3 GetInputHitPoint(Vector3 pos)
    {
        Ray ray = cam.ScreenPointToRay(inputPos);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    public void LookAtPosition(Vector3 lookPos)
    {
        gameObject.transform.LookAt(lookPos);

        DebugKiller();
    }

    public void DebugKiller()
    {
        Ray ray = new Ray(bulletSpawn.position, gameObject.transform.forward);
        RaycastHit hit;

        lR.positionCount = 2;

        // should spawn projectile instead
        if (Physics.Raycast(ray, out hit, 500f))
        {
            Debug.Log("Hit: " + hit.collider.name);
            Health healthComp = hit.collider.gameObject.GetComponent<Health>();

            if(healthComp)
            {
                healthComp.TakeDamage(25);
            }

            lR.SetPosition(0, ray.origin);
            lR.SetPosition(1, hit.point);


            Debug.DrawLine(ray.origin, hit.point, Color.green, 10f);
            return;
        }

        lR.SetPosition(0, ray.origin);
        lR.SetPosition(1, ray.direction * 50f);

        Debug.DrawLine(ray.origin, gameObject.transform.forward * 50, Color.red, 10f);   
    }
}
