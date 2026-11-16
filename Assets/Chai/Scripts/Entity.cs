using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected float hp; //serialize for testing purposes
    [SerializeField] protected float moveSpeed;

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0) OnDead();
    }

    public void OnDead()
    {
        Destroy(this.gameObject);
    }
}
