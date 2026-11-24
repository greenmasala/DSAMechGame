using System.Collections;
using UnityEngine;

public class ArtilleryGun : GunEquipment
{
    public GameObject AmmoObj;
    
    public override void OnUse(Player player)
    {
        if (CanShoot & Ammo > 0)
        {
            //OnShake(.35f, .5f);
            Recoil();
            CameraShake.Shake(.35f, 1.5f);
            StartCoroutine(ShootWithDelay());
        }
    }

    IEnumerator ShootWithDelay()
    {
        Ammo--;
        CanShoot = false;
        var ammoObj = Instantiate(AmmoObj, ShootPoint.position, ShootPoint.rotation);
        ammoObj.GetComponent<ArtilleryAmmo>().Damage = Damage;
        ammoObj.GetComponent<Rigidbody>().linearVelocity = ShootPoint.transform.up * Power;

        yield return new WaitForSeconds(1.5f);

        CanShoot = true;
    }
}
