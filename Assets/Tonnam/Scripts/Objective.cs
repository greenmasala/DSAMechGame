using UnityEngine;

public class Objective : Entity
{
    [ContextMenu("Destroy Objective")]
    private void DestroySelf()
    {
        TakeDamage(hp);
    }

    protected override void OnDead()
    {
        if (!isActive) return;

        // GameManager.Instance.ObjectiveDestroyed();
        base.OnDead();
    }
}