using System.Diagnostics.Metrics;
using UnityEngine;
using UnityEngine.UIElements;

public class ArtilleryAmmo : MonoBehaviour
{
    public float BulletDamage;
    Vector3 position;
    float radius = 5;
    bool hasExploded = false;

    private void OnCollisionEnter(Collision collision)
    {
        var surroundingObjs = Physics.OverlapSphere(transform.position, radius);
        position = collision.GetContact(0).point;
        hasExploded = true;

        foreach (var obj in surroundingObjs)
        {
            Entity entObj = obj.GetComponent<Entity>();
            if (entObj == null) continue;

            entObj.TakeDamage(BulletDamage);
        }

        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        if (!hasExploded) return;

        Gizmos.DrawWireSphere(position, radius);
    }
}
