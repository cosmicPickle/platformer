using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class AttackAgent : MonoBehaviour
{

    [Header("General Settings")]
    public float range;

    public Stat attack = 5;
    public Stat knockback = 0.05f;
    public Stat knockbackSpeed = 30;

    public Stat attackRate = 1;
    public float attackDuration = 0.2f;

    public LayerMask enemyMask { get; protected set; }
    public LayerMask obstacleMask { get; protected set; }

    public delegate void OnAttackStart();
    public delegate void OnAttackComplete();
    public delegate void OnRecalculateAttackArea(AttackArea attackArea);

    public OnAttackStart onAttackStart;
    public OnAttackComplete onAttackComplete;
    public OnRecalculateAttackArea onRecalculateAttackArea;

    protected float timeToNextAttack;
    protected float timeToAttackComplete;
    protected bool isAttacking;

    public Transform target { get; protected set; }
    protected List<Collider2D> hitTargets;
    protected Controller2D controller;

    protected AttackArea attackArea = new AttackArea();
    

    void Awake()
    {
        controller = GetComponent<Controller2D>();
    }

    public virtual void Init(LayerMask enemies, LayerMask obstacles)
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

    public List<Collider2D> GetHitTargets()
    {
        return hitTargets;
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

    protected virtual void RecalculateAttackArea()
    {

    }

    protected virtual void OnDrawGizmos()
    {
        if (isAttacking)
        {
            Gizmos.color = Color.green;
            Vector2 p1 = new Vector2(attackArea.leadingPoing.x, attackArea.trailingPoint.y);
            Vector2 p2 = new Vector2(attackArea.trailingPoint.x, attackArea.leadingPoing.y);

            Gizmos.DrawLine(attackArea.leadingPoing, p1);
            Gizmos.DrawLine(p1, attackArea.trailingPoint);
            Gizmos.DrawLine(attackArea.trailingPoint, p2);
            Gizmos.DrawLine(p2, attackArea.leadingPoing);
        }
    }

    public struct AttackArea
    {
        public Vector2 leadingPoing;
        public Vector2 trailingPoint;
        public Vector2 size;
    }
}
