using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Matches the encounter index")]
    [SerializeField]
    private int enemySpawnerKey = -1;
    [SerializeField]
    private EnemyController enemyToSpawn = null;


    void Start()
    {
        EnemyManager.s_enmInstance.RegisterSpawner(enemySpawnerKey,this);
    }

    public void SpawnEnemy()
    {
        if (enemyToSpawn)
            Instantiate(enemyToSpawn, transform.position, transform.rotation);
    }
}
