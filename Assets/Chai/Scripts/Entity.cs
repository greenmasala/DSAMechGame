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

    // [Ben] Made this virtual so my Jeep can blow up when it dies instead of just disappearing
    public virtual void OnDead()
    {
        Destroy(this.gameObject);
    }
}
