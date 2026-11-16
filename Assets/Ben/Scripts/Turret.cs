using UnityEngine;

/// <summary>
/// Turret enemy (Ben). A Tank that can't move (as Tonnam said: Tank with moveSpeed 0).
///
/// Stands still, turns its Top toward the player and fires powerful, fast bullets.
/// The "powerful and fast" part is just its settings: high Damage, low Fire Rate,
/// and the TurretBullet prefab (much faster than the Tank's bullet).
/// </summary>
public class Turret : Tank
{
    protected override void Awake()
    {
        moveSpeed = 0f;
        base.Awake();
        rb.isKinematic = true; // can't be pushed around by the player or explosions
    }

    // Keeps Move Speed at 0 in the Inspector too, so nobody wonders why changing it does nothing.
    private void OnValidate()
    {
        moveSpeed = 0f;
    }
}
