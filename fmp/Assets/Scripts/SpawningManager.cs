using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningManager : MonoBehaviour
{
    public int zoneToSpawn;
    public int round, enemiesLeft, enemiesToSpawn, spawnChance, enemyIndex;
    public GameObject[] enemyPrefabs;
    void Start() => round = 0;
    void Update()
    {
        zoneToSpawn = ZoneManager.activeZone;
        enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length;
        //if(enemiesLeft == 0) 
        StartNewRound();
    }

    private void StartNewRound()
    {
        
        round++;
        spawnChance = Random.Range(1,101);
        switch (spawnChance)
        {
            case var n when (spawnChance >= 0 && spawnChance <= 10):
                enemyIndex = 1;
                break;
            case var n when (spawnChance >= 10 && spawnChance <= 20):
                enemyIndex = 2;
                break;
            case var n when (spawnChance >= 20 && spawnChance <= 30):
                enemyIndex = 3;
                break;
            default:
                enemyIndex = 4;
                break;
        }
    }
}
