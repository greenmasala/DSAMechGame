using System.Collections;
using TMPro;
using UnityEngine;

public class StaminaController : MonoBehaviour
{
    //public static StaminaController Instance;

    public float CurrentEnergy = 100f;
    float maxEnergy;

    float jumpCost = 15;
    float lastSprintTime;
    public bool HasRegenerated;
    public bool IsSprinting;

    float staminaDrain = 20f;
    float staminaRegen = 30f;
    float staminaRegenDelay = 0.5f;
    float delayTimer;

    float normalSpeed;
    float runSpeed;

    public TextMeshProUGUI StaminaText;
    private PlayerController player;

    private void Awake()
    {
        //Instance = this;
        maxEnergy = CurrentEnergy;
        UpdateStaminaText(CurrentEnergy); 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRegeneration();

        if (IsSprinting)
        {
            CurrentEnergy -= (staminaDrain * Time.deltaTime);
            //lastSprintTime = Time.time;
            UpdateStaminaText(CurrentEnergy);
            if (CurrentEnergy <= 0)
            {
                CurrentEnergy = 0;
                IsSprinting = false;
                //HasRegenerated = false;
            }
        }
        //else
        //{
        //    HasRegenerated = false;
        //}

        //if (!HasRegenerated)
        //{
        //    if (Time.time > lastSprintTime + staminaRegenDelay)
        //    {
        //        if (CurrentEnergy < maxEnergy)
        //        {
        //            CurrentEnergy += (staminaRegen * Time.deltaTime);
        //            UpdateStaminaText(CurrentEnergy);
        //        }
        //        else
        //        {
        //            HasRegenerated = true;
        //        }
        //    }
        //}
    }

    public void UpdateStamina(float energyAmount)
    {
        if (CurrentEnergy > 0)
        {
            //HasRegenerated = false;
            CurrentEnergy -= energyAmount;
            delayTimer = staminaRegenDelay;
            UpdateStaminaText(CurrentEnergy);
        }
    }

    void UpdateStaminaText(float energyAmount)
    {
        StaminaText.text = energyAmount.ToString();
    }

    public bool HasStamina()
    {
        return CurrentEnergy > 0;
    }

    private void HandleRegeneration()
    {
        if (HasRegenerated)
        {
            delayTimer = staminaRegenDelay;
            return;
        }

        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        if (CurrentEnergy < maxEnergy)
        {
            CurrentEnergy += (staminaRegen * Time.deltaTime);
            CurrentEnergy = Mathf.Clamp(CurrentEnergy, 0f, maxEnergy);
        }
    }

    //public static void UpdateStamina(float energy) => Instance.DrainStamina(energy);

    //IEnumerator RegenerateStamina()
    //{

    //}
}
