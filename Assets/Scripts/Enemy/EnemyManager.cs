using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    #region singletonlogic
    private static EnemyManager s_instance;
    public static EnemyManager s_enmInstance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new EnemyManager();
            }

            return s_instance;
        }
    }

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            DontDestroyOnLoad(this);

        }
        else
        {
            Destroy(gameObject);
        }

    }
    #endregion

    private static List<EnemyController> s_enemyControllers = new List<EnemyController>();
    public PlayerController playerRef = null;


    public static int countEnemies => s_enemyControllers.Count;
    public static EnemyController GetEnemy(int index) => s_enemyControllers[index];

    private Dictionary<int, List<EnemySpawner>> enemySpawners = new Dictionary<int, List<EnemySpawner>>();
    public bool bEngagedInBattle = false; //Turns true once a battle starts


    private void Start()
    {
        
    }

    public int AddEnemyController(EnemyController ec_)
    {
        s_enemyControllers.Add(ec_);
        return countEnemies - 1;
 
    }

    public void RemoveEnemyContoller(int index_)
    {
        if(index_ >= 0 && index_ < countEnemies)
        {
            s_enemyControllers.RemoveAt(index_);
        }
    }


    public void BeginEncounter(int encounterIndex_)
    {
        //Called from the gate system once we enter a new area with new enemies
        //Instantiate the enemies
        foreach (EnemySpawner es in enemySpawners[encounterIndex_])
        {
            es.SpawnEnemy();
        }
        Invoke("ForceTheFirstEnemyToMove", 1.0f); //Give the enemy controllers time to register before forcing the first one to move

    }

    private void ForceTheFirstEnemyToMove()
    {
        if(s_enemyControllers.Count>0)
            if(s_enemyControllers[0])
                s_enemyControllers[0].StartMoving();
    }

    public void RegisterSpawner(int key_, EnemySpawner es_)
    {
        //Automating the creation of enemyspawner lists
        if(enemySpawners.ContainsKey(key_))
        {
            enemySpawners[key_].Add(es_);
        }
        else
        {
            List<EnemySpawner> les = new List<EnemySpawner>();
            les.Add(es_);
            enemySpawners.Add(key_, les);
        }
    }
}
