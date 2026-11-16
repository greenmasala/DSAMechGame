using UnityEngine;

public class Equipment : MonoBehaviour
{
    public string Name;
    public float Damage;
    public int Ammo;
    public float Range; //for raycasts
    public float Power; //for projectiles
    public bool CanShoot;
    public bool SemiAuto;
    public Transform ShootPoint;

    private void Awake()
    {
        CanShoot = true;
        ShootPoint = transform.Find("ShootPoint");
        Debug.Log($"{Name} shootpoint: {ShootPoint.name}");

        if (ShootPoint == null)
        {
            Debug.Log("No ShootPoint found!");
        }
    }

    public virtual void OnUse(Player player) { }
}
