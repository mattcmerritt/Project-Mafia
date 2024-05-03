using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Mirror;
using Cinemachine;

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
    [SerializeField] private bool enemiesSpawning;
    [SerializeField] private int activeLevel = 0;
    [SerializeField] private int currentWaves;

    //for fading
    [SerializeField] private PlayableDirector fadeDirector;
    [SerializeField] private CinemachineVirtualCamera playerFollowVCam;

    // for cutscene
    [SerializeField] private PlayableDirector cutsceneDirector;
    private bool cutsceneFinished;
    [SerializeField] private GameObject playerSword;

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
    }

    [ClientRpc]
    private void ClientMovePlayerToStart(Vector3 start)
    {
        StartCoroutine(MovePlayerCoroutine(start));
    }

    private IEnumerator MovePlayerCoroutine(Vector3 start)
    {
        // disable player input
        PlayerManager.Instance.PauseAllPlayers();
        // fade in here
        fadeDirector.Play();
        yield return new WaitForSeconds(1);
        // on server start this is not ready yet
        if (PlayerManager.Instance)
        {
            playerFollowVCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 0f;
            playerFollowVCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 0f;
            playerFollowVCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 0f;
            PlayerManager.Instance.transform.position = start;
        }
        // fade out automatically happens, could go here if needed
        yield return new WaitForSeconds(1);
        playerFollowVCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_XDamping = 1.5f;
        playerFollowVCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 1.5f;
        playerFollowVCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 1f;

        // for the last room, play the cutscene on enter
        if (activeLevel == levelDetails.Count - 1)
        {
            cutsceneDirector.Play();
            yield return new WaitForSeconds(54); // TODO: match cutscene length
            cutsceneFinished = true;

            // TODO: fix sword glitch on cutscene
            playerSword.transform.localPosition = new Vector3(-0.00680000009f, 0.0523074381f, 0.00837089401f);
            playerSword.transform.localEulerAngles = new Vector3(39.1976585f, 187.147018f, 267.431396f);
        }

        // enable player input
        PlayerManager.Instance.ResumeAllPlayers();

        // Spawn the enemies after
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        // determine which enemies need to be spawned, and spawn them
        enemies = new List<Agent>();
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

        enemiesSpawning = false;
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
                Debug.Log("Spawning next wave.");
                enemiesSpawning = true;
                StartCoroutine(SpawnWaveOfEnemies(0));
                currentWaves--;
            }
            else if (activeLevel < levelDetails.Count - 1 && currentWaves <= 0 && enemies.Count == 0 && !enemiesSpawning)
            {
                Debug.Log("Moving to next room!");
                activeLevel++;
                enemiesSpawning = true;
                LoadLevel(activeLevel);
                currentWaves--;
            }
            // keep repeating last wave
            else if (activeLevel == levelDetails.Count - 1 && enemies.Count <= 0 && !enemiesSpawning)
            {
                Debug.Log("Respawning final wave!");
                enemiesSpawning = true;
                StartCoroutine(SpawnWaveOfEnemies(2));
            }
        }
    }

    private IEnumerator SpawnWaveOfEnemies(int delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnEnemies();
    }   
}
