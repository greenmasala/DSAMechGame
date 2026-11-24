using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    Tween shakeTween;
    PlayerCamera holder;

    void Awake() => Instance = this;

    private void Start()
    {
        holder = GetComponent<PlayerCamera>();
    }

    void OnShake(float duration, float str)
    {
        if (shakeTween.IsActive() && shakeTween != null)
        {
            shakeTween.Complete();
        }

        shakeTween = transform.DOShakePosition(duration, str);
    }

    public static void Shake(float duration, float str) => Instance.OnShake(duration, str);

    void OnTilt(float dir)
    {
        var rotateDir = new Vector3(holder.xRotation, holder.yRotation, dir * -25f);
        transform.DOLocalRotate(rotateDir, .2f);
    }

    public static void CameraTilt(float dir) => Instance.OnTilt(dir);
}
