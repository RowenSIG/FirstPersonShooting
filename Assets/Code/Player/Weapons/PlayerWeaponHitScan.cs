using Unity.VisualScripting;
using UnityEngine;
using Fusion;
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

    [SerializeField]
    private WeaponRecoil recoil;

    [SerializeField]
    private float bulletForce;
    [SerializeField]
    private float bulletDamage;

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
        base.Setup(player);
        DoReload(instant: true);
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

        var fireInput = leftFire;
        if (CanFire && fireInput)
        {
            Fire();
        }
        fireHeld = leftFire;
    }

    private void Fire()
    {
        //where?...
        fireHeld = false;
        recycleTimer = recycleTime;

        ammoInMagazine -= roundsFiredPerFire;

        FireBullet();

        var shake = player.PlayerCamera.GetComponent<CameraShake>();
        if (shake != null)
        {
            shake.DoShake(0.1f);
        }

        recoil.DoRecoil(1f);

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
            NetworkId netObjectId = default;
            if (hitInfo.collider != null)
            {
                if (hitInfo.collider.gameObject.GetComponent<NetworkObject>() is NetworkObject netObj)
                {
                    netObjectId = netObj.Id;
                }
                else if (hitInfo.collider.attachedRigidbody != null
                    && hitInfo.collider.attachedRigidbody.GetComponent<NetworkObject>() is NetworkObject netObj2)
                {
                    netObjectId = netObj2.Id;


                    if(netObj2.GetComponent<Enemy>() is Enemy enemy)
                    {
                        EnemyManager.Instance.EnemyHit(enemy, bulletDamage);
                    }
                }
            }
            
            projectileData.Set(fireCount % projectileData.Length, new ProjectileData()
            {
                point = hitInfo.point,
                normal = hitInfo.normal,
                direction = ray.direction,
                id = netObjectId,
            });
            fireCount++;

        }
    }

    private void ShowBulletHit(Vector3 point, Vector3 normal, Vector3 direction, NetworkId id)
    {

        //wherever it hit:
        var bulletHit = Instantiate(bulletStrikePrefab);
        bulletHit.transform.position = point;
        bulletHit.transform.forward = normal;


        if (id)
        {
            Runner.TryFindObject(id, out NetworkObject netObj);
            if (netObj != null)
            {
                bulletHit.transform.SetParent(netObj.transform, true);

                var collider = netObj.GetComponent<Collider>();
                if (collider != null)
                {

                    if (collider.attachedRigidbody != null)
                    {
                        collider.attachedRigidbody.AddForceAtPosition(direction * bulletForce, point, ForceMode.Impulse);
                    }

                    var smashblock = collider.gameObject.GetComponent<SmashBlock>();
                    if (smashblock != null)
                    {
                        var body = smashblock.GetComponent<Rigidbody>();
                        var forceDir = direction + Vector3.up;
                        if (body == null)
                        {
                            body = smashblock.AddComponent<Rigidbody>();
                            smashblock.GoPhysical(body);
                            forceDir = Vector3Utils.RandomVector3().normalized;
                        }
                        body.AddForceAtPosition(forceDir * 10f, point, ForceMode.Impulse);
                    }
                }
                var rb = netObj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForceAtPosition(direction * bulletForce, point, ForceMode.Impulse);
                }

            }
        }
    }

    private void DoReload(bool instant = false)
    {
        totalAmmo += ammoInMagazine;
        ammoInMagazine = 0;
        reloadTimer = reloadTime;

        if (instant)
        {
            reloadTimer = 0f;
            ammoInMagazine = Mathf.Min(magazineCapacity, totalAmmo);
            totalAmmo -= ammoInMagazine;
        }
        Log($"[PlayerWeaponHitScan] Reload!");

    }


     [Networked]
    private int fireCount { get; set; }
    [Networked, Capacity(32)]
    private NetworkArray<ProjectileData> projectileData { get; }

    private int visibleFireCount;
    private struct ProjectileData : INetworkStruct
    {
        public Vector3 point;
        public Vector3 normal;
        public Vector3 direction;
        public NetworkId id;
    }  
    
    public override void Render()
    {
        base.Render();
        if (visibleFireCount < fireCount)
        {
            // Play fire effects (e.g. fire sound, muzzle particle)
        }

        for (int i = visibleFireCount; i < fireCount; i++)
        {
            var data = projectileData[i % projectileData.Length];

            ShowBulletHit(data.point, data.normal, data.direction, data.id);

            // Show projectile visuals (e.g. spawn dummy flying projectile or trail
            // from fireTransform to data.HitPosition or spawn impact effect on data.HitPosition)
        }

        visibleFireCount = fireCount;
    }
}
