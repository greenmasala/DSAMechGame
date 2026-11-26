using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    public string Name;
    public string Description;
    public Image Icon;
    public int Ammo;
    public bool IsUsing;
  
    public EquipmentInputType InputType;

    public virtual void OnUse(Player player) { }
}
