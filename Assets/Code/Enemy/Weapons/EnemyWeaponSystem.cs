using UnityEngine;
using static Logging;

public enum eEnemyWeaponType
{
    INVALID = 0,

    MACHINE_GUN = 100,
    ROCKET_LAUNCHER = 200,
}

public enum eEnemyWeaponSide
{
    INVALID = 0,

    NONE = 10,
    RIGHT = 20,
    LEFT = 30,
    EITHER = 40,
}

public class EnemyWeaponSystem : MonoBehaviour
{
    public static EnemyWeaponSystem Instance {get ; private set; }

    [SerializeField]
    private EnemyWeaponDatabase database;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public EnemyWeaponData GetWeaponDataFromTypeAndLevel(eEnemyWeaponType type, int level)
    {
        //let's just find it the old fashioned way:
        foreach(var weaponData in database.Data)
        {
            if(weaponData.weaponType == type && weaponData.level == level)
                return weaponData;   
        }
        LogError($"[EnemyWeaponSystem] No Weapon of type[{type}] and level[{level}]");
        return null;
    }
    
}
