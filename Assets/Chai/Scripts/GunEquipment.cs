using DG.Tweening;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class GunEquipment : Equipment
{
    public Transform ShootPoint;
    public GameObject GunModel;
    public GameObject MuzzleFlash;
    public float Damage;
    public float FireRate;
    protected float nextFireTime;
    public float Range; //for raycasts
    public float Power; //for projectiles
    public bool CanShoot;
    public TrailRenderer Trail;
    public GameObject ImpactVFX;

    float normalAmount;
    public float RecoilAmount;
    Sequence recoilSequence;
    Tween shakeTween;

    private void Awake()
    {
        normalAmount = transform.localPosition.z;
        CanShoot = true;
        ShootPoint = transform.Find("ShootPoint");
        Debug.Log($"{Name} shootpoint: {ShootPoint.name}");

        if (ShootPoint == null)
        {
            Debug.Log("No ShootPoint found!");
        }
    }

    protected void Recoil()
    {
        recoilSequence.Kill();

        MuzzleFlashController(true);

        //CameraShake.Shake(.25f, .2f);
        recoilSequence = DOTween.Sequence();
        recoilSequence.Append(transform.DOLocalMoveZ(RecoilAmount, .5f).SetEase(Ease.OutQuad));
        recoilSequence.Append(transform.DOLocalMoveZ(normalAmount, 2f).SetEase(Ease.OutQuad));
        GunModel.transform.DOShakePosition(.2f, .05f).SetEase(Ease.OutQuad);
    }

    public void MuzzleFlashController(bool enable)
    {
        if (enable)
        {
            if (!MuzzleFlash.activeSelf || MuzzleFlash != null)
            {
                if (InputType == EquipmentInputType.Click || InputType == EquipmentInputType.Release)
                {
                    Instantiate(MuzzleFlash, ShootPoint.position, Quaternion.identity);
                }
                else
                {
                    MuzzleFlash.GetComponent<ParticleSystem>().Play();
                }
            }
        }
        else
        {
            if (InputType != EquipmentInputType.Click && InputType != EquipmentInputType.Release)
            {
                MuzzleFlash.GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    protected IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
        Debug.Log("test");
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= Range * Time.deltaTime;

            yield return null;
        }

        //Animator.SetBool("IsShooting", false);
        Trail.transform.position = HitPoint;
        if (MadeImpact)
        {
            Instantiate(ImpactVFX, HitPoint, Quaternion.LookRotation(HitNormal));
        }

        Destroy(Trail.gameObject, Trail.time);
    }
}
