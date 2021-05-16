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

    public CombatHandler combatHandler = null;

    [SerializeField] private GameObject m_easyEnemy;
    [SerializeField] private GameObject m_normalEnemy;
    [SerializeField] private GameObject m_hardEnemy;

    public IntValue levelValue;

    [SerializeField] private int m_enemiesPer5Levels = 2;
    public int levelCap = 30;

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

            if(s_enemyControllers.Count == 0)
            {
				//Call Sean's stuff to unlock camera
				if (playerRef != null)
				{
					playerRef.GetCamera().ChangeState(CameraStates.MoveRight);
				}
            }
        }
    }

    public static int ClampInt(int t, int min, int max) {
        if (t < min) return min;
        if (t > max) return max;
        return t;
	}

	public void SpawnEnemies(Vector3 position, Vector3 maxpos) {

        int level5 = levelValue.Value / 5;
        int maxlevel5 = levelCap / 5;
        int enemyCount = ClampInt(level5 + Random.Range(-1, 1), 1, levelCap / 5) * m_enemiesPer5Levels;

        List<int> spawnchances = new List<int>();
        spawnchances.Add((maxlevel5 - level5) * 80);
        spawnchances.Add((level5) * 30);
        spawnchances.Add((level5) * 5);

		for (int i = 0; i < enemyCount; i++) {

            GameObject go = null; 
            Vector3 pos = Vector3.Lerp(position, maxpos, Random.Range(0.2f, 1.0f));
            pos.y = Random.Range(-7f, 7f);
            int chance = Random.Range(0, 2);

            int spawn = -1;

			for (int j = 0; j < spawnchances.Count; j++) {
                if (spawnchances[j] > chance) {
                    spawn = j;
                    break;
                } else chance -= spawnchances[j];
			}

			switch (spawn) {
                case 0: go = Instantiate(m_easyEnemy, pos, Quaternion.identity); break;
                case 1: go = Instantiate(m_normalEnemy, pos, Quaternion.identity); break;
                case 2: go = Instantiate(m_hardEnemy, pos, Quaternion.identity); break;
				default: goto case 2;
			}

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
