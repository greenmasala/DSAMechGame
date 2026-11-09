using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    Tween shakeTween;

    void Awake() => Instance = this;

    void OnShake(float duration, float str)
    {
        if (shakeTween.IsActive() && shakeTween != null)
        {
            shakeTween.Complete();
        }

        shakeTween = transform.DOShakePosition(duration, str);
    }

    public static void Shake(float duration, float str) => Instance.OnShake(duration, str);

    //void OnTilt()
}
