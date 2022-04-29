using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class SpawningManager : MonoBehaviour
{
    private const float ghoulPercentage = 0.6f, goblinPercentage = 0.3f, knightPercentage = 0.1f;

    public Vector3[,] spawnArea;


    [SerializeField]
    [Header("Ghoul Statistics")]
    private int ghoulsToSpawn;
    [SerializeField]
    private float ghoulHealth, ghoulDamage;

    [SerializeField]
    [Header("Goblin Statistics")]
    private int goblinsToSpawn;
    [SerializeField]
    private float goblinHealth, goblinDamage;

    [SerializeField]
    [Header("Knight Statistics")]
    private int knightsToSpawn;
    [SerializeField]
    private float knightHealth, knightDamage;

    [Space(10)]
    public GameObject[] enemyPrefabs;
    
    public int round, enemiesLeft, enemiesToSpawn, zoneToSpawn;
    public TMP_Text RoundDisplay, EnemyRemainingDisplay;

    public int ghoulsSpawned = 0, goblinsSpawned = 0, knightsSpawned = 0;
    public float xLoc, zLoc;

    private float totalPercentage, bloodPercentage, holyPercentage, remainingPercentage;

    void Start()
    {
        round = 1;

        spawnArea[1, 0] = new Vector3(18, 1, 12);
        spawnArea[1, 1] = new Vector3(0, 1, 20);

        spawnArea[2, 0] = new Vector3(0, 1, 0);
        spawnArea[2, 1] = new Vector3(0, 1, 0);

    }

    void Update()
    {
        CalculateSpawnChance(round, false, false);
        CalculateMonsterStats(round);
        zoneToSpawn = ZoneManager.activeZone;
        enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if(enemiesLeft == 0) 
            StartNewRound();
        RoundDisplay.text = round.ToString();
        EnemyRemainingDisplay.text = enemiesLeft.ToString();
    }

    private void StartNewRound()
    {
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

    private void SpawnMonsters(int ghoulsTS, int goblinsTS, int knightsTS, float bloodPercent, float holyPercent, int zone)
    {
        
        while(ghoulsTS < ghoulsSpawned)
        {
            xLoc = Random.Range(spawnArea[zone, 0].x , spawnArea[zone, 1].x);
            zLoc = Random.Range(spawnArea[zone, 0].z, spawnArea[zone, 1].z);
            Vector3 spawnLocation = new Vector3(xLoc, spawnArea[zone, 0].y, zLoc);
            Instantiate(enemyPrefabs[0], spawnLocation, Quaternion.identity);
            ghoulsSpawned++;
        }
        while (goblinsTS < goblinsSpawned)
        {
            xLoc = Random.Range(spawnArea[zone, 0].x, spawnArea[zone, 1].x);
            zLoc = Random.Range(spawnArea[zone, 0].z, spawnArea[zone, 1].z);
            Vector3 spawnLocation = new Vector3(xLoc, spawnArea[zone, 0].y, zLoc);
            Instantiate(enemyPrefabs[1], spawnLocation, Quaternion.identity);
            goblinsSpawned++;
        }
        while (knightsTS < knightsSpawned)
        {
            xLoc = Random.Range(spawnArea[zone, 0].x, spawnArea[zone, 1].x);
            zLoc = Random.Range(spawnArea[zone, 0].z, spawnArea[zone, 1].z);
            Vector3 spawnLocation = new Vector3(xLoc, spawnArea[zone, 0].y, zLoc);
            Instantiate(enemyPrefabs[2], spawnLocation, Quaternion.identity);
            knightsSpawned++;
        }
    }
}
