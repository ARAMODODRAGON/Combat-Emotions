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

    private static List<EnemyController> s_enemyControllers;
    public readonly PlayerController playerRef = null;


    public static int countEnemies => s_enemyControllers.Count;
    public static EnemyController GetEnemy(int index) => s_enemyControllers[index];

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


    public void BeginEncounter()
    {
        //TODO
        //Instantiate the enemies
        s_enemyControllers[0].b_Moving = true;

    }
}
