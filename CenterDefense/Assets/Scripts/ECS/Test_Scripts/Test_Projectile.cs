using UnityEngine;

public class Test_Projectile : MonoBehaviour
{
    [SerializeField]
    public float timeBetweenSpawns = 1f;
    private float timeTillNextSpawn = 0;

    [SerializeField]
    public ushort projectileID = 0;

    // Start is called before the first frame update
    void Start()
    {
        timeTillNextSpawn = timeBetweenSpawns;
    }

    // Update is called once per frame
    void Update()
    {
        timeTillNextSpawn -= Time.deltaTime;

        if (timeTillNextSpawn < 0)
        {
            timeTillNextSpawn = timeBetweenSpawns;

            ProjectileFactory.CreateProjectiles(projectileID, transform.position, transform.rotation, 0);
        }
    }
}