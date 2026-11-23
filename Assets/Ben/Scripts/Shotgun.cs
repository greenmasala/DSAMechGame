using UnityEngine;

// Close-range weapon: fires several pellets (raycasts) in a cone.
// Set InputType to Click. Damage is per pellet; keep Range short (e.g. 15).
public class Shotgun : GunEquipment
{
    [Tooltip("Number of raycasts fired per shot")]
    public int Pellets = 8;

    [Tooltip("Cone half-angle in degrees")]
    public float SpreadAngle = 8f;

    [Tooltip("Print what the shotgun does to the console (for testing)")]
    public bool LogToConsole = true;

    public override void OnUse(Player player)
    {
        if (Ammo <= 0)
        {
            Log("[Shotgun] Out of ammo");
            return;
        }
        if (Time.time < nextFireTime)
        {
            Log("[Shotgun] Still cooling down");
            return;
        }

        Ammo--;
        nextFireTime = Time.time + FireRate;

        int hits = 0;
        string lastHit = "";
        for (int i = 0; i < Pellets; i++)
        {
            Entity target = FirePellet(player);
            if (target != null)
            {
                hits++;
                lastHit = target.name;
            }
        }

        string result = hits > 0 ? $"{hits}/{Pellets} pellets hit {lastHit} ({hits * Damage} dmg)" : "all pellets missed";
        Log($"[Shotgun] FIRED -> {result} | ammo left {Ammo}");
    }

    // Returns the Entity the pellet damaged, or null if it missed
    Entity FirePellet(Player player)
    {
        // Random direction inside the cone
        Vector2 offset = Random.insideUnitCircle * SpreadAngle;
        Vector3 dir = Quaternion.AngleAxis(offset.x, ShootPoint.up)
                    * Quaternion.AngleAxis(offset.y, ShootPoint.right)
                    * ShootPoint.forward;

        Debug.DrawRay(ShootPoint.position, dir * Range, Color.yellow, 0.5f);

        if (Physics.Raycast(ShootPoint.position, dir, out RaycastHit hit, Range))
        {
            Entity target = hit.collider.GetComponentInParent<Entity>();
            if (target != null && target != player)
            {
                target.TakeDamage(Damage);
                return target;
            }
        }
        return null;
    }

    void Log(string message)
    {
        if (LogToConsole) Debug.Log(message);
    }
}
