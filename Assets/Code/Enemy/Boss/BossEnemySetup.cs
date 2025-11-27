using System.Collections.Generic;
using UnityEngine;
using static Logging;

public class BossEnemySetup 
{
    public struct WeaponSetup
    {
        public eEnemyWeaponModuleType moduleType;
        public eEnemyWeaponType weaponType;
        public int level;
    }

    private BossEnemyData bossData;
    private List<eEnemyWeaponModuleType> modules = new();
    private List<WeaponSetup> setups = new();

    public List<WeaponSetup> Setup => setups;
    
    
    public BossEnemySetup(BossEnemyData bossData)
    {
        this.bossData = bossData;

        foreach(var module in bossData.Modules)
        {
            modules.Add(module);
        }
    }
    
    public void SetWeaponInModule(eEnemyWeaponModuleType weaponModule, eEnemyWeaponType weaponType, int level)
    {
        //check, i guess...
        if(modules.Contains(weaponModule) == false)
        {
            LogError($"[BossEnemySetup] no weapon module type [{weaponModule}] within bossData[{bossData}]");
            return;
        }
       
        Setup.Add(new WeaponSetup() { weaponType = weaponType, moduleType = weaponModule, level = level });

    }
}
