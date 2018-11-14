using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BotLocator))]
[RequireComponent(typeof(BotAttacker))]
[RequireComponent(typeof(Hitbox))]
[RequireComponent(typeof(Controller2D))]
public class BotFlyerNavigator : MonoBehaviour, IBotNavigator
{

    public bool chaseOusideBounds = false;
    public Stat moveSpeed = 6;


    public float enemyStoppingDistancePercent = 80;
    public bool canBeKnockedBack = true;

    public Vector2[] patrolTargets;
    protected Vector2[] globalPatrolTargets;
    protected Vector2[] patrolBounds = new Vector2[2];

    protected BotLocator locator;
    protected BotAttacker attacker;
    protected Hitbox hitbox;
    protected Hitbox.Knockback currentKnockback;
    protected Controller2D controller;

    protected Vector2 currentTarget;
    protected Transform enemyInRange;
    protected int currentPatrolTarget = 0;

    protected bool stopped = true;
    protected bool patroling = false;
    protected bool keepingDistance = false;


    protected void Awake()
    {
        locator = GetComponent<BotLocator>();
        attacker = GetComponent<BotAttacker>();
        controller = GetComponent<Controller2D>();

        hitbox = GetComponent<Hitbox>();
        hitbox.onDamageTaken += OnDamageTaken;

        globalPatrolTargets = new Vector2[patrolTargets.Length];
        
        for (int i = 0; i < patrolTargets.Length; i++)
        {
            globalPatrolTargets[i] = patrolTargets[i] + (Vector2)transform.position;
        }

        CalculatePatrolBounds();
    }

    protected void Update()
    {
        if(currentKnockback.duration > 0)
        {
            HandleKnockback();
            return;
        }

        if (!stopped)
        {
            Vector2 moveAmount = (currentTarget - (Vector2)transform.position).normalized * moveSpeed * Time.deltaTime;
            HandleReachedDestination(ref moveAmount);
            Vector2 navDirection = new Vector2(Mathf.Sign(moveAmount.x), 0);

            controller.Move(moveAmount, navDirection);
        }

        if (patroling)
        {
            ResetPatrolTarget();
        }

        if (keepingDistance)
        {
            ResetEnemyTarget();
        }

    }

    public bool IsTargetOnPath(Vector2 target)
    {
        if(chaseOusideBounds)
        {
            return true;
        }

        if (globalPatrolTargets == null || globalPatrolTargets.Length  < 2)
            return false;

        return target.x >= patrolBounds[0].x
            && target.y >= patrolBounds[0].y
            && target.x <= patrolBounds[1].x
            && target.y <= patrolBounds[1].y;
    }

    public bool GetKnockbackStatus()
    {
        return currentKnockback.duration > 0;
    }

    public void KeepAttackDistance()
    {
        Resume();
        keepingDistance = true;
        patroling = false;
        enemyInRange = locator.LocateEnemy().transform;
        currentTarget = enemyInRange.position;
    }

    public void Patrol()
    {
        Resume();
        patroling = true;
        keepingDistance = false;
        currentPatrolTarget = 0;
        currentTarget = globalPatrolTargets[currentPatrolTarget];
    }

    public void Pause()
    {
        stopped = true;
    }

    public void Resume()
    {
        stopped = false;
    }

    public void Stop()
    {
        stopped = true;
        currentTarget = transform.position;
        patroling = false;
        keepingDistance = false;
        enemyInRange = null;
        currentPatrolTarget = 0;
    }

    protected void ResetEnemyTarget()
    {
        currentTarget = enemyInRange.position;
        currentTarget += ((Vector2)transform.position - currentTarget).normalized * enemyStoppingDistancePercent / 100 * attacker.range;
    }

    protected void ResetPatrolTarget()
    {
        if((Vector2)transform.position == currentTarget)
        {
            currentPatrolTarget++;

            if(currentPatrolTarget >= globalPatrolTargets.Length)
            {
                currentPatrolTarget = 0;
            }

            currentTarget = globalPatrolTargets[currentPatrolTarget];
        }
    }

    protected void CalculatePatrolBounds()
    {
        if (globalPatrolTargets.Length < 2)
            return;

        patrolBounds[0] = patrolBounds[1] = globalPatrolTargets[0];

        for (int i = 0; i < globalPatrolTargets.Length; i++) {
            Vector2 gpt = globalPatrolTargets[i];

            if (gpt.x < patrolBounds[0].x)
                patrolBounds[0].x = gpt.x;
            if (gpt.y < patrolBounds[0].y)
                patrolBounds[0].y = gpt.y;
            if (gpt.x > patrolBounds[1].x)
                patrolBounds[1].x = gpt.x;
            if (gpt.y > patrolBounds[1].y)
                patrolBounds[1].y = gpt.y;
        }
    }

    protected void HandleReachedDestination(ref Vector2 moveAmount)
    {
        Vector2 distance = currentTarget - (Vector2)transform.position;
        if(moveAmount.magnitude > distance.magnitude)
        {
            moveAmount = distance;
        }
    }

    protected void HandleKnockback()
    {
        Vector2 knockbackVelocity = currentKnockback.direction * currentKnockback.speed;
        Vector2 navDirection = new Vector2(-Mathf.Sign(knockbackVelocity.x), 0);

        controller.Move(knockbackVelocity * Time.deltaTime, navDirection);
        currentKnockback.duration -= Time.deltaTime;        
    }

    protected virtual void OnDamageTaken(float amount, Hitbox.Knockback knockback)
    {
        print(currentKnockback);
        if (canBeKnockedBack)
        {
            currentKnockback = knockback;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        float size = .3f;

        if (patrolTargets != null)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < patrolTargets.Length; i++)
            {
                Vector3 globalWaypointPosition = (Application.isPlaying) ? globalPatrolTargets[i] : patrolTargets[i] + (Vector2)transform.position;
                Gizmos.DrawLine(globalWaypointPosition - Vector3.up * size, globalWaypointPosition + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPosition - Vector3.left * size, globalWaypointPosition + Vector3.left * size);
            }
        }

        if(globalPatrolTargets != null && globalPatrolTargets.Length > 2)
        {
            Vector2 p1 = new Vector2(patrolBounds[0].x, patrolBounds[1].y);
            Vector2 p2 = new Vector2(patrolBounds[1].x, patrolBounds[0].y);

            Gizmos.color = Color.gray;

            Gizmos.DrawLine(patrolBounds[0], p1);
            Gizmos.DrawLine(p1, patrolBounds[1]);
            Gizmos.DrawLine(patrolBounds[1], p2);
            Gizmos.DrawLine(p2, patrolBounds[0]);
        }
    }
}
