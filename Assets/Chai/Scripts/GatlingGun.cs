using UnityEngine;
using UnityEngine.Pool;

public class GatlingGun : GunEquipment
{
    float spreadFactor = 2f;
    bool hasShot = false;
    Ray rayForGizmo;

    public override void OnUse(Player player)
    {
        if (Ammo > 0)
        {
           Shoot();
        }
    }

    // Update is called once per frame
    void Shoot()    
    {
        if (Time.time < nextFireTime)
        {
            return;
        }

        Ammo--;
        nextFireTime = Time.time + FireRate;
        RaycastHit hit;

        Debug.DrawRay(ShootPoint.position, ShootPoint.forward * Range, Color.red, 0.5f);
        Debug.Log($"nextFireTime: {nextFireTime} Time.time: {Time.time}");

        if (Time.time >= nextFireTime)
        {
            CanShoot = true;
        }

        Recoil();
        //OnShake(.2f, .05f);
        CameraShake.Shake(.25f, .2f);

        if (Physics.Raycast(ShootPoint.position, ShootPoint.forward, out hit, Range))
        {
            StartCoroutine(SpawnTrail(ShootPoint.position, hit.point, hit.normal, true));

            if (hit.collider.GetComponent<Entity>())
            {
                var hitActor = hit.collider.GetComponent<Entity>();
                Debug.Log("Hit: " + hit.transform.name);
                hitActor.TakeDamage(25);
            }
        }
        else
        {
            StartCoroutine(SpawnTrail(ShootPoint.position, ShootPoint.position + ShootPoint.forward * Range, Vector3.zero, false));
        }
    }
}
