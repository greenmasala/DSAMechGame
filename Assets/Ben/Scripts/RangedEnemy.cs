using UnityEngine;

/// <summary>
/// Base class for enemies that shoot (Ben). Tank and Turret extend this.
/// </summary>
public class RangedEnemy : Enemy
{
    [SerializeField] protected float fireRadius = 25f;
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected Bullet bulletPrefab;
    [SerializeField] protected Transform muzzle;
    [SerializeField] protected SoundType fireSound = SoundType.None;

    private float nextFireTime;
    private Collider playerCollider;

    protected override void Start()
    {
        base.Start();
        if (player != null) playerCollider = player.GetComponent<Collider>();
    }

    protected virtual void Update()
    {
        if (player == null) return;

        if (IsPlayerInRange() && IsAimedAtPlayer() && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Fire();
        }
    }

    protected bool IsPlayerInRange()
    {
        return FlatDistanceToPlayer() <= fireRadius;
    }

    protected virtual bool IsAimedAtPlayer()
    {
        return true;
    }

    protected void Fire()
    {
        if (bulletPrefab == null) return;

        Vector3 origin = muzzle != null ? muzzle.position : transform.position + Vector3.up;
        Quaternion aim = Quaternion.LookRotation(AimPoint() - origin);

        Bullet bullet = Instantiate(bulletPrefab, origin, aim);
        bullet.Initialize(this, damage);

        if (fireSound != SoundType.None && AudioManager.Instance != null) AudioManager.Instance.PlayOneShotSFX(fireSound);
    }

    // Where bullets are shot at. Default: straight at the player. Tank overrides this to aim with lag.
    protected virtual Vector3 AimPoint()
    {
        return PlayerAimPoint();
    }

    // The middle of the player's body. The player's pivot is at its feet, so aiming at it would shoot the ground.
    protected Vector3 PlayerAimPoint()
    {
        if (playerCollider != null) return playerCollider.bounds.center;
        return player.transform.position + Vector3.up;
    }

    protected float FlatDistanceToPlayer()
    {
        Vector3 diff = player.transform.position - transform.position;
        diff.y = 0f;
        return diff.magnitude;
    }

    // Scene view helpers: orange = fire radius, yellow = attacking range.
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, fireRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackingRange);
    }
}
