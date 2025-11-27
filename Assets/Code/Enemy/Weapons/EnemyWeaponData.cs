using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyWeaponData", menuName = "Scriptable Objects/EnemyWeaponData")]
public class EnemyWeaponData : ScriptableObject
{
    
    [SerializeField]
    public eEnemyWeaponType weaponType;
    
    [SerializeField]
    public int level;

    [SerializeField]
    public GameObject prefab;

    [SerializeField]
    public List<eEnemyWeaponModuleType> compatibleModuleTypes;

    //a bunch of stuff:
    //rarity
    //clip size
    //reload time
    //repeat rate
    //bullet type
    //damage per hit
    //other stuff
    //weight? 
    //cost?    
}
