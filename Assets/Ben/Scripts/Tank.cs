using UnityEngine;

/// Tank enemy (Ben).
[RequireComponent(typeof(Rigidbody))]
public class Tank : RangedEnemy
{
    [SerializeField] protected Transform bottom;
    [SerializeField] protected Transform top;
    [SerializeField] protected Transform gun;
    [SerializeField] protected float acceleration = 1.5f;
    [SerializeField] protected float bottomTurnSpeed = 25f;
    [SerializeField] protected float topTurnSpeed = 30f;
    [SerializeField] protected float tiltSpeed = 15f;
    [SerializeField] protected float maxTiltUp = 30f;
    [SerializeField] protected float maxTiltDown = 15f;
    [SerializeField] protected float aimTolerance = 5f;

    // WW1 jank
    [SerializeField] protected float aimLag = 0.8f;      // seconds the gunner is behind the player
    [SerializeField] protected float aimWobble = 1.5f;   // how far the aim drifts around the player
    [Range(0f, 1f)]
    [SerializeField] protected float jank = 0.6f;        // 0 = smooth, 1 = turns/drives in fits and starts

    protected Rigidbody rb;
    private float currentSpeed;
    private bool inPosition; // close enough, stopped to shoot

    private Vector3 aimTarget; // where the gunner THINKS the player is
    private Vector3 aimTargetVelocity;
    private float noiseSeed;   // so every tank janks differently

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        noiseSeed = Random.Range(0f, 1000f);
    }

    protected override void Start()
    {
        base.Start();
        if (player != null) aimTarget = PlayerAimPoint();
    }

    protected override void Update()
    {
        if (player != null)
        {
            UpdateAimTarget();
            AimTop();
            TiltGun();
        }
        base.Update(); // shooting
    }

    private void FixedUpdate()
    {
        if (rb.isKinematic) return; // Turret: doesn't move

        float targetSpeed = 0f;

        if (player != null && moveSpeed > 0f)
        {
            float distance = FlatDistanceToPlayer();

            // Stop at attackingRange, but only start driving again once the player is a bit further,
            if (distance <= attackingRange) inPosition = true;
            else if (distance > attackingRange * 1.2f) inPosition = false;

            if (!inPosition) targetSpeed = TurnBottomTowardPlayer() * Stutter(30f, 0.5f); // engine surges
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        // Drive the way the Bottom faces. Keep gravity's vertical speed.
        Vector3 forward = bottom != null ? bottom.forward : transform.forward;
        Vector3 velocity = forward * currentSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    // Turns the Bottom toward the player and returns how fast to drive:
    // full speed when facing them, slowing down to 0 when they're 90+ degrees off (turns on the spot).
    private float TurnBottomTowardPlayer()
    {
        if (bottom == null) return moveSpeed;

        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 0.01f) return 0f;

        Quaternion target = Quaternion.LookRotation(toPlayer);
        float turn = bottomTurnSpeed * Stutter(40f, 1f) * Time.fixedDeltaTime;
        bottom.rotation = Quaternion.RotateTowards(bottom.rotation, target, turn);

        float angle = Vector3.Angle(bottom.forward, toPlayer);
        return moveSpeed * Mathf.Clamp01(1f - angle / 90f);
    }

    // Slowly follows the real player (aimLag) with a slow random drift (aimWobble).
    private void UpdateAimTarget()
    {
        Vector3 wobble = new Vector3(Noise(1f) - 0.5f, Noise(2f) - 0.5f, Noise(3f) - 0.5f) * 2f * aimWobble;
        aimTarget = Vector3.SmoothDamp(aimTarget, PlayerAimPoint() + wobble, ref aimTargetVelocity, aimLag);
    }

    // Left/right only. The Gun inside it does the up/down (TiltGun).
    private void AimTop()
    {
        if (top == null) return;

        Vector3 toTarget = aimTarget - top.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude < 0.01f) return;

        Quaternion target = Quaternion.LookRotation(toTarget);
        float turn = topTurnSpeed * Stutter(10f, 1.5f) * Time.deltaTime; // hand-cranked: turns in fits and starts
        top.rotation = Quaternion.RotateTowards(top.rotation, target, turn);
    }

    // Up/down only, within maxTiltUp / maxTiltDown.
    private void TiltGun()
    {
        if (gun == null || top == null) return;

        Vector3 local = top.InverseTransformDirection(aimTarget - gun.position);
        float flat = new Vector2(local.x, local.z).magnitude;
        float upAngle = Mathf.Atan2(local.y, flat) * Mathf.Rad2Deg; // + = target is above
        upAngle = Mathf.Clamp(upAngle, -maxTiltDown, maxTiltUp);

        // Rotating around X: negative tilts the barrel up.
        Quaternion target = Quaternion.Euler(-upAngle, 0f, 0f);
        float turn = tiltSpeed * Stutter(20f, 1.5f) * Time.deltaTime;
        gun.localRotation = Quaternion.RotateTowards(gun.localRotation, target, turn);
    }

    // Fires only when the barrel points at where the gunner thinks the player is.
    // If that's outside the tilt limits (e.g. right next to it), it can't shoot.
    protected override bool IsAimedAtPlayer()
    {
        if (gun != null) return Vector3.Angle(gun.forward, aimTarget - gun.position) <= aimTolerance;
        if (top == null) return true;

        Vector3 toTarget = aimTarget - top.position;
        toTarget.y = 0f;
        return Vector3.Angle(top.forward, toTarget) <= aimTolerance;
    }

    // Bullets go where the gunner is aiming, not straight at the player. That's what makes the lag matter.
    protected override Vector3 AimPoint()
    {
        return aimTarget;
    }

    // Smooth random 0..1 that changes over time. 'channel' picks a different random stream.
    private float Noise(float channel, float rate = 0.7f)
    {
        return Mathf.PerlinNoise(noiseSeed + channel * 10f, Time.time * rate);
    }

    // Speed multiplier: always 1 when jank = 0, randomly dips toward 0 (pauses) when jank = 1.
    private float Stutter(float channel, float rate)
    {
        float burst = Mathf.SmoothStep(0f, 1f, (Noise(channel, rate) - 0.3f) / 0.4f);
        return Mathf.Lerp(1f, burst, jank);
    }
}
