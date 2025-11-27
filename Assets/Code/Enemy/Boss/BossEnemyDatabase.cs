using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossEnemyDatabase", menuName = "Scriptable Objects/BossEnemyDatabase")]
public class BossEnemyDatabase : ScriptableObject
{
   
    [SerializeField]
    private List<BossEnemyData> data;
    public List<BossEnemyData> Data => data;
}
