using System.Collections.Generic;
using Fusion;
using UnityEngine;
using static Logging;

public class EnemyManager : SimulationBehaviour
{
    private static EnemyManager instance;
    public static EnemyManager Instance => instance;

    private GameObject enemyContainer = null;


    [SerializeField]
    private Enemy enemyPrefab;

    [SerializeField]
    private EnemyConfiguration enemyConfigPrefab;

    [SerializeField]
    private float enemySpawnPeriod;
    [SerializeField]
    private int enemyLimit;

    private List<Enemy> enemies = new List<Enemy>();
    private float lastSpawn;

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
        if (Time.realtimeSinceStartup < 15f)
            return;

        if (Runner == null)
            return;

        if (Runner.IsSharedModeMasterClient 
        && Runner.CanSpawn
        && Time.timeSinceLevelLoad - lastSpawn > enemySpawnPeriod 
        && enemies.Count < enemyLimit)
        {
            InstantiateEnemy();
            lastSpawn = Time.timeSinceLevelLoad;
        }

        enemies.RemoveAll(p => p == null);
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

    public void EnemyHit(Enemy enemy, float damage)
    {
        if(Runner.IsSharedModeMasterClient)
        {
            enemy.TakeDamage(damage);

            if(enemy.hp <= 0)
            {
                Runner.Despawn(enemy.GetComponent<NetworkObject>());
            }
        }
    }
}
