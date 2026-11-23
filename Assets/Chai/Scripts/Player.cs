using UnityEngine;

// [Ben] Chai, I changed this to Entity like the class diagram so my Jeep can damage the player
// [Ben] Don't forget to set Hp in the Inspector, it's 0 by default so you die in one hit lol
public class Player : Entity
{
    public Equipment[] Weapons;
    float energy;

    // [Ben] Chai, my ShieldEquipment sets this (0.75 = take 25% damage) while the shield is held up
    [HideInInspector] public float DamageReduction;

    public override void TakeDamage(float damage)
    {
        if (DamageReduction > 0f)
            Debug.Log($"[Shield] Blocked {damage * DamageReduction:F0} of {damage:F0} damage");

        base.TakeDamage(damage * (1f - DamageReduction));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // [Ben] Added the Weapons.Length check to prevent a crash (it crashed my scene, my test player has no weapons)
        if (Input.GetMouseButtonDown(0) && Weapons.Length > 0)
        {
            Weapons[0].OnUse(this);
        }

        if (Input.GetMouseButtonDown(1) && Weapons.Length > 1)
        {
            Weapons[1].OnUse(this);
        }
    }
}
