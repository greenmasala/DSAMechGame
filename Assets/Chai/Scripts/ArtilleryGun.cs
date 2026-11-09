using System.Collections;
using UnityEngine;

public class ArtilleryGun : Equipment
{
    public GameObject AmmoObj;

    public override void OnUse(Player player)
    {
        if (CanShoot)
        {
            StartCoroutine(ShootWithDelay());
        }
    }

    IEnumerator ShootWithDelay()
    {
        CanShoot = false;
        var ammoObj = Instantiate(AmmoObj, ShootPoint.position, ShootPoint.rotation);
        ammoObj.GetComponent<ArtilleryAmmo>().BulletDamage = Damage;
        ammoObj.GetComponent<Rigidbody>().linearVelocity = ShootPoint.transform.up * Power;

        yield return new WaitForSeconds(1.5f);

        CanShoot = true;
    }
}
