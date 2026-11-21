using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected float hp; //serialize for testing purposes
    protected float maxHP;
   
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected GameObject model;
    public event Action<float> OnHealthChanged;

    protected bool isActive;

    private void Awake()
    {
        maxHP = hp;
    }

    protected virtual void Start()
    {
        EntityManager.Instance.Register(this);
    }

    public void TakeDamage(float damage)
    {
        if (!isActive) return;

        hp -= damage;

        OnHealthChanged?.Invoke(hp);

        if (hp <= 0) OnDead();
    }

    // [Ben] Made this virtual so my Jeep can blow up when it dies instead of just disappearing
    public virtual void OnDead()
    {
        if (!isActive) return;

        EntityManager.Instance.Unregister(this);
        Destroy(this.gameObject);
    }

    protected virtual void Update()
    {
        if (!isActive) return;
    }

    protected virtual void LateUpdate()
    {
        if (MapGenerator.Instance.IsChunkLoaded(transform.position))
        {
            if (!isActive) OnLoaded();
        }
        else
        {
            if (isActive) OnUnloaded();
        }
    }

    protected virtual void OnUnloaded()
    {
        isActive = false;
        if (model != null) model.SetActive(false);
    }

    protected virtual void OnLoaded()
    {
        isActive = true;
        if (model != null) model.SetActive(true);
    }
}
