using UnityEngine;

// DEV ONLY - put on the Player for testing equipment. Remove it (or untick it) for the real game.
//  - Press Tab to cycle the left mouse weapon through the guns in LeftMouseGuns
//  - Logs mouse presses/releases and which equipment they use
// The setup menu (Tools > Ben > Equipment Test Setup) adds this and fills it in for you.
[RequireComponent(typeof(Player))]
public class EquipmentDevTools : MonoBehaviour
{
    [Tooltip("Guns that Tab cycles through on left mouse (e.g. Laser, Shotgun, Gatling, Artillery)")]
    public Equipment[] LeftMouseGuns;

    public KeyCode CycleKey = KeyCode.Tab;

    [Tooltip("Log mouse presses/releases to the console")]
    public bool LogMouse = true;

    Player player;
    int index;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void Start()
    {
        // Start the cycle from whatever is already on left mouse
        int current = System.Array.IndexOf(LeftMouseGuns, player.Weapons[0]);
        index = Mathf.Max(0, current);
        Debug.Log($"[Dev] LMB = {NameOf(player.Weapons[0])}, RMB = {NameOf(player.Weapons[1])}. Press {CycleKey} to switch the LMB gun.");
    }

    void Update()
    {
        if (Input.GetKeyDown(CycleKey)) CycleLeftGun();

        if (LogMouse)
        {
            LogButton(0, KeyCode.Mouse0, "LMB");
            LogButton(1, KeyCode.Mouse1, "RMB");
        }
    }

    void CycleLeftGun()
    {
        if (LeftMouseGuns == null || LeftMouseGuns.Length == 0) return;

        // Skip empty slots
        for (int i = 0; i < LeftMouseGuns.Length; i++)
        {
            index = (index + 1) % LeftMouseGuns.Length;
            if (LeftMouseGuns[index] != null) break;
        }

        player.Weapons[0] = LeftMouseGuns[index];
        player.UpdateText(0);
        Debug.Log($"[Dev] LMB switched to: {NameOf(player.Weapons[0])} ({index + 1}/{LeftMouseGuns.Length})");
    }

    void LogButton(int slot, KeyCode key, string label)
    {
        if (player.Weapons.Length <= slot) return;

        if (Input.GetKeyDown(key)) Debug.Log($"[Dev] {label} pressed -> {NameOf(player.Weapons[slot])}");
        if (Input.GetKeyUp(key)) Debug.Log($"[Dev] {label} released -> {NameOf(player.Weapons[slot])}");
    }

    static string NameOf(Equipment e)
    {
        if (e == null) return "(empty)";
        return string.IsNullOrEmpty(e.Name) ? e.GetType().Name : e.Name;
    }
}
