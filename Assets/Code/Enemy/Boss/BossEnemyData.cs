using System.Collections.Generic;
using UnityEngine;

public enum eBossEnemyType
{
    INVALID = 0,

    MECHA_BEAR = 10,

    GIANT_SPIDER = 20,

    //others.
}

[CreateAssetMenu(fileName = "BossEnemyData", menuName = "Scriptable Objects/BossEnemyData")]
public class BossEnemyData : ScriptableObject
{
    [SerializeField]
    private eBossEnemyType type;
    public eBossEnemyType Type => type;

    [SerializeField]
    private GameObject prefab;
    public GameObject Prefab => prefab;

    [SerializeField]
    private List<eEnemyWeaponModuleType> modules;
    public List<eEnemyWeaponModuleType> Modules => modules;


    //other things... modifiers?
    
}
