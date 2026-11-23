using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }

    [SerializeField] private Transform entityParent;

    private List<Entity> entities = new List<Entity>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Register(Entity entity)
    {
        entity.transform.SetParent(entityParent);
        entities.Add(entity);
    }

    public void Unregister(Entity entity)
    {
        entities.Remove(entity);
    }
}
