using UnityEngine;

public enum eEnemyWeaponModuleType
{
    INVALID = 0,

    HIP = 100,
    LOWER_CHEST = 110,
    UPPER_CHEST = 120,

    SHOULDER = 200,
    UPPER_ARM = 210,
    LOWER_ARM = 220,
    HAND = 230,

    UPPER_LEG = 300,
    LOWER_LEG = 310,
    
    UPPER_BACK = 400,
    LOWER_BACK = 410,
    
    HEAD_SIDE = 500,
    HEAD_TOP = 510,
}

public class EnemyWeaponModule : MonoBehaviour
{
    [SerializeField]
    private eEnemyWeaponModuleType moduleType;
    public eEnemyWeaponModuleType ModuleType => moduleType;

    [SerializeField]
    private Transform weaponModuleAnchorPoint;

    private EnemyWeapon weapon;

    public void AttachWeapon(BossEnemySetup.WeaponSetup setup)
    {
        var type = setup.weaponType;
        var level = setup.level;

        var data = EnemyWeaponSystem.Instance.GetWeaponDataFromTypeAndLevel(type, level);

        var prefab = data.prefab;
        var clone = Instantiate(prefab);
        clone.transform.parent = weaponModuleAnchorPoint;
        clone.transform.localScale = Vector3.one;
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.identity;

        weapon = clone.GetComponent<EnemyWeapon>();
        weapon.Initialise(data);
    }

    public void Clear()
    {
        if(weapon != null)
        {
            Destroy(weapon.gameObject);
            weapon = null;
        }
    }
    public void Fire()
    {
        if(weapon != null)
        {
            weapon.Fire();
        }
    }
}
