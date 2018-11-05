using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class AttackAgent : MonoBehaviour
{

    public float range;

    public Stat attack;
    public Stat knockback;
    public Stat knockbackSpeed;

    public Stat attackRate;
    public float attackDuration;

    public LayerMask enemyMask { get; protected set; }
    public LayerMask obstacleMask { get; protected set; }

    public delegate void OnAttackStart();
    public delegate void OnAttackComplete();

    public OnAttackStart onAttackStart;
    public OnAttackComplete onAttackComplete;

    protected float timeToNextAttack;
    protected float timeToAttackComplete;
    protected bool isAttacking;

    public Transform target { get; protected set; }
    protected List<Collider2D> hitTargets;
    protected Controller2D controller;

    void Awake()
    {
        controller = GetComponent<Controller2D>();
    }

    public void Init(LayerMask enemies, LayerMask obstacles)
    {
        enemyMask = enemies;
        obstacleMask = obstacles;
    }

    protected virtual void Update()
    {
        if (timeToNextAttack > 0)
        {
            timeToNextAttack -= Time.deltaTime;
        }
        else
        {
            timeToNextAttack = 0;
        }

        if (timeToAttackComplete > 0)
        {
            timeToAttackComplete -= Time.deltaTime;
        }
        else
        {
            if (isAttacking)
            {
                timeToAttackComplete = 0;
                isAttacking = false;

                if (onAttackComplete != null)
                {
                    onAttackComplete();
                }
            }
        }
    }

    public void Attack()
    {
        Attack(null);
    }

    public void Attack(Transform newTarget)
    {
        if (enemyMask == 0)
        {
            Debug.LogWarning("AttackAgent's EnemyMask is not being set. No damage will be done.");
        }
        if (timeToNextAttack > 0)
            return;

        timeToNextAttack = 1 / attackRate;

        target = newTarget;
        timeToAttackComplete = attackDuration;
        isAttacking = true;
        hitTargets = new List<Collider2D>();

        if (onAttackStart != null)
        {
            onAttackStart();
        }

        OnAttack();
    }

    protected virtual void OnAttack()
    {

    }
}
