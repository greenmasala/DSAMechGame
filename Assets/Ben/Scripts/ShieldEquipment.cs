using UnityEngine;

// Hold the button to raise a shield: cuts incoming damage by 75% and drains energy while up.
// Set InputType to Hold in the Inspector. Ammo isn't used.
public class ShieldEquipment : Equipment
{
    [Tooltip("How much damage the shield blocks (0.75 = 75%)")]
    [Range(0f, 1f)] public float DamageReduction = 0.75f;

    [Tooltip("Energy drained per second while the shield is up")]
    public float EnergyPerSecond = 20f;

    [Tooltip("Optional: shield mesh/effect shown while the shield is up")]
    public GameObject ShieldVisual;

    [Tooltip("Print shield up/down to the console (for testing)")]
    public bool LogToConsole = true;

    Player owner;
    StaminaController stamina;
    int lastUsedFrame = -1;
    bool isUp;

    void Start()
    {
        if (ShieldVisual != null) ShieldVisual.SetActive(false);
    }

    // Player calls this every frame while the button is held
    public override void OnUse(Player player)
    {
        owner = player;
        if (stamina == null) stamina = player.GetComponentInChildren<StaminaController>();

        float cost = EnergyPerSecond * Time.deltaTime;
        if (stamina == null || !stamina.HasStamina(cost))
        {
            SetShield(false, "out of energy");
            return;
        }

        stamina.UpdateStamina(cost);
        lastUsedFrame = Time.frameCount;
        SetShield(true, $"blocking {DamageReduction * 100:F0}% damage");
    }

    // Player doesn't tell us when the button is released, so drop the shield
    // on any frame where OnUse wasn't called
    void LateUpdate()
    {
        if (isUp && Time.frameCount != lastUsedFrame) SetShield(false, "released");
    }

    void OnDisable()
    {
        SetShield(false, "disabled");
    }

    void SetShield(bool up, string reason)
    {
        if (isUp == up) return;
        isUp = up;

        if (LogToConsole)
        {
            string energy = stamina != null ? $" | energy {stamina.CurrentEnergy:F0}" : "";
            Debug.Log($"[Shield] {(up ? "ACTIVE" : "OFF")} ({reason}){energy}");
        }

        if (owner != null) owner.DamageReduction = up ? DamageReduction : 0f;
        if (ShieldVisual != null) ShieldVisual.SetActive(up);
    }
}
