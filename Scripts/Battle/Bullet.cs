using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{

    Vector2 velocity;
    Collider2D ctrlCollider;
    RaycastOrigins raycastOrigins;

    LayerMask compoundMask;

    ShooterAttackAgent attackAgent;
    float trackDurationLeft;

    void Start()
    {
        ctrlCollider = GetComponent<Collider2D>();
    }

    public void Shoot(ShooterAttackAgent newAttackAgent)
    {
        attackAgent = newAttackAgent;
        Vector2 direction = (attackAgent.target.position - attackAgent.transform.position).normalized;

        if(attackAgent.trackType == ShooterAttackAgent.TrackType.None)
        {
            direction.y = 0;
        }
        else if (attackAgent.trackType == ShooterAttackAgent.TrackType.Track)
        {
            trackDurationLeft = attackAgent.trackDuration;
        }

        velocity = attackAgent.bulletSpeed * direction;
        compoundMask = attackAgent.obstacleMask | attackAgent.enemyMask;
    }

    // Update is called once per frame
    void Update()
    {
        float moveDistance = Vector2.Distance(attackAgent.transform.position, transform.position);

        if(moveDistance >= attackAgent.range)
        {
            Destroy(gameObject);
            return;
        }

        if (attackAgent.trackType == ShooterAttackAgent.TrackType.Track && trackDurationLeft > 0)
        {
            trackDurationLeft -= Time.deltaTime;
            velocity = attackAgent.bulletSpeed * (attackAgent.target.position - attackAgent.transform.position).normalized;
        }
        
        Vector2 moveAmount = velocity * Time.deltaTime;

        transform.Translate(moveAmount);
        UpdateRaycastOrigins();

        RaycastHit2D hit = Physics2D.Raycast(raycastOrigins.center, moveAmount.normalized, moveAmount.magnitude, compoundMask);

        Debug.DrawRay(raycastOrigins.center, moveAmount, Color.red);

        if(hit)
        {
            if(attackAgent.enemyMask == (attackAgent.enemyMask | 1 << hit.collider.gameObject.layer))
            {
                Hitbox hitbox = hit.collider.GetComponent<Hitbox>();

                if(hitbox)
                {
                    hitbox.Damage(attackAgent);
                }
            }
            Destroy(gameObject);
        }

        
    }

    protected virtual void UpdateRaycastOrigins()
    {
        Bounds bounds = ctrlCollider.bounds;
        bounds.Expand(RaycastController.skinWidth * -2);
        raycastOrigins.center = bounds.center;

        raycastOrigins.radius = Vector2.Distance(raycastOrigins.center, new Vector2(bounds.min.x, bounds.min.y));
    }

    struct RaycastOrigins
    {
        public float radius;
        public Vector2 center;
    }
}
