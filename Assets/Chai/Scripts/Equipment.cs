using UnityEngine;

public class Equipment : MonoBehaviour
{
    public string Name;
    public int Ammo;
    public bool IsUsing;
  
    public EquipmentInputType InputType;

    public virtual void OnUse(Player player) { }
}
