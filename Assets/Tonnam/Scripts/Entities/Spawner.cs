using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum SpawnType
    {
        PickRandom,
        All
    }

    public bool IsActive { get; private set; }

    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private SpawnType spawnType;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private bool activateOnStart = true;

    [SerializeField] private float startPositionY = 300f;
    [SerializeField] private float raycastDistance = 500f;


    private void Start()
    {
        if (activateOnStart)
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
        }
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void Update()
    {
        if (IsActive)
        {
            Spawn();
            Deactivate(); // Deactivate after spawning to prevent continuous spawning
        }
    }

    public void Spawn()
    {
        switch (spawnType)
        {
            case SpawnType.PickRandom:
                SpawnRandom();
                break;
            case SpawnType.All:
                SpawnAll();
                break;
        }
    }

    private void SpawnRandom()
    {
        if (prefabs.Length == 0) return;

        int randomIndex = Random.Range(0, prefabs.Length);
        GameObject prefabToSpawn = prefabs[randomIndex];

        Vector3 spawnPosition = GetSpawnPosition();
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }

    private void SpawnAll()
    {
        foreach (var prefab in prefabs)
        {
            Vector3 spawnPosition = GetSpawnPosition();
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        spawnPosition.y = startPositionY; // Set the y-coordinate to the start position

        Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hitInfo, raycastDistance);

        if (hitInfo.collider != null)
        {
            spawnPosition = hitInfo.point;
        }

        return spawnPosition;
    }
}
