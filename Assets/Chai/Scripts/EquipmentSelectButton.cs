using TMPro;
using UnityEngine;

public class EquipmentSelectButton : MonoBehaviour
{
    public Equipment Equipment;
    public GameObject EquipmentPrefab;
    public TextMeshProUGUI EquipmentName;
    public TextMeshProUGUI EquipmentDescription;
    
    public void SetButtonValues(GameObject equipment)
    {
        if (equipment != null)
        {
            EquipmentPrefab = equipment;
            EquipmentName.text = EquipmentPrefab.GetComponent<Equipment>().Name; 
            EquipmentDescription.text = EquipmentPrefab.GetComponent<Equipment>().Description;
        }
    }
}
