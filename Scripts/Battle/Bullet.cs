using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public Vector2 knockbackEffect;
    [HideInInspector]
    public float knockbackDuration;

    protected Vector2 velocity;
    protected Collider2D ctrlCollider;
    RaycastOrigins raycastOrigins;

    public LayerMask obstacleMask;
    LayerMask targetMask;
    LayerMask compoundMask;

    Vector2 initialPosition;
    float bulletRange;

    void Start()
    {
        ctrlCollider = GetComponent<Collider2D>();
    }

    public void Shoot(Vector2 direction, float speed, float range, LayerMask collisionMask)
    {
        velocity = direction * speed;
        compoundMask = obstacleMask | collisionMask;
        targetMask = collisionMask;
        initialPosition = transform.position;
        bulletRange = range;
    }

    // Update is called once per frame
    void Update()
    {
        float moveDistance = Mathf.Sqrt(
            Mathf.Pow(transform.position.x - initialPosition.x, 2) + 
            Mathf.Pow(transform.position.y - initialPosition.y, 2)
        );

        if(moveDistance >= bulletRange)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 moveAmount = velocity * Time.deltaTime;

        transform.Translate(moveAmount);
        UpdateRaycastOrigins();

        RaycastHit2D hit = Physics2D.Raycast(raycastOrigins.center, moveAmount.normalized, moveAmount.magnitude, compoundMask);

        Debug.DrawRay(raycastOrigins.center, moveAmount, Color.red);

        if(hit)
        {
            if(targetMask == (targetMask | 1 << hit.collider.gameObject.layer))
            {
                Hitbox hitbox = hit.collider.GetComponent<Hitbox>();

                if(hitbox)
                {
                    hitbox.Damage(damage, knockbackEffect, Mathf.Sign(moveAmount.x), knockbackDuration);
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
