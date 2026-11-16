using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    public Equipment[] Weapons;
<<<<<<< Updated upstream
=======
    public TextMeshProUGUI[] EquipmentAmmo;
    public TextMeshProUGUI HealthText;
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
        if (Input.GetMouseButton(0))
        {
            Debug.Log("LAUNCHING");
            Weapons[0].OnUse(this);
        }
        //else if (Input.GetMouseButtonDown(0) & !Weapons[0].SemiAuto)
        //{
        //    Weapons[0].OnUse(this);
        //}

        if (Input.GetMouseButton(1) & Weapons[1].SemiAuto)
        {
            Weapons[1].OnUse(this);
        }
        else if (Input.GetMouseButtonDown(1) & !Weapons[1].SemiAuto)
=======
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
        HealthText.text = hp.ToString("F2");
    }

    public void UseEquipment(int index, KeyCode key)
    {
        switch (Weapons[index].InputType)
>>>>>>> Stashed changes
        {
            case EquipmentInputType.Click:

                if (Input.GetKeyDown(key))
                {
                    Weapons[index].OnUse(this);
                    UpdateText(index);
                }

                break;

            case EquipmentInputType.Hold:

                if (Input.GetKey(key))
                {
                    Weapons[index].OnUse(this);
                    UpdateText(index);
                }

                break;

            case EquipmentInputType.Release:

                if (Input.GetKey(key))
                {
                    Weapons[index].OnUse(this);
                    UpdateText(index);
                }

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
}
