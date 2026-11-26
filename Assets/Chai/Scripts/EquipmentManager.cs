using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public List<GameObject> EquipmentList;
    public EquipmentSelectButton[] EquipmentButtons;
    public Equipment[] EquippedEquipment;
    GameObject selectedEquipment;
    public Player Player;
    public GameObject EquipmentSelectUI;
    public GameObject SlotSelectUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = FindFirstObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GetEquipment();
        }
    }

    public void GetEquipment()
    {
        EquippedEquipment = Player.Weapons; //need to fix
        List<GameObject> availablePool = new List<GameObject>();

        foreach (var item in EquipmentList)
        {
            if (!EquippedEquipment.Contains(item.GetComponent<Equipment>()))
            {
                availablePool.Add(item);
            }
            else
            {
                Debug.Log($"{item} already equipped");
            }
        }

        for (int i = 0; i < EquipmentButtons.Length; i++)
        {
            int randomIndex = Random.Range(0, availablePool.Count);
            Debug.Log($"GOT EQUIPMENT: " + availablePool.Count);
            EquipmentButtons[i].SetButtonValues(availablePool[randomIndex]);
            availablePool.RemoveAt(randomIndex);
        }

        ShowEquipmentSelect();
    }

    public void ShowEquipmentSelect()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        EquipmentSelectUI.SetActive(!EquipmentSelectUI.activeInHierarchy);
        SlotSelectUI.SetActive(false);
    }

    public void ShowSlotSelect()
    {
        SlotSelectUI.SetActive(!SlotSelectUI.activeInHierarchy);
    }

    public void SetEquipment(int index)
    {
        selectedEquipment = EquipmentButtons[index].EquipmentPrefab;

        ShowSlotSelect();
    }

    public void SetSlot(int index)
    {
        Transform child = Player.Slots[index].GetChild(0);
        Destroy(child.gameObject);
        GameObject newWeapon = Instantiate(selectedEquipment, Player.Slots[index]);
        newWeapon.transform.position = Player.Slots[index].transform.position;
        //newWeapon.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        Player.Weapons[index] = newWeapon.GetComponent<Equipment>();
        Player.UpdateText(index);

        ShowEquipmentSelect();
        ShowSlotSelect();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
