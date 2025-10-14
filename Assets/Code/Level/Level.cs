using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{

    [SerializeField]
    private LevelSpawnPoint spawnPoint;
    public LevelSpawnPoint SpawnPoint => spawnPoint;


    [SerializeField]
    private List<LevelSpawnPoint> enemySpawnPoints = new List<LevelSpawnPoint>();
    public List<LevelSpawnPoint> EnemySpawnPoints => enemySpawnPoints;

    public LevelSpawnPoint GetRandomEnemySpawnPoint()
    {
        if (enemySpawnPoints.Count == 0)
        {
            Debug.LogError($"[Level] No enemy spawn points found!");
            return null;
        }
        return enemySpawnPoints.Random();
    }
}
