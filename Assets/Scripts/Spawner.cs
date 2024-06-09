using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemyPrefab;
    private float SpawnRangeX = 1;
    private float SpawnRangeY = 1;
    private int spawnLimit = 3;
    private int spawnedEnemies = 0;

    void Start()
    {
        InvokeRepeating("Spawn", 10, 2.2f);
    }

    void Update()
    {
        
    }

    public void Spawn()
    {
        if (spawnedEnemies < spawnLimit)
        {
            int index = Random.Range(0, enemyPrefab.Length);
            Vector3 spawnPos = new Vector3(transform.position.x + Random.Range(SpawnRangeX, -SpawnRangeX), transform.position.y + Random.Range(-SpawnRangeY, SpawnRangeY), 0);
            Instantiate(enemyPrefab[index], spawnPos, enemyPrefab[index].transform.rotation);
            spawnedEnemies++;
        }
    }
}
