using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public List<GameObject> EquipmentList;
    public List<GameObject> EquippedEquipments = new List<GameObject>();
    public EquipmentSelectButton[] EquipmentButtons;
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
        List<GameObject> availablePool = new List<GameObject>();

        foreach (var item in EquipmentList)
        {
            if (!EquippedEquipments.Contains(item))
            {
                availablePool.Add(item);
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
