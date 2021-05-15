using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrawlerEnemyBase : MonoBehaviour
{
    private EnemyController ctrl = null;

    [Header("-1: attacks player without checking distance")]
    public float enemyAttackRadius = -1.0f;

    void Start()
    {
        ctrl = gameObject.GetComponent<EnemyController>();
        //If the radius is negative, then start moving
        if (enemyAttackRadius < 0.0f)
            if (ctrl)
                ctrl.b_Moving = true;
    }

    void Update()
    {
        CheckRadius();
    }

    private void CheckRadius()
    {
        //Check distance to player and move when the player is close enough
        if (ctrl)
        {
            if (!ctrl.b_Moving)
            {
                float distanceToPlayer = (EnemyManager.s_enmInstance.playerRef.transform.position - gameObject.transform.position).magnitude;

                if (distanceToPlayer <= enemyAttackRadius)
                {
                    ctrl.b_Moving = true;
                }
            }
        }
    }
}
