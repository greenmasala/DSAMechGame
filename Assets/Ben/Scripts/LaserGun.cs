using System.Collections;
using UnityEngine;

// Long-range power shot. Costs energy instead of ammo.
// Set InputType to Click. Uses Damage, FireRate and Range from GunEquipment.
public class LaserGun : GunEquipment
{
    [Tooltip("Energy used per shot (25 = a quarter of a full bar)")]
    public float EnergyCost = 25f;

    [Tooltip("Optional: LineRenderer used to draw the beam")]
    public LineRenderer Beam;
    public float BeamDuration = 0.1f;

    [Tooltip("Print what the laser does to the console (for testing)")]
    public bool LogToConsole = true;

    StaminaController stamina;

    void Start()
    {
        if (Beam != null) Beam.enabled = false;
    }

    public override void OnUse(Player player)
    {
        if (Time.time < nextFireTime)
        {
            Log("[Laser] Still cooling down");
            return;
        }

        if (stamina == null) stamina = player.GetComponentInChildren<StaminaController>();
        if (stamina == null || !stamina.HasStamina(EnergyCost))
        {
            Log($"[Laser] Not enough energy ({(stamina != null ? stamina.CurrentEnergy : 0):F0}/{EnergyCost})");
            return;
        }

        stamina.UpdateStamina(EnergyCost);
        nextFireTime = Time.time + FireRate;
        Shoot(player);
    }

    void Shoot(Player player)
    {
        Vector3 start = ShootPoint.position;
        Vector3 end = start + ShootPoint.forward * Range;
        string result = "hit nothing";

        if (Physics.Raycast(start, ShootPoint.forward, out RaycastHit hit, Range))
        {
            end = hit.point;
            Entity target = hit.collider.GetComponentInParent<Entity>();
            if (target != null && target != player)
            {
                target.TakeDamage(Damage);
                result = $"hit {target.name} for {Damage}";
            }
            else
            {
                result = $"hit {hit.collider.name} (not an Entity, no damage)";
            }
        }

        Log($"[Laser] FIRED -> {result} | energy left {stamina.CurrentEnergy:F0}");

        Debug.DrawLine(start, end, Color.cyan, 0.5f);
        if (Beam != null) StartCoroutine(ShowBeam(start, end));
    }

    void Log(string message)
    {
        if (LogToConsole) Debug.Log(message);
    }

    IEnumerator ShowBeam(Vector3 start, Vector3 end)
    {
        Beam.SetPosition(0, start);
        Beam.SetPosition(1, end);
        Beam.enabled = true;
        yield return new WaitForSeconds(BeamDuration);
        Beam.enabled = false;
    }
}
