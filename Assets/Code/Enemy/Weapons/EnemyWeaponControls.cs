using System.Collections.Generic;
using UnityEngine;
using static Logging;

public class EnemyWeaponControls : MonoBehaviour
{
    private List<EnemyWeaponModule> modules = new ();

    private void Awake()
    {
        var found = GetComponentsInChildren<EnemyWeaponModule>();
        modules.AddRange(found);
    }

    public void Setup(BossEnemySetup bossSetup)
    {
        //we have a large number of modules, we can just attach them... yes?

        foreach(var setup in bossSetup.Setup)
        {

            var module = modules.Find( p => p.ModuleType == setup.moduleType );
            if(module == null)
            {
                LogError($"[EnemyWeaponControls] Setup fail - module not found with moduleType[{setup.moduleType}]");
                continue;
            }

            module.AttachWeapon(setup);

        }
    }

    public void Fire()
    {
        foreach(var module in modules)
        {
            module.Fire();
        }
    }
}
