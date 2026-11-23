using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    public Equipment[] Weapons;
    public TextMeshProUGUI[] EquipmentAmmo;
    public TextMeshProUGUI HealthText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateText(0);
        UpdateText(1);
        UpdateHealthText();
    }

    // Update is called once per frame
    void Update()
    {
        UseEquipment(0, KeyCode.Mouse0);
        UseEquipment(1, KeyCode.Mouse1);
    }

    private void OnEnable()
    {
        OnHealthChanged += UpdateHealth;
    }

    private void OnDisable()
    {
        OnHealthChanged -= UpdateHealth;
    }

    private void UpdateHealth(float hp)
    {
        HealthText.text = hp.ToString("F0");
    }

    public void UseEquipment(int index, KeyCode key)
    {
        switch (Weapons[index].InputType)
        {
            case EquipmentInputType.Click:

                InputReceiver(Input.GetKeyDown(key), index);
                break;

            case EquipmentInputType.Hold:

                InputReceiver(Input.GetKey(key), index);
                //if (Input.GetKey(key))
                //{
                //    Weapons[index].OnUse(this);
                //    UpdateText(index);
                //}

                break;

            case EquipmentInputType.Release:

                InputReceiver(Input.GetKeyUp(key), index);
                //if (Input.GetKey(key))
                //{
                //    Weapons[index].OnUse(this);
                //    UpdateText(index);
                //}

                break;
        }
    }

    public void UpdateText(int index)
    {
        EquipmentAmmo[index].text = Weapons[index].Ammo.ToString();
    }

    public void UpdateHealthText()
    {
        HealthText.text = hp.ToString();
    }

    void InputReceiver(bool input, int index)
    {
        if (input)
        {
            Weapons[index].OnUse(this);
            Weapons[index].IsUsing = true;
            UpdateText(index);
        }
        else if (Weapons[index].IsUsing)
        {
            Weapons[index].IsUsing = false;
        
            Weapons[index].gameObject.GetComponent<GunEquipment>().MuzzleFlashController(false);
        }
    }
}
