using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected float hp; //serialize for testing purposes
    [SerializeField] protected float moveSpeed;

    // [Ben] Made this virtual so Player can cut damage while my ShieldEquipment is up
    public virtual void TakeDamage(float damage)
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
