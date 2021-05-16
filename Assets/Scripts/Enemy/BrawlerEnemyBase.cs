using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrawlerEnemyBase : MonoBehaviour
{

    public static readonly string APPEAR_EFFECT = "Appear";
    public static readonly string DEATH_EFFECT = "Die";

    private EnemyController ctrl = null;

    [Header("-1: attacks player without checking distance")]
    public float enemyAttackRadius = -1.0f;

    [SerializeField]
    private BrawlerEnemyEffectsScriptableObject effects;

    void Start()
    {
        ctrl = gameObject.GetComponent<EnemyController>();
        //If the radius is negative, then start moving
        if (enemyAttackRadius < 0.0f)
            if (ctrl)
                ctrl.StartMoving();

        //Spawn appear effect
        EnemySpawnObject effect_ = effects.GetEffect(APPEAR_EFFECT);
        if (effect_)
            Instantiate(effect_, gameObject.transform.position, gameObject.transform.rotation);
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
            if (!ctrl.b_Moving && !EnemyManager.s_enmInstance.bEngagedInBattle)
            {
                float distanceToPlayer = (EnemyManager.s_enmInstance.playerRef.transform.position - gameObject.transform.position).magnitude;

                if (distanceToPlayer <= enemyAttackRadius)
                {
                    ctrl.StartMoving();
                }
            }
        }
    }

    void OnDestroy()
    {
        //Spawn death effect
        EnemySpawnObject effect_ = effects.GetEffect(DEATH_EFFECT);
        if (effect_)
            Instantiate(effect_,gameObject.transform.position, gameObject.transform.rotation);
    }
}
