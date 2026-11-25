using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public List<Equipment> EquipmentList;
    public List<Equipment> EquippedEquipments = new List<Equipment>();
    public EquipmentSelectButton[] EquipmentButtons;
    public Player Player;
    public Canvas EquipmentSelectUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = FindFirstObjectByType<Player>();
        GetEquipments();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetEquipments()
    {
        List<Equipment> availablePool = new List<Equipment>();

        foreach (Equipment item in EquipmentList)
        {
            if (!EquippedEquipments.Contains(item))
            {
                availablePool.Add(item);
            }
        }

        //List<Equipment> pickedEquipment = new List<Equipment>();

        for (int i = 0; i < EquipmentButtons.Length; i++)
        {
            int randomIndex = Random.Range(0, availablePool.Count);
            Debug.Log($"GOT EQUIPMENT: " + availablePool.Count);
            EquipmentButtons[i].SetButtonValues(availablePool[randomIndex]);
            availablePool.RemoveAt(randomIndex);
        }
    }

    public void SetEquippedEquipment(int index)
    {

    }
}
