using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BotLocator : MonoBehaviour {

    public float targetLockRange;
    public bool lockThroughWalls = false;

    Collider2D currentTarget;
    Collider2D ctrlCollider;

    public LayerMask enemyMask;
    public LayerMask obstacleMask;

    protected DebugRay targetConnectionRay;

    public void Awake()
    {
        ctrlCollider = GetComponent<Collider2D>();
    }

    public float GetDistanceToTarget()
    {
        return (!currentTarget) ? float.MaxValue : Vector2.Distance(transform.position, currentTarget.transform.position);
    }

    public virtual Collider2D LocateEnemy()
    {
        if(!currentTarget)
        {
            currentTarget = Detect();
        }
        else if (Vector2.Distance(transform.position, currentTarget.transform.position) >= targetLockRange)
        {
            currentTarget = Detect();
        }

        if(currentTarget != null)
        {
            Vector2 direction = currentTarget.transform.position - transform.position;

            targetConnectionRay = new DebugRay
            {
                origin = transform.position,
                direction = direction.normalized,
                magnitude = direction.magnitude,
            };
        } else
        {
            targetConnectionRay = null;
        }

        return currentTarget;
    }

    Collider2D Detect()
    {

        Collider2D[] hits = Physics2D.OverlapCircleAll(ctrlCollider.bounds.center, targetLockRange, enemyMask);

        for (int i = 0; i < hits.Length; i++)
        {
            if(lockThroughWalls)
            {
                return hits[i];
            }

            Vector2 direction = hits[i].transform.position - transform.position;
            RaycastHit2D obstacle = Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, obstacleMask & ~enemyMask);

            if (!obstacle)
            {
                return hits[i];
            }
        }

        return null;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetLockRange);

        if(targetConnectionRay != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(targetConnectionRay.origin, targetConnectionRay.direction * targetConnectionRay.magnitude);
        }
    }

    protected class DebugRay
    {
        public Vector2 origin;
        public Vector2 direction;
        public float magnitude;
    }
}
