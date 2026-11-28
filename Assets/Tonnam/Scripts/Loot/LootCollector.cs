using UnityEngine;

public class LootCollector : MonoBehaviour
{
    [SerializeField] private Player player;

    private void OnTriggerEnter(Collider other)
    {
        if (player == null) return;

        if (other.TryGetComponent(out Loot loot))
        {
            loot.Collect(player);
        }
    }
}