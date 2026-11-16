using UnityEngine;

/// <summary>
/// Straight-flying enemy projectile (Ben). Fired by RangedEnemy (Tank, Turret).
///
/// Moves forward at 'speed' and checks with a raycast each frame instead of using a collider,
/// so even very fast bullets (the Turret's) can't fly through thin walls or the player.
/// Damages the Player only (enemies don't shoot each other) and is destroyed on any hit,
/// or after 'lifetime' seconds if it hits nothing.
///
/// Damage comes from the enemy that fired it (Enemy.damage), so one bullet prefab can be shared.
/// </summary>
public class Bullet : MonoBehaviour
{
    [Tooltip("Units per second.")]
    [SerializeField] private float speed = 30f;

    [Tooltip("Seconds before a bullet that hit nothing is removed.")]
    [SerializeField] private float lifetime = 4f;

    [Tooltip("Spawned where the bullet hits. Leave empty for none.")]
    [SerializeField] private GameObject hitVFX;

    private Entity owner;
    private float damage;

    // [Ben] The class diagram says Initialize(Player). I changed it to take the shooter instead,
    // so the bullet doesn't hit the enemy that fired it, and gets the damage from that enemy.
    public void Initialize(Entity owner, float damage)
    {
        this.owner = owner;
        this.damage = damage;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        float step = speed * Time.deltaTime;

        // Everything between here and where the bullet will be this frame, closest first.
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, step, ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (owner != null && hit.collider.transform.IsChildOf(owner.transform)) continue; // don't hit the shooter
            Hit(hit);
            return;
        }

        transform.position += transform.forward * step;
    }

    private void Hit(RaycastHit hit)
    {
        Player player = hit.collider.GetComponentInParent<Player>();
        if (player != null) player.TakeDamage(damage);

        if (hitVFX != null) Instantiate(hitVFX, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(gameObject);
    }
}
