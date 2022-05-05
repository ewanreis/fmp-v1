using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class SpawningManager : MonoBehaviour
{
    private const float ghoulPercentage = 0.6f, goblinPercentage = 0.3f, knightPercentage = 0.1f;

    private Vector3[,] spawnArea = new Vector3[7,2];
    private bool canSpawn = false;

    public static int round;

    [SerializeField]
    [Header("Ghoul Statistics")]
    private int ghoulsToSpawn;
    public static float ghoulHealth, ghoulDamage;

    [SerializeField]
    [Header("Goblin Statistics")]
    private int goblinsToSpawn;
    public static float goblinHealth, goblinDamage;

    [SerializeField]
    [Header("Knight Statistics")]
    private int knightsToSpawn;
    public static float knightHealth, knightDamage;

    [Space(10)]
    public GameObject[] enemyPrefabs;
    
    [SerializeField]
    private int enemiesLeft, enemiesToSpawn, zoneToSpawn;
    public TMP_Text RoundDisplay, EnemyRemainingDisplay;

    [SerializeField]
    private int ghoulsSpawned = 0, goblinsSpawned = 0, knightsSpawned = 0;
    [SerializeField]
    private float xLoc, zLoc;

    private float totalPercentage, bloodPercentage, holyPercentage, remainingPercentage;

    void Start()
    {
        round = 0;

        spawnArea[1, 0] = new Vector3(18, 1, 12);
        spawnArea[1, 1] = new Vector3(-8, 1, 18);

        spawnArea[2, 0] = new Vector3(0, 1, 0);
        spawnArea[2, 1] = new Vector3(0, 1, 0);
    }

    void Update()
    {
        if(!PauseMenu.isPaused)
        {
            zoneToSpawn = ZoneManager.activeZone;
            enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if(enemiesLeft == 0 && canSpawn == false) 
                StartNewRound();
            RoundDisplay.text = round.ToString();
            EnemyRemainingDisplay.text = enemiesLeft.ToString();
        }
    }

    private void StartNewRound()
    {
        ghoulsSpawned = 0;
        goblinsSpawned = 0;
        knightsSpawned = 0;
        CalculateSpawnChance(round, false, false);
        CalculateMonsterStats(round);
        StartCoroutine(SpawnMonsters(ghoulsToSpawn, goblinsToSpawn, knightsToSpawn, holyPercentage, bloodPercentage, zoneToSpawn));
        round++;
    }

    private void CalculateSpawnChance(int round, bool dangerArea, bool isNightmare)
    {
        int totalMonstersToSpawn = round + 4;
        totalPercentage = totalMonstersToSpawn * 20;
        bloodPercentage = round / totalPercentage;
        if (dangerArea)
            bloodPercentage *= 10;
        if (isNightmare)
            bloodPercentage *= 10;

        holyPercentage = (isNightmare) ? 5 : 0;

        ghoulsToSpawn = (int)Math.Round(totalMonstersToSpawn * ghoulPercentage);
        goblinsToSpawn = (int)Math.Round(totalMonstersToSpawn * goblinPercentage);
        knightsToSpawn = (int)Math.Floor(totalMonstersToSpawn * knightPercentage);

        enemiesToSpawn = ghoulsToSpawn + goblinsToSpawn + knightsToSpawn;

        remainingPercentage = totalPercentage - goblinPercentage - goblinPercentage - knightPercentage;
    }
    private void CalculateMonsterStats(int round)
    {
        ghoulHealth = 20 + ((float)round / 10);
        goblinHealth = 60 + ((float)round / 10);
        knightHealth = 200 + ((float)round / 10);

        ghoulDamage = 1 + round;
        goblinDamage = 5 + round;
        knightDamage = 15 + round;
    }

    private IEnumerator SpawnMonsters(int ghoulsTS, int goblinsTS, int knightsTS, float bloodPercent, float holyPercent, int zone)
    {
        canSpawn = true;
        Vector3 spawnLocation;
        yield return new WaitForSeconds(3f);
        while(canSpawn)
        {
            xLoc = Random.Range(spawnArea[zone, 0].x , spawnArea[zone, 1].x);
            zLoc = Random.Range(spawnArea[zone, 0].z, spawnArea[zone, 1].z);
            spawnLocation = new Vector3(xLoc, spawnArea[zone, 0].y, zLoc);
            if (ghoulsTS > ghoulsSpawned)
            {
                Instantiate(enemyPrefabs[Random.Range(0,3)], spawnLocation, Quaternion.identity);
                yield return new WaitForSeconds(0.1f);
                ghoulsSpawned++;
            }
            xLoc = Random.Range(spawnArea[zone, 0].x , spawnArea[zone, 1].x);
            zLoc = Random.Range(spawnArea[zone, 0].z, spawnArea[zone, 1].z);
            spawnLocation = new Vector3(xLoc, spawnArea[zone, 0].y, zLoc);
            if (goblinsTS > goblinsSpawned)
            {
                Instantiate(enemyPrefabs[Random.Range(3,6)], spawnLocation, Quaternion.identity);
                yield return new WaitForSeconds(0.1f);
                goblinsSpawned++;
            }
            xLoc = Random.Range(spawnArea[zone, 0].x , spawnArea[zone, 1].x);
            zLoc = Random.Range(spawnArea[zone, 0].z, spawnArea[zone, 1].z);
            spawnLocation = new Vector3(xLoc, spawnArea[zone, 0].y, zLoc);
            if (knightsTS > knightsSpawned)
            {
                Instantiate(enemyPrefabs[Random.Range(6,8)], spawnLocation, Quaternion.identity);
                yield return new WaitForSeconds(0.1f);
                knightsSpawned++;
            }
            if (ghoulsTS == ghoulsSpawned && goblinsTS == goblinsSpawned && knightsTS == knightsSpawned)
            {
                Debug.Log($"Spawned {ghoulsSpawned + goblinsSpawned + knightsSpawned} monsters\nIndividual Spawned:\nGhouls: {ghoulsSpawned}\nGoblins:{goblinsSpawned}\nKnights:{knightsSpawned}");
                canSpawn = false;
            }
        }
        
    }
}
