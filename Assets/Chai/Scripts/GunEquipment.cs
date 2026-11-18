using System.Xml.Linq;
using UnityEngine;

public class GunEquipment : Equipment
{
    public Transform ShootPoint;
    public float Damage;
    public float FireRate;
    protected float nextFireTime;
    public float Range; //for raycasts
    public float Power; //for projectiles
    public bool CanShoot;

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
}
