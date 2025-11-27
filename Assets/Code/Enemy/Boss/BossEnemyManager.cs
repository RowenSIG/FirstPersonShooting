using UnityEngine;

public class BossEnemyManager : MonoBehaviour
{
    [SerializeField]
    private BossEnemy bossEnemy;

    [SerializeField]
    private BossEnemyDatabase bossEnemyDatabase;
    void Start()
    {
        var bossData = bossEnemyDatabase.Data.Find( p=> p.Type == eBossEnemyType.MECHA_BEAR);

        var setup = new BossEnemySetup(bossData);
        setup.SetWeaponInModule( eEnemyWeaponModuleType.LOWER_ARM, eEnemyWeaponType.MACHINE_GUN, 1 );

        bossEnemy.Setup(setup);    
    }
}
