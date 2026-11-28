using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSelectButton : MonoBehaviour
{
    public Equipment Equipment;
    public GameObject EquipmentPrefab;
    public TextMeshProUGUI EquipmentName;
    public TextMeshProUGUI EquipmentDescription;
    public Image EquipmentIcon;
    
    public void SetButtonValues(GameObject equipment)
    {
        if (equipment != null)
        {
            EquipmentPrefab = equipment;
            EquipmentName.text = EquipmentPrefab.GetComponent<Equipment>().Name.ToUpper(); 
            EquipmentDescription.text = EquipmentPrefab.GetComponent<Equipment>().Description;
            EquipmentIcon.sprite = EquipmentPrefab.GetComponent<Equipment>().Icon;
        }
    }
}
