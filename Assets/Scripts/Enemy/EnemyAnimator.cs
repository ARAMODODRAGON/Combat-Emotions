using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    public Animator anim { get; private set; } = null;


    private static readonly int ANIM_IDLE = Animator.StringToHash("Idle");
    private static readonly int ANIM_MOVE = Animator.StringToHash("Move");
    private static readonly int ANIM_DIE = Animator.StringToHash("Die");
    private static readonly int ANIM_ATTACK = Animator.StringToHash("Attack");
    private static readonly int ANIM_HURT = Animator.StringToHash("Hurt");


    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void ToggleIdle()
    {
        if (anim)
            anim.SetTrigger(ANIM_IDLE);
    }

    public void ToggleMove()
    {
        if (anim)
            anim.SetTrigger(ANIM_MOVE);
    }

    public void ToggleDie()
    {
        if (anim)
            anim.SetTrigger(ANIM_DIE);
    }

    public void Dead()
    {
        Destroy(gameObject);
    }

    public void Attack()
    {
        if (anim)
            anim.SetTrigger(ANIM_ATTACK);
    }

    public void Hurt()
    {
        if (anim)
            anim.SetTrigger(ANIM_HURT);
    }
}
