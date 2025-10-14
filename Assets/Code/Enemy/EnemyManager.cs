using System.Collections.Generic;
using Fusion;
using UnityEngine;
using static Logging;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;
    public static EnemyManager Instance => instance;

    private GameObject enemyContainer = null;

    [SerializeField]
    private NetworkRunner Runner;

    [SerializeField]
    private Enemy enemyPrefab;

    [SerializeField]
    private EnemyConfiguration enemyConfigPrefab;

    private List<Enemy> enemies = new List<Enemy>();
    private float lastSpawn;
    private float spawnPeriod = 5;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(Time.timeSinceLevelLoad - lastSpawn > spawnPeriod && enemies.Count < 4)
        {
            InstantiateEnemy();
            lastSpawn = Time.timeSinceLevelLoad;
        }
    }

    public void InstantiateEnemy()
    {
        if (enemyContainer == null)
        {
            enemyContainer = new GameObject();
            enemyContainer.name = "enemyContainer";
        }

        Log($"[EnemyManager] InstantiateEnemy");
        var enemyObj = Runner.Spawn(enemyPrefab.gameObject, position: enemyContainer.transform.position, rotation: Quaternion.identity);
        var enemy = enemyObj.GetComponent<Enemy>();
        enemy.Setup(enemyConfigPrefab);
        enemy.gameObject.name = $"Enemy[{enemies.Count}]";
        enemies.Add(enemy);

        var spawn = LevelManager.Instance.GetLevel().GetRandomEnemySpawnPoint();
        enemy.transform.position = spawn.transform.position;
        enemy.transform.rotation = spawn.transform.rotation;
    }
}
