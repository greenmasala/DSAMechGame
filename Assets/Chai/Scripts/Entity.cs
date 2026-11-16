using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected float hp; //serialize for testing purposes
    protected float maxHP;
   
    [SerializeField] protected float moveSpeed;
    public event Action<float> OnHealthChanged;

    private void Awake()
    {
        maxHP = hp;
    }
    public void TakeDamage(float damage)
    {
        hp -= damage;

        OnHealthChanged?.Invoke(hp);

        if (hp <= 0) OnDead();
    }

    public void OnDead()
    {
        Destroy(this.gameObject);
    }
}
