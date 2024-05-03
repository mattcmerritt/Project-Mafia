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

[System.Serializable]
public struct LevelDetails
{
    public Vector3 playerSpawn;
    public List<EnemyToSpawnDetails> enemies;
    public int waves;
} 

public class EnemyManager : NetworkBehaviour
{
    public static EnemyManager Instance;

    // Initial values for the room
    [SerializeField] private List<GameObject> enemyPrefabOptions;
    [SerializeField] private List<LevelDetails> levelDetails;

    // State information - only maintained server side
    [SerializeField] private List<Agent> enemies;
    private bool enemiesSpawning;
    [SerializeField] private int activeLevel = 0;
    private int currentWaves;

    private void Start()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        LoadLevel(0);
    }

    private void LoadLevel(int activeLevel)
    {
        Debug.Log("Loading level " + activeLevel);
        this.activeLevel = activeLevel;
        currentWaves = levelDetails[activeLevel].waves;
        ClientMovePlayerToStart(levelDetails[activeLevel].playerSpawn);
        SpawnEnemies();
    }

    [ClientRpc]
    private void ClientMovePlayerToStart(Vector3 start)
    {
        StartCoroutine(MovePlayerCoroutine(start));
    }

    private IEnumerator MovePlayerCoroutine(Vector3 start)
    {
        // TODO: fade in here
        yield return new WaitForSeconds(1);
        // on server start this is not ready yet
        if (PlayerManager.Instance)
        {
            PlayerManager.Instance.transform.position = start;
        }
        // TODO: fade out here
        yield return new WaitForSeconds(1);
    }

    private void SpawnEnemies()
    {
        // determine which enemies need to be spawned, and spawn them
        enemies = new List<Agent>();
        // decrease the number of waves
        currentWaves--;
        // spawn prefabs
        foreach (EnemyToSpawnDetails enemyToSpawn in levelDetails[activeLevel].enemies)
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
            if (currentWaves > 0 && enemies.Count == 0 && !enemiesSpawning)
            {
                enemiesSpawning = true;
                StartCoroutine(SpawnWaveOfEnemies(0));
                currentWaves--;
            }
            else if (activeLevel < levelDetails.Count - 1 && currentWaves == 0 && enemies.Count == 0 && !enemiesSpawning)
            {
                activeLevel++;
                LoadLevel(activeLevel);
            }
            // keep repeating last wave
            else if (activeLevel >= levelDetails.Count - 1 && currentWaves == 0 && enemies.Count == 0 && !enemiesSpawning)
            {
                activeLevel = levelDetails.Count - 1;
                enemiesSpawning = true;
                StartCoroutine(SpawnWaveOfEnemies(2));
            }
        }
    }

    private IEnumerator SpawnWaveOfEnemies(int delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnEnemies();
        enemiesSpawning = false;
    }   
}
