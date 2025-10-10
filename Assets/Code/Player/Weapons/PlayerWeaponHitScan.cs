using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Logging;

public class PlayerWeaponHitScan : PlayerWeapon
{
    public enum eFireMode
    {
        INVALID = 0,

        SINGLE_SHOT = 10,
        AUTOMATIC = 20,
    }

    [SerializeField]
    private int magazineCapacity;
    [SerializeField]
    private int roundsFiredPerFire = 1;
    [SerializeField]
    private eFireMode fireMode;

    [SerializeField]
    private float recycleTime = 0.1f;
    [SerializeField]
    private float reloadTime = 3f;

    [SerializeField]
    private Transform muzzleFlashPoint;
    [SerializeField]
    private Transform projectileOrigin;

    [SerializeField]
    private GameObject muzzleFlashPrefab;
    [SerializeField]
    private GameObject bulletStrikePrefab;

    [SerializeField]
    private GameObject reloadingVisual;

    private float recycleTimer = 0f;
    private float reloadTimer = 0f;
    private bool fireHeld = true;

    private int totalAmmo = int.MaxValue;
    private int ammoInMagazine = 0;

    private bool CanFire
    {
        get
        {
            if (fireHeld && fireMode == eFireMode.SINGLE_SHOT)
            {
                return false;
            }

            if (reloadTimer <= 0f
                 && ammoInMagazine > 0
                 && recycleTimer <= 0f)
            {
                return true;
            }

            return false;
        }
    }

    public override void Setup(Player player)
    {
        DoReload();
        base.Setup(player);
    }

    public override void UpdateWeapon(float deltaTime, bool leftFire, bool rightFire, bool reloadButton)
    {
        bool reloading = reloadTimer > 0f;
        reloadTimer -= deltaTime;
        recycleTimer -= deltaTime;

        reloadingVisual.EnsureActive(reloading);
        
        if (reloading && reloadTimer <= 0f)
        {
            ammoInMagazine = Mathf.Min(magazineCapacity, totalAmmo);
            totalAmmo -= ammoInMagazine;
            Log($"[PlayerWeaponHitScan] Finished Reloading!");

        }

        if (reloading == false && reloadButton)
        {
            DoReload();
        }

        if (CanFire && rightFire)
        {
            Fire();
        }
        fireHeld = rightFire;
    }

    private void Fire()
    {
        //where?...
        fireHeld = false;
        recycleTimer = recycleTime;

        ammoInMagazine -= roundsFiredPerFire;

        FireBullet();

        if (ammoInMagazine <= 0)
        {
            DoReload();
        }

    }

    private void FireBullet()
    {
        //simple version:
        var flash = Instantiate(muzzleFlashPrefab, transform);
        flash.transform.position = muzzleFlashPoint.position;
        var localRot = flash.transform.localEulerAngles;
        localRot.z = Random.Range(0, 30f);
        flash.transform.localEulerAngles = localRot;
        
        //
        var ray = player.PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        var hit = Physics.Raycast(ray, out var hitInfo, 100f);
        if (hit)
        {
            //wherever it hit:
            var bulletHit = Instantiate(bulletStrikePrefab);
            bulletHit.transform.position = hitInfo.point;
            bulletHit.transform.forward = hitInfo.normal;

            var smashblock = hitInfo.collider.gameObject.GetComponent<SmashBlock>(); 
            if ( smashblock != null )
            {
                var body = smashblock.GetComponent<Rigidbody>();
                var forceDir = ray.direction + Vector3.up;
                if(body == null)
                {
                    body = smashblock.AddComponent<Rigidbody>();
                    forceDir = Vector3Utils.RandomVector3().normalized;
                }
                body.AddForce( forceDir * 10f, ForceMode.Impulse );
            }
        }
    }

    private void DoReload()
    {
        totalAmmo += ammoInMagazine;
        ammoInMagazine = 0;
        reloadTimer = reloadTime;
        Log($"[PlayerWeaponHitScan] Reload!");

    }


}
