using DG.Tweening;
using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Pool;

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

    private IObjectPool<TrailRenderer> trailPool;
    private IObjectPool<GameObject> impactVFXPool;

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

        trailPool = new ObjectPool<TrailRenderer>(
            createFunc: () => Instantiate(Trail),
            actionOnGet: (trail) => trail.gameObject.SetActive(true),
            actionOnRelease: (trail) => trail.gameObject.SetActive(false),
            actionOnDestroy: (trail) => Destroy(trail.gameObject),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 20
        );

        impactVFXPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(ImpactVFX),
            actionOnGet: (vfx) => vfx.SetActive(true),
            actionOnRelease: (vfx) => vfx.SetActive(false),
            actionOnDestroy: (vfx) => Destroy(vfx),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 20
        );
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

    protected IEnumerator SpawnTrail(Vector3 StartPoint, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
        TrailRenderer Trail = trailPool.Get();

        // freaky ahh way to move the trail without it being visible at the start
        Trail.enabled = false;
        Trail.transform.position = StartPoint;
        Trail.Clear();
        Trail.enabled = true;

        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(StartPoint, HitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= Range * Time.deltaTime;

            yield return null;
        }

        //Animator.SetBool("IsShooting", false);
        Trail.transform.position = HitPoint;
        if (MadeImpact)
        {
            GameObject vfxInstance = impactVFXPool.Get();
            vfxInstance.transform.position = HitPoint;
            vfxInstance.transform.rotation = Quaternion.LookRotation(HitNormal);
            StartCoroutine(DelayedRelease(vfxInstance, 1f)); // 1 second delay is arbitrary, i just made it up. im too lazy for this
        }

        yield return new WaitForSeconds(Trail.time);
        trailPool.Release(Trail);
    }

    protected IEnumerator DelayedRelease(GameObject vfxInstance, float delay)
    {
        yield return new WaitForSeconds(delay);
        impactVFXPool.Release(vfxInstance);
    }
}
