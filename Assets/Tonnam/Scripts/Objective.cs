using UnityEngine;

public class Objective : MonoBehaviour
{
    [ContextMenu("Destroy Objective")]
    private void DestroySelf()
    {
        GameManager.Instance.ObjectiveDestroyed();
        Destroy(gameObject);
    }
}