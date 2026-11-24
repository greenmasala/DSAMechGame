using UnityEngine;

public class EquipmentRetract : MonoBehaviour
{
    public Transform weaponPivot; // Assign your child gun object here
    public float checkDistance = 1.1f;
    public float maxRetract = 0.35f; // 3.3f is 3.3 meters (way behind the player!)
    public LayerMask wallLayers; // Uncheck your Player and Weapon layers in Inspector

    private Vector3 ogPos;

    void Start()
    {
        ogPos = weaponPivot.localPosition;
    }

    void Update()
    {
        Vector3 targetPos = ogPos;

        Debug.DrawRay(transform.position, transform.forward * checkDistance, Color.white);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, checkDistance, wallLayers, QueryTriggerInteraction.Ignore))
        {
            Debug.Log($"Hit {hit.collider.name} at {hit.distance}m");
            float penetration = Mathf.Clamp01(1f - (hit.distance / checkDistance));
            targetPos = ogPos - Vector3.forward * (penetration * maxRetract);
            targetPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + targetPos.z);
        }

        weaponPivot.localPosition = Vector3.Lerp(weaponPivot.localPosition, targetPos, Time.deltaTime * 12f);
    }
}
