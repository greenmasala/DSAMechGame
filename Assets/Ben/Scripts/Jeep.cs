using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Suicide car-bomb enemy (Ben).
///
/// CHASE   -> drives like a car toward the player (forward only, turns in arcs).
/// ARMED   -> player came within attackingRange: brakes, beeps and flashes for fuseTime seconds.
/// EXPLODE -> damages every Entity within explodeDistance, then destroys itself.
///
/// Killing the Jeep at any time makes it explode right away (it's a car bomb).
/// Explosions also hurt other Jeeps, so they can chain-react.
///
/// Setup: Rigidbody + Collider on the root, visuals as children (see Assets/Ben/README.md).
/// Moving speed comes from Entity.moveSpeed, damage and arm distance come from Enemy.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Jeep : Enemy
{
    private enum State { Chase, Armed, Exploded }

    [Header("Driving")]
    [Tooltip("How fast the Jeep speeds up (units per second, per second). Top speed is 'Move Speed'.")]
    [SerializeField] private float acceleration = 8f;

    [Tooltip("How fast the Jeep can turn at top speed (degrees per second). It turns slower when driving slowly and can't turn when stopped, like a real car.")]
    [SerializeField] private float turnSpeed = 90f;

    [Tooltip("How quickly the Jeep stops once armed.")]
    [SerializeField] private float brakeForce = 15f;

    [Header("Bomb")]
    [Tooltip("Seconds between arming (player within 'Attacking Range') and exploding. This is the player's time to escape.")]
    [SerializeField] private float fuseTime = 2f;

    [Tooltip("Blast radius of the explosion. Keep it bigger than 'Attacking Range' so running away matters.")]
    [SerializeField] private float explodeDistance = 6f;

    [Header("Effects (optional)")]
    [Tooltip("Spawned when the Jeep explodes. Leave empty for none.")]
    [SerializeField] private GameObject explosionVFX;

    [Tooltip("Renderers that flash red while armed (e.g. a roof light on the real model). If empty, the whole Jeep model flashes.")]
    [SerializeField] private Renderer[] warningLights;

    [SerializeField] private Color warningColor = Color.red;

    private State state = State.Chase;
    private Rigidbody rb;
    private float currentSpeed;
    private float fuseTimer;
    private float nextBeepTime;

    // Original colors so the flash can switch back.
    private readonly List<Material> flashMaterials = new List<Material>();
    private readonly List<Color> originalColors = new List<Color>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Cars don't tip over or spin from bumps; we steer it ourselves.
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Renderer[] renderers = warningLights != null && warningLights.Length > 0
            ? warningLights
            : GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            flashMaterials.Add(r.material);
            originalColors.Add(r.material.color);
        }
    }

    private void Update()
    {
        if (state == State.Chase && player != null && FlatDistanceToPlayer() <= attackingRange)
        {
            Arm();
        }

        if (state == State.Armed)
        {
            UpdateFuse();
        }
    }

    private void FixedUpdate()
    {
        if (state == State.Chase && player != null)
        {
            DriveTowardPlayer();
        }
        else
        {
            // Armed, or no player: roll to a stop.
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeForce * Time.fixedDeltaTime);
        }

        // Keep gravity's vertical speed, replace the horizontal part with "drive forward".
        Vector3 forwardVelocity = transform.forward * currentSpeed;
        rb.linearVelocity = new Vector3(forwardVelocity.x, rb.linearVelocity.y, forwardVelocity.z);
    }

    private void DriveTowardPlayer()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0f;

        // How far we need to turn: -180..180 degrees (negative = left).
        float angle = Vector3.SignedAngle(transform.forward, toPlayer, Vector3.up);

        // Slow down for sharp turns (down to 40% of top speed when the player is behind us).
        float targetSpeed = moveSpeed * Mathf.Lerp(1f, 0.4f, Mathf.Abs(angle) / 180f);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        // Turning needs speed: a stopped car can't steer.
        float speedPercent = moveSpeed > 0f ? currentSpeed / moveSpeed : 0f;
        float maxTurn = turnSpeed * speedPercent * Time.fixedDeltaTime;
        float turn = Mathf.Clamp(angle, -maxTurn, maxTurn);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
    }

    private void Arm()
    {
        state = State.Armed;
        fuseTimer = fuseTime;
        nextBeepTime = 0f;
    }

    private void UpdateFuse()
    {
        fuseTimer -= Time.deltaTime;

        if (fuseTimer <= 0f)
        {
            Explode();
            return;
        }

        // Beep faster as the fuse runs out: every 0.5s at the start, every 0.1s at the end.
        float progress = 1f - fuseTimer / fuseTime;
        float beepInterval = Mathf.Lerp(0.5f, 0.1f, progress);

        if (Time.time >= nextBeepTime)
        {
            nextBeepTime = Time.time + beepInterval;
            if (AudioManager.Instance != null) AudioManager.Instance.PlayOneShotSFX(SoundType.Jeep_Beep);
        }

        // Light is on for the first half of each beep.
        bool lightOn = nextBeepTime - Time.time > beepInterval * 0.5f;
        SetFlash(lightOn);
    }

    private void SetFlash(bool on)
    {
        for (int i = 0; i < flashMaterials.Count; i++)
        {
            flashMaterials[i].color = on ? warningColor : originalColors[i];
        }
    }

    private void Explode()
    {
        if (state == State.Exploded) return; // only explode once (matters for chain reactions)
        state = State.Exploded;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayOneShotSFX(SoundType.Jeep_Explode);
        if (explosionVFX != null) Instantiate(explosionVFX, transform.position, Quaternion.identity);

        // One Entity can have several colliders, so collect each Entity once.
        HashSet<Entity> hit = new HashSet<Entity>();
        foreach (Collider col in Physics.OverlapSphere(transform.position, explodeDistance))
        {
            Entity entity = col.GetComponentInParent<Entity>();
            if (entity != null && entity != this) hit.Add(entity);
        }

        foreach (Entity entity in hit)
        {
            if (entity != null) entity.TakeDamage(damage); // may be destroyed by an earlier chain explosion
        }

        Destroy(gameObject);
    }

    // Shot to death -> blow up instead of just disappearing.
    public override void OnDead()
    {
        Explode();
    }

    private float FlatDistanceToPlayer()
    {
        Vector3 diff = player.transform.position - transform.position;
        diff.y = 0f;
        return diff.magnitude;
    }

    // Scene view helpers: yellow = arm distance, red = blast radius.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackingRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeDistance);
    }
}
