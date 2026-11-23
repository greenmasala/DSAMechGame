using System.Diagnostics.Metrics;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.UIElements;

public class ArtilleryAmmo : Ammo
{
    float radius = 5;
    bool hasExploded = false;

    private void OnCollisionEnter(Collision collision)
    {
        var surroundingObjs = Physics.OverlapSphere(transform.position, radius);
        hasExploded = true; 
        impactPoint = collision.GetContact(0).point;

        if (ImpactVFX != null)
        {
            Debug.Log("done???");
            Instantiate(ImpactVFX, impactPoint, Quaternion.identity);
        }

        foreach (var obj in surroundingObjs)
        {
            Entity entObj = obj.GetComponent<Entity>();
            if (entObj == null) continue;

            entObj.TakeDamage(Damage);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (!hasExploded) return;

        Gizmos.DrawWireSphere(impactPoint, radius);
    }
}
