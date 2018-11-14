using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BotLocator))]
[RequireComponent(typeof(BotAttacker))]
[RequireComponent(typeof(Hitbox))]
[RequireComponent(typeof(Controller2D))]
public class BotNavigator : MonoBehaviour, IBotNavigator
{

    [Header("Movement Settings")]
    public Vector2 gravityDirection = new Vector2(0, -1);
    public float gravity = 50;
    public Stat moveSpeed = 6;

    [Header("Attack Settings")]
    public float enemyStoppingDistancePercent = 80;
    public bool canBeKnockedBack = true;

    [Header("Patrol Settings")]
    public Vector3[] patrolTargets;
    protected Vector3[] globalPatrolTargets;
    protected Vector3 minPatrolTarget, maxPatrolTarget;

    protected float navDirection;

    protected BotLocator locator;
    protected BotAttacker attacker;
    protected Controller2D controller;
    protected Hitbox hitbox;
    protected Hitbox.Knockback currentKnockback;

    protected Vector2 currentTarget;
    protected Transform enemyInRange;
    protected int currentPatrolTarget = 0;

    protected Vector2 velocity;
    protected float velocitySmoothing;

    protected bool stopped = true;
    protected bool patroling = false;
    protected bool keepingDistance = false;

    protected float accelerationTimeAirborne = .2f;
    protected float accelerationTimeGrounded = .1f;

    protected virtual void Awake()
    {
        locator = GetComponent<BotLocator>();
        attacker = GetComponent<BotAttacker>();

        hitbox = GetComponent<Hitbox>();
        hitbox.onDamageTaken += OnDamageTaken;

        controller = GetComponent<Controller2D>();

        globalPatrolTargets = new Vector3[patrolTargets.Length];

        if (gravityDirection == Vector2.zero)
        {
            Debug.LogWarning("Gravity not setup corectly");
            return;
        }

        for (int i = 0; i < patrolTargets.Length; i++)
        {
            globalPatrolTargets[i] = patrolTargets[i] + transform.position;

            if (gravityDirection.y != 0)
            {
                minPatrolTarget = i == 0 || minPatrolTarget.x > globalPatrolTargets[i].x ? globalPatrolTargets[i] : minPatrolTarget;
                maxPatrolTarget = i == 0 || maxPatrolTarget.x < globalPatrolTargets[i].x ? globalPatrolTargets[i] : maxPatrolTarget;
            } else
            {
                minPatrolTarget = i == 0 || minPatrolTarget.y > globalPatrolTargets[i].y ? globalPatrolTargets[i] : minPatrolTarget;
                maxPatrolTarget = i == 0 || maxPatrolTarget.y < globalPatrolTargets[i].y ? globalPatrolTargets[i] : maxPatrolTarget;
            }  
        }
    }

    protected virtual void Update()
    {
        Vector2 moveAmount = Vector2.zero;

        if(currentKnockback.duration > 0)
        {
            CalculateVelocity();
            moveAmount = velocity * Time.deltaTime;
            controller.Move(moveAmount, Vector2.zero);
            return;
        }

        if(stopped)
        {
            return;
        }

        CalculateVelocity();
        moveAmount = velocity * Time.deltaTime;
        HandleDestinationArrival(ref moveAmount);

        controller.Move(moveAmount, Vector2.right * navDirection);
        HandleGravity();

        if(patroling)
        {
            ResetPatrolTarget();
        }

        if(keepingDistance)
        {
            ResetEnemyTarget();
        }
    }

    public bool GetKnockbackStatus()
    {
        return currentKnockback.duration > 0;
    }

    public bool IsTargetOnPath(Vector2 target)
    {
        if (gravityDirection == Vector2.zero)
        {
            Debug.LogWarning("Gravity not setup corectly");
            return true;
        }

        if(gravityDirection.y != 0)
        {
            return target.x >= minPatrolTarget.x && target.x <= maxPatrolTarget.x;
        } else
        {
            return target.y >= minPatrolTarget.y && target.y <= maxPatrolTarget.y;
        }
    }

    public virtual void Patrol()
    {
        if(globalPatrolTargets.Length == 0)
        {
            Debug.LogWarning("Patrol targets not set");
            return;
        }

        Resume();
        patroling = true;
        keepingDistance = false;
        currentTarget = globalPatrolTargets[0];
        currentPatrolTarget = 0;
    }

    public virtual void KeepAttackDistance()
    {
        Resume();
        patroling = false;
        keepingDistance = true;
        enemyInRange = locator.LocateEnemy().transform;
        currentTarget = enemyInRange.position;
    }

    public virtual void Stop()
    {
        Pause();
        currentTarget = transform.position;
        patroling = false;
        keepingDistance = false;
        enemyInRange = null;
        currentPatrolTarget = 0;
    }

    public virtual void Pause()
    {
        stopped = true;
    }

    public virtual void Resume()
    {
        stopped = false;
    }

    protected virtual void CalculateVelocity()
    {
        if (gravityDirection == Vector2.zero)
        {
            Debug.LogWarning("Gravity not setup corectly");
            return;
        }

        bool grounded = false;

        if (gravityDirection.y != 0)
        {
            grounded = Mathf.Sign(gravityDirection.y) > 0 ? controller.collisions.above : controller.collisions.below;
        }
        else
        {
            grounded = Mathf.Sign(gravityDirection.x) > 0 ? controller.collisions.right : controller.collisions.left;
        }

        ChangeNavDirection();

        float targetVelocity = navDirection * moveSpeed;
        Vector2 knockbackVelocity = CalculateKnockbackVelocity();

        if (knockbackVelocity != Vector2.zero)
        {
            targetVelocity = gravityDirection.y != 0 ? knockbackVelocity.x : knockbackVelocity.y;
        }

        velocity.x = gravityDirection.y != 0
            ? Mathf.SmoothDamp(velocity.x, targetVelocity, ref velocitySmoothing, grounded ? accelerationTimeGrounded : accelerationTimeAirborne)
            : knockbackVelocity != Vector2.zero
                ? Mathf.Abs(currentKnockback.direction.x) == Mathf.Abs(gravityDirection.x)
                    ? currentKnockback.direction.x
                    : knockbackVelocity.x
                : velocity.x + Mathf.Sign(gravityDirection.x) * gravity * Time.deltaTime;

        velocity.y = gravityDirection.x != 0
            ? Mathf.SmoothDamp(velocity.y, targetVelocity, ref velocitySmoothing, grounded ? accelerationTimeGrounded : accelerationTimeAirborne)
            : knockbackVelocity != Vector2.zero
                ? Mathf.Abs(currentKnockback.direction.y) == Mathf.Abs(gravityDirection.y) 
                    ? currentKnockback.direction.y 
                    : knockbackVelocity.y
                : velocity.y + Mathf.Sign(gravityDirection.y) * gravity * Time.deltaTime;

    }

    protected virtual Vector2 CalculateKnockbackVelocity()
    {
        Vector2 v = Vector2.zero;
        if (currentKnockback.duration > 0)
        {
            if (currentKnockback.direction.x == 0)
            {
                currentKnockback.direction.x = -controller.collisions.faceDir;
            }

            float dir = (gravityDirection.y != 0) 
                ? - Mathf.Sign(gravityDirection.y) * Mathf.Sign(currentKnockback.direction.x) 
                : - Mathf.Sign(gravityDirection.x) * Mathf.Sign(currentKnockback.direction.y);
            float angle = - dir * Hitbox.Knockback.Angle;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
            Vector2 baseDirection = (gravityDirection.y != 0) 
                ? - gravityDirection.y * dir * Vector2.right * currentKnockback.speed 
                : - gravityDirection.x * dir * Vector2.up * currentKnockback.speed;

            v = new Vector2(baseDirection.x * cos + baseDirection.y * sin, -baseDirection.x * sin + baseDirection.y * cos);
            currentKnockback.duration -= Time.deltaTime;
        }
        else
        {
            currentKnockback.duration = 0;
        }

        return v;
    }

    protected virtual void HandleDestinationArrival(ref Vector2 moveAmount)
    {
        if (gravityDirection == Vector2.zero)
        {
            Debug.LogWarning("Gravity not setup corectly");
            return;
        }

        float distance = gravityDirection.y != 0 ? Mathf.Abs(currentTarget.x - transform.position.x) : Mathf.Abs(currentTarget.y - transform.position.y);

        if (gravityDirection.y != 0 && distance < Mathf.Abs(moveAmount.x))
        {
            moveAmount.x = currentTarget.x - transform.position.x;
            Pause();
        }
        else if (gravityDirection.x != 0 && distance < Mathf.Abs(moveAmount.y))
        {
            moveAmount.y = currentTarget.y - transform.position.y;
            Pause();
        }
    }

    protected virtual void HandleGravity()
    {
        if (gravityDirection == Vector2.zero)
        {
            Debug.LogWarning("Gravity not setup corectly");
            return;
        }

        if (gravityDirection.y != 0 && (controller.collisions.above || controller.collisions.below))
        {
            if (Mathf.Sign(gravityDirection.y) == Mathf.Sign(velocity.y))
                velocity.y = 0;
        }
        else if (gravityDirection.x != 0 && (controller.collisions.left || controller.collisions.right))
        {
            if (Mathf.Sign(gravityDirection.x) == Mathf.Sign(velocity.x))
                velocity.x = 0;
        }
    }

    protected virtual void ResetPatrolTarget()
    {
        if (gravityDirection == Vector2.zero)
        {
            Debug.LogWarning("Gravity not setup corectly");
            return;
        }

        if (gravityDirection.y != 0 && currentTarget.x == transform.position.x)
        {
            currentPatrolTarget++;

            if (currentPatrolTarget >= patrolTargets.Length)
            {
                currentPatrolTarget = 0;
            }

            currentTarget = globalPatrolTargets[currentPatrolTarget];
            Resume();
        }
        else if (gravityDirection.x != 0 && currentTarget.y == transform.position.y)
        {
            currentPatrolTarget++;

            if (currentPatrolTarget >= patrolTargets.Length)
            {
                currentPatrolTarget = 0;
            }

            currentTarget = globalPatrolTargets[currentPatrolTarget];
            Resume();
        }
    }

    protected virtual void ResetEnemyTarget()
    {
        currentTarget = enemyInRange.position;

        if (gravityDirection == Vector2.zero)
        {
            Debug.LogWarning("Gravity not setup corectly");
            return;
        }

        if (gravityDirection.y != 0)
        {
            currentTarget.x = currentTarget.x - Mathf.Sign(currentTarget.x - transform.position.x) * attacker.range * enemyStoppingDistancePercent / 100;
        }
        else
        {
            currentTarget.y = currentTarget.y - Mathf.Sign(currentTarget.y - transform.position.y) * attacker.range * enemyStoppingDistancePercent / 100;
        }
        Resume();
    }

    protected virtual void ChangeNavDirection()
    {
        if (gravityDirection == Vector2.zero)
        {
            Debug.LogWarning("Gravity not setup corectly");
            return;
        }

        navDirection = gravityDirection.y != 0 
            ? Mathf.Sign((currentTarget - (Vector2)transform.position).x)
            : Mathf.Sign((currentTarget - (Vector2)transform.position).y);
    }

    protected virtual void OnDamageTaken(float amount, Hitbox.Knockback knockback)
    {
        if (canBeKnockedBack)
        {
            currentKnockback = knockback;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (patrolTargets != null)
        {
            Gizmos.color = Color.green;
            float size = .3f;

            for (int i = 0; i < patrolTargets.Length; i++)
            {
                Vector3 globalWaypointPosition = (Application.isPlaying) ? globalPatrolTargets[i] : patrolTargets[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPosition - Vector3.up * size, globalWaypointPosition + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPosition - Vector3.left * size, globalWaypointPosition + Vector3.left * size);
            }
        }
    }
}
