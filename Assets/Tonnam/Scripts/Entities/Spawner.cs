using System;
using UnityEngine;

[Serializable]
public struct SpawnConfig
{
    public Spawner.SpawnType SpawnType;
    public Vector3 Offset;
    public float SpawnRadius;
    public bool RandomizeRotation;
    public Vector3 MinRotation;
    public Vector3 MaxRotation;
}

public class Spawner : MonoBehaviour
{
    public enum SpawnType
    {
        PickRandom,
        All
    }

    public bool IsActive { get; private set; }

    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private SpawnConfig config;
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
        switch (config.SpawnType)
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

        int randomIndex = UnityEngine.Random.Range(0, prefabs.Length);
        GameObject prefabToSpawn = prefabs[randomIndex];

        InternalSpawn(prefabToSpawn);
    }

    private void SpawnAll()
    {
        foreach (var prefab in prefabs)
        {
            InternalSpawn(prefab);
        }
    }

    private void InternalSpawn(GameObject prefab)
    {
        Vector3 spawnPosition = GetSpawnPosition();
        Vector3 spawnRotation = config.RandomizeRotation
            ? new Vector3(
                UnityEngine.Random.Range(config.MinRotation.x, config.MaxRotation.x),
                UnityEngine.Random.Range(config.MinRotation.y, config.MaxRotation.y),
                UnityEngine.Random.Range(config.MinRotation.z, config.MaxRotation.z))
            : Vector3.zero;

        Instantiate(prefab, spawnPosition, Quaternion.Euler(spawnRotation), transform);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition = transform.position + UnityEngine.Random.insideUnitSphere * config.SpawnRadius;
        spawnPosition.y = startPositionY; // Set the y-coordinate to the start position

        Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hitInfo, raycastDistance);

        if (hitInfo.collider != null)
        {
            spawnPosition = hitInfo.point;
        }

        return spawnPosition + config.Offset;
    }
}
