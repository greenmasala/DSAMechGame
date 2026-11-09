using UnityEngine;

public class GatlingGun : Equipment
{
    float spreadFactor = 2f;
    bool hasShot = false;
    Ray rayForGizmo;

    public override void OnUse(Player player)
    {
        Shoot();
    }

    // Update is called once per frame
    void Shoot()
    {
        RaycastHit hit;

        Debug.DrawRay(ShootPoint.position, ShootPoint.forward * Range, Color.red, 0.5f);

        if (Physics.Raycast(ShootPoint.position, ShootPoint.forward, out hit, Range))
        {
            if (hit.collider.GetComponent<Entity>())
            {
                var hitActor = hit.collider.GetComponent<Entity>();
                Debug.Log("Hit: " + hit.transform.name);
                hitActor.TakeDamage(25);
            }
        }
    }
}
