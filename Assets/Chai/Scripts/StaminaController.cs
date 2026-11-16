using System.Collections;
using TMPro;
using UnityEngine;

public class StaminaController : MonoBehaviour
{
    //public static StaminaController Instance;

    public float CurrentEnergy = 100f;
    float maxEnergy;
    public bool IsRegenerating;

    float staminaRegen = 20f;
    float staminaRegenDelay = 0.5f;
    float regenTimer;

    public TextMeshProUGUI StaminaText;

    private void Awake()
    {
        maxEnergy = CurrentEnergy;
        UpdateStaminaText(CurrentEnergy); 
    }

    private void Update()
    {
        if (CurrentEnergy >= maxEnergy)
        {
            return;
        }

        if (regenTimer > 0)
        {
            regenTimer -= Time.deltaTime;
            return;
        }

        IsRegenerating = true;

        CurrentEnergy += staminaRegen * Time.deltaTime;
        CurrentEnergy = Mathf.Min(CurrentEnergy, maxEnergy);

        UpdateStaminaText(CurrentEnergy);
    }

    public void UpdateStamina(float energyAmount)
    {
        if (CurrentEnergy >= energyAmount)
        {
            CurrentEnergy = Mathf.Max(0, CurrentEnergy - energyAmount);
            IsRegenerating = false;
            regenTimer = staminaRegenDelay;
            UpdateStaminaText(CurrentEnergy);
        }
    }

    void UpdateStaminaText(float energyAmount)
    {
        StaminaText.text = energyAmount.ToString("F2");
    }

    public bool HasStamina(float energy)
    {
        return CurrentEnergy >= energy;
    }

    //private void HandleRegeneration()
    //{
    //    if (HasRegenerated)
    //    {
    //        delayTimer = staminaRegenDelay;
    //        return;
    //    }

    //    if (delayTimer > 0f)
    //    {
    //        delayTimer -= Time.deltaTime;
    //        return;
    //    }

    //    if (CurrentEnergy < maxEnergy)
    //    {
    //        CurrentEnergy += (staminaRegen * Time.deltaTime);
    //        CurrentEnergy = Mathf.Clamp(CurrentEnergy, 0f, maxEnergy);
    //    }
    //}

    //public static void UpdateStamina(float energy) => Instance.DrainStamina(energy);
}
