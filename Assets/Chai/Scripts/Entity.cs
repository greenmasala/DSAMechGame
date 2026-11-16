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

    // [Ben] Made this virtual so my Jeep can blow up when it dies instead of just disappearing
    public virtual void OnDead()
    {
        Destroy(this.gameObject);
    }
}
