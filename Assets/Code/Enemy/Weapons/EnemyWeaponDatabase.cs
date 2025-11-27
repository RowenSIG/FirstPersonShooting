using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWeaponDatabase", menuName = "Scriptable Objects/EnemyWeaponDatabase")]
public class EnemyWeaponDatabase : ScriptableObject
{
    [SerializeField]
    private List<EnemyWeaponData> data;
    public List<EnemyWeaponData> Data => data;

}
