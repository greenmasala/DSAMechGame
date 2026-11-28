using UnityEngine;
using UnityEngine.Events;

public abstract class Loot : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float flySpeed = 5f;
    [SerializeField] private float destroyDelay = 0.5f;

    [SerializeField] private UnityEvent OnCollectedEvent;

    protected bool isCollected;

    public virtual void Collect(Player player)
    {
        if (isCollected) return;

        OnCollect(player);
        isCollected = true;

        OnCollectedEvent?.Invoke();
        FlyTowardsLocation(player.transform.position);
        Destroy(gameObject, destroyDelay);
    }

    protected abstract void OnCollect(Player player);

    protected virtual void FlyTowardsLocation(Vector3 target)
    {
        if (rb == null) return;

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.AddForce((target - transform.position).normalized * flySpeed, ForceMode.VelocityChange);
    }

    public void BumpUp(float force)
    {
        if (rb == null) return;

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * force, ForceMode.VelocityChange);
    }
}
