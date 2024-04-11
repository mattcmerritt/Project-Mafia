using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public struct EnemyToSpawnDetails
{
    public string type;
    public int amount;
    public List<Vector3> spawnLocations;
    public bool spawnRandomly;
}

public class EnemyManager : NetworkBehaviour
{
    public static EnemyManager Instance;

    // Initial values for the room
    [SerializeField] private List<GameObject> enemyPrefabOptions;
    [SerializeField] public List<EnemyToSpawnDetails> enemiesToSpawn;

    // State information - only maintained server side
    [SerializeField] private List<Agent> enemies;
    private bool enemiesSpawning;

    private void Start()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        // determine which enemies need to be spawned, and spawn them
        enemies = new List<Agent>();
        foreach (EnemyToSpawnDetails enemyToSpawn in enemiesToSpawn)
        {
            GameObject prefab = enemyPrefabOptions.Find((GameObject obj) => obj.GetComponent(enemyToSpawn.type) != null);    
            if (enemyToSpawn.spawnRandomly)
            {
                List<Vector3> choices = new List<Vector3>(enemyToSpawn.spawnLocations);
                for (int i = 0; i < enemyToSpawn.amount; i++)
                {
                    int choice = Random.Range(0, choices.Count);
                    Vector3 selectedPosition = choices[choice];
                    choices.RemoveAt(choice);
                    GameObject spawnedEnemy = Instantiate(prefab, selectedPosition, Quaternion.identity);
                    enemies.Add(spawnedEnemy.GetComponent<Agent>());
                    NetworkServer.Spawn(spawnedEnemy);
                }
            }
            else
            {
                for (int i = 0; i < enemyToSpawn.amount; i++)
                {
                    GameObject spawnedEnemy = Instantiate(prefab, enemyToSpawn.spawnLocations[i], Quaternion.identity);
                    enemies.Add(spawnedEnemy.GetComponent<Agent>());
                    NetworkServer.Spawn(spawnedEnemy);
                }
            } 
        }
    }

    [Server]
    public void RemoveEnemy(Agent agent)
    {
        int index = enemies.FindIndex((Agent a) => a == agent);
        NetworkServer.Destroy(enemies[index].gameObject);
        enemies.RemoveAt(index);
    }

    private void Update()
    {
        if (isServer)
        {
            if (enemies.Count == 0 && !enemiesSpawning)
            {
                enemiesSpawning = true;
                StartCoroutine(SpawnWaveOfEnemies());
            }
        }
    }

    private IEnumerator SpawnWaveOfEnemies()
    {
        yield return new WaitForSeconds(2);
        OnStartServer();
        enemiesSpawning = false;
    }
}
