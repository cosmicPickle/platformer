using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour {

    public Stat damage = 10;
    public Stat knockbackDuration = 0.05f;
    public Stat knockbackSpeed = 20f;

    public bool instakill = false;

    public GameObject projectilePrefab;
    public Vector3[] launchPositions;
    public Vector2 projectileDirection = Vector2.up;
    public float projectileSpeed = 5;
    public float launchInterval = 1;
    public float minMoveDistance = .5f;

    public LayerMask hitMask;
    public LayerMask obstacleMask;
    public float lifetime = 2;

    float launchTimer;
    List<Projectile> launchedProjectiles = new List<Projectile>();

    private void Start()
    {
        Launch();
        launchTimer = launchInterval;
    }
    private void Update()
    {
        if (launchTimer > 0)
        {
            launchTimer -= Time.deltaTime;
        }
        else
        {
            launchTimer = launchInterval;
            Launch();
        }

        for (int i = 0; i < launchedProjectiles.Count; i++)
        {
            Projectile proj = launchedProjectiles[i];

            if (proj.lifetime <= 0 && !proj.destroyed)
            {
                Destroy(proj.collider.gameObject);
                proj.destroyed = true;
                launchedProjectiles[i] = proj;
                continue;
            } else
            {
                if(proj.destroyed)
                {
                    continue;
                }
                proj.lifetime -= Time.deltaTime;

                Vector3 moveAmount = (Vector3)projectileDirection * projectileSpeed * Time.deltaTime;
                proj.moveDistance += moveAmount.magnitude;

                proj.collider.transform.position += moveAmount;

                if (proj.moveDistance >= minMoveDistance)
                {
                    CheckCollisions(ref proj);
                }

                launchedProjectiles[i] = proj;
            }
        }

        launchedProjectiles.RemoveAll(p => p.destroyed);
    }

    void Launch()
    {

        if (launchPositions != null)
        {
            for (int i = 0; i < launchPositions.Length; i++)
            {
                Vector2 pos = transform.position + launchPositions[i];
                Collider2D projectile = Instantiate(projectilePrefab, pos, transform.rotation).GetComponent<Collider2D>();

                launchedProjectiles.Add(new Projectile
                {
                    collider = projectile,
                    lifetime = lifetime,
                    moveDistance = 0,
                    destroyed = false
                });

            }
        }
    }

    void CheckCollisions(ref Projectile proj)
    {

        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = hitMask | obstacleMask;

        Collider2D[] hits = new Collider2D[5];
        proj.collider.OverlapCollider(filter, hits);
        List<Hitbox> hitTargets = new List<Hitbox>(hits.Length);

        for (int j = 0; j < hits.Length; j++)
        {
            if (hits[j] == null)
            {
                continue;
            }

            Destroy(proj.collider.gameObject);
            proj.destroyed = true;

            if (hitMask == (hitMask | 1 << hits[j].gameObject.layer))
            {
                Hitbox hitbox = hits[j].GetComponent<Hitbox>();

                if (!hitbox)
                {
                    continue;
                }

                if (hitTargets.Contains(hitbox))
                {
                    continue;
                }

                if (instakill)
                {
                    hitbox.Die();
                }
                else
                {
                    hitbox.Damage(damage.GetValue(), new Hitbox.Knockback
                    {
                        duration = knockbackDuration.GetValue(),
                        direction = projectileDirection.x != 0 ? new Vector2(Mathf.Sign(projectileDirection.x), 0) : Vector2.zero,
                        speed = knockbackSpeed.GetValue()
                    });
                }
            }

            
        }
    }

    private void OnDrawGizmos()
    {
        if (launchPositions != null)
        {
            float size = .3f;
            for (int i = 0; i < launchPositions.Length; i++)
            {
                Vector2 pos = transform.position + launchPositions[i];

                Gizmos.color = Color.red;
                Gizmos.DrawLine(pos - Vector2.up * size, pos + Vector2.up * size);
                Gizmos.DrawLine(pos - Vector2.left * size, pos + Vector2.left * size);
            }
        }
    }

    struct Projectile
    {
        public Collider2D collider;
        public float lifetime;
        public float moveDistance;
        public bool destroyed;
    }

    
}
