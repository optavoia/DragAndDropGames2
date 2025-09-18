using UnityEngine;

public class FlyingObjectSpawnScript : MonoBehaviour
{
    ScreenBoundriesScript screenBoundriesScript;
    public GameObject[] cludsPrefabs;
    public GameObject[] objectPrefabs;
    public Transform spawnPoint;

    public float cloudsSpawnInterval = 2f;
    public float objectSpawnInterval = 3f;
    private float minY, maxY;
    public float cloudMinSpeed = 1.5f;
    public float cloudMaxSpeed = 150f;
    public float objectMinSpeed = 2f;
    public float objectMaxSpeed = 200f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screenBoundriesScript = FindFirstObjectByType<ScreenBoundriesScript>();
        minY = screenBoundriesScript.minY;
        maxY = screenBoundriesScript.maxY;
        InvokeRepeating(nameof(SpawnCloud), 0f, cloudsSpawnInterval);
        InvokeRepeating(nameof(SpawnObject), 0f, objectSpawnInterval);
    }

    // Update is called once per frame
    void SpawnCloud()
    {
        if(cludsPrefabs.Length == 0)
            return;
        GameObject cloudPrefabs = cludsPrefabs[Random.Range(0, cludsPrefabs.Length)];
        float y = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(spawnPoint.position.x, y, spawnPoint.position.z);
        GameObject cloud = Instantiate(cloudPrefabs, spawnPosition, Quaternion.identity, spawnPoint);
        float movementSpeed = Random.Range(cloudMinSpeed, cloudMaxSpeed);
        FlyeingObjectScript controller = cloud.GetComponent<FlyeingObjectScript>();
        controller.speed = movementSpeed;
    }
    void SpawnObject()
    {
        if (objectPrefabs.Length == 0)
            return;

        GameObject objectPrefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
        float y = Random.Range(minY, maxY);

        Vector3 spawnPosition = 
            new Vector3(-spawnPoint.position.x, y, spawnPoint.position.z);

        GameObject flyingObject = 
            Instantiate(objectPrefab, spawnPosition, Quaternion.identity, spawnPoint);
        float movementSpeed = Random.Range(objectMinSpeed, objectMaxSpeed);
        FlyeingObjectScript controller = flyingObject.GetComponent<FlyeingObjectScript>();
        controller.speed = movementSpeed;
    }
}
