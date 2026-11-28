using UnityEngine;

public class HealthLoot : Loot
{
    [SerializeField] private int healthAmount = 20;

    protected override void OnCollect(Player player)
    {
        player.Heal(healthAmount);
    }
}
