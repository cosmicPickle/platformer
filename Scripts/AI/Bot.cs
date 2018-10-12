using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent (typeof(Controller2D))]
[RequireComponent (typeof(AttackAgent))]
[RequireComponent(typeof(Hitbox))]
[RequireComponent(typeof(FreezeController))]
public class Bot : MonoBehaviour
{
    ActionState currentState = ActionState.Idle;
    enum ActionState { Idle, Patrol, Attack, Follow };
    enum BotType { Friendly, Enemy };

    public LayerMask enemyCollisionMask;
    public float targetLockRange = 10;
    public bool chaseOutsidePatrolRadius = true;
    CircularRaycastOrigins raycastOrigins;

    public float idleTime = 0.5f;
    float timeToIdleEnd;

    protected Vector2 initialPosition;
    public float minPatrolRadius = 5f;
    public float maxPatrolRadius = 15f;
    public float minPatrolTime = 3f;
    public float maxPatrolTime = 7f;
    

    protected float patrolRadius;
    float timeToPatrolEnd;

    public float attackRate = 1f;
    public DetectionInfo detectionInfo;
    protected float lastAttack;

    protected Controller2D controller;
    protected AttackAgent attackAgent;
    protected FreezeController freezeController;

    void Awake()
    {
        
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        controller = GetComponent<Controller2D>();
        freezeController = GetComponent<FreezeController>();
        attackAgent = GetComponent<AttackAgent>();

        detectionInfo = new DetectionInfo();
        detectionInfo.Reset();

        timeToIdleEnd = idleTime;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

        if(initialPosition == Vector2.zero && controller.collisions.below)
        {
            initialPosition = new Vector2(transform.position.x, transform.position.y);
        }

        UpdateRaycastOrigins();
        DetectEnemy();
        SetState();

        switch(currentState) {
            case ActionState.Idle: Idle(); break;
            case ActionState.Patrol: Patrol(); break;
            case ActionState.Attack:
                {
                    StartAttack();

                    if(Time.time >= lastAttack + attackRate && detectionInfo.selectedEnemy)
                    {
                        Collider2D[] hits = Physics2D.OverlapCircleAll(raycastOrigins.center, raycastOrigins.radius + attackAgent.attackRange, enemyCollisionMask);
                        
                        for(int i = 0; i < hits.Length; i++)
                        {
                            if(hits[i].gameObject == detectionInfo.selectedEnemy.gameObject)
                            {
                                lastAttack = Time.time;
                                attackAgent.Attack(detectionInfo.selectedEnemy, enemyCollisionMask);
                            }
                        }
                    }
                    break;
                }
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (freezeController.getFreeze())
        {
            return;
        }

        if (Application.isPlaying)
        {
            DebugExtension.DrawCircle(raycastOrigins.center, new Vector3(0, 0, 1), Color.yellow, raycastOrigins.radius + targetLockRange);
            DebugExtension.DrawCircle(raycastOrigins.center, new Vector3(0, 0, 1), Color.red, raycastOrigins.radius + attackAgent.attackRange);
        }
        if (patrolRadius != 0)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(
                new Vector2(initialPosition.x - patrolRadius, initialPosition.y),
                new Vector2(initialPosition.x + patrolRadius, initialPosition.y)
            );
        }
    }

    void DetectEnemy()
    {
        if (freezeController.getFreeze())
        {
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(raycastOrigins.center, raycastOrigins.radius + targetLockRange, enemyCollisionMask);
        List<Collider2D> hitsList = new List<Collider2D>();
        bool selectedEnemyInRange = false;

        if (hits.Length > 0)
        {
            for(int i = 0; i < hits.Length; i ++)
            {
                bool rayDrawn = false;

                Collider2D hit = hits[i];
                Vector2 direction = new Vector2(
                    hit.transform.position.x - transform.position.x,
                    hit.transform.position.y - transform.position.y
                );

                RaycastHit2D obstacle = Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, controller.collisionMask & ~enemyCollisionMask);
                
                if (hit == detectionInfo.selectedEnemy)
                {
                    Debug.DrawRay(transform.position, direction, Color.green);
                    selectedEnemyInRange = true;
                    rayDrawn = true;
                }

                if (!obstacle) {
                    hitsList.Add(hit);
                    if (!rayDrawn)
                    {
                        Debug.DrawRay(transform.position, direction, Color.gray);
                    }
                } else
                {
                    if (!rayDrawn)
                    {
                        Debug.DrawRay(transform.position, direction, Color.red);
                    }
                }
            }

            if(hitsList.Count > 0)
            {
                if (!selectedEnemyInRange || !detectionInfo.selectedEnemy)
                {
                    detectionInfo.selectedEnemy = null;
                    detectionInfo.detectedEnemies = hitsList;
                }

                detectionInfo.enemyInRange = true;
            } else
            {
                if (!selectedEnemyInRange)
                {
                    detectionInfo.Reset();
                }
            }
        } else
        {
            detectionInfo.Reset();
        }
    }

    void SetState()
    {
        if (freezeController.getFreeze())
        {
            currentState = ActionState.Idle;
            return;
        }

        if (GetComponent<Hitbox>().GetKnockbackStatus())
        {
            currentState = ActionState.Idle;
            return;
        }

        if (detectionInfo.enemyInRange)
        {
            if (Mathf.Abs(transform.position.x - initialPosition.x) <= patrolRadius || chaseOutsidePatrolRadius)
            {
                currentState = ActionState.Attack;
                if (!detectionInfo.selectedEnemy)
                {
                    int enemyId = Random.Range(0, detectionInfo.detectedEnemies.Count - 1);
                    detectionInfo.selectedEnemy = detectionInfo.detectedEnemies[enemyId];
                }
                return;
            }
        }

        if (currentState == ActionState.Attack)
        {
            currentState = ActionState.Idle;
        }

        if (currentState == ActionState.Idle)
        {
            if (timeToIdleEnd > 0)
            {
                timeToIdleEnd -= Time.deltaTime;
            }
            else
            {
                currentState = ActionState.Patrol;
                timeToPatrolEnd = Random.Range(minPatrolTime, maxPatrolTime);
                patrolRadius = Random.Range(minPatrolRadius, maxPatrolRadius);

            }
        }

        if (currentState == ActionState.Patrol)
        {
            if (timeToPatrolEnd > 0)
            {
                timeToPatrolEnd -= Time.deltaTime;
            }
            else
            {
                currentState = ActionState.Idle;
                timeToIdleEnd = idleTime;
            }
        }
    }

    protected virtual void UpdateRaycastOrigins()
    {
        Bounds bounds = controller.ctrlCollider.bounds;
        bounds.Expand(RaycastController.skinWidth * -2);
        raycastOrigins.center = bounds.center;

        raycastOrigins.radius = Vector2.Distance(raycastOrigins.center, new Vector2(bounds.min.x, bounds.min.y));
    }

    protected virtual void Idle()
    {

    }

    protected virtual void Patrol()
    {

    }

    protected virtual void StartAttack()
    {

    }

    public struct DetectionInfo
    {
        public bool enemyInRange;
        public List<Collider2D> detectedEnemies;
        public Collider2D selectedEnemy;

        public void Reset()
        {
            enemyInRange = false;
            detectedEnemies = new List<Collider2D>();
            selectedEnemy = null;
        }
    }

    public struct CircularRaycastOrigins
    {
        public Vector2 center;
        public float radius;
    }
}
