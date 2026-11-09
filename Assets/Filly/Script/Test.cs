using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            AudioManager.Instance.PlayOneShotSFX(SoundType.TestSound);
            //Debug.Log("E");
        }
    }
}
