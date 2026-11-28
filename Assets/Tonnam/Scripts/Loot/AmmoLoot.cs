using UnityEngine;

public class AmmoLoot : Loot
{
    [SerializeField] private int ammoAmount = 10;

    protected override void OnCollect(Player player)
    {
        // TODO: imlpement ammo collection logic
        // player.AddAmmo(ammoAmount);
    }
}
