using TMPro;
using UnityEngine;

public class EquipmentSelectButton : MonoBehaviour
{
    public Equipment Equipment;
    public TextMeshProUGUI EquipmentName;
    public TextMeshProUGUI EquipmentDescription;
    
    public void SetButtonValues(Equipment equipment)
    {
        if (equipment != null)
        {
            EquipmentName.text = equipment.Name;
        }
    }

    public void SetValue()
    {

    }
}
