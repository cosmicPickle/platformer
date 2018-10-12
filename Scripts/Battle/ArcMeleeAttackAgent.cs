using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcMeleeAttackAgent : AttackAgent
{
    public float attackSpeed = 0.5f;

    public bool showDebugWeapon = false;
    public GameObject debugWeapon;
    GameObject debugWeaponInstance;

    float lastAttackStart;
    bool attackInProgress = false;
    List<Hitbox> hitEnemies = new List<Hitbox>();

    LayerMask enemyMask;
    ArcInfo arcInfo = new ArcInfo();

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (attackInProgress && lastAttackStart + attackSpeed <= Time.time)
        {
            attackInProgress = false;
            hitEnemies = new List<Hitbox>();

            if (showDebugWeapon && debugWeaponInstance && debugWeaponInstance.activeSelf)
            {
                debugWeaponInstance.SetActive(false);
            }
        } else {
            CastAttack();
        }
       
    }

    public override void Attack(LayerMask collisionMask)
    {
        if(attackInProgress)
        {
            return;
        }

        attackInProgress = true;
        lastAttackStart = Time.time;
        enemyMask = collisionMask;

        if (showDebugWeapon)
        {
            if (debugWeaponInstance && !debugWeaponInstance.activeSelf)
            {
                debugWeaponInstance.SetActive(true);
                PositionAndScaleDebugWeapon();
            }
            else
            {
                if (debugWeapon)
                {
                    debugWeaponInstance = Instantiate(debugWeapon, transform.position, debugWeapon.transform.rotation, transform);
                    PositionAndScaleDebugWeapon();
                }
            }
        }
        CastAttack();
    }

    public override void Attack(Collider2D target, LayerMask collisionMask)
    {
        Attack(collisionMask);
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (attackInProgress)
            {
                CalculateArcInfo();
                DebugExtension.DrawCircle(arcInfo.center, new Vector3(0, 0, 1), Color.yellow, arcInfo.radius - RaycastController.skinWidth);
            }
        }
    }

    void CastAttack()
    {
        if(!attackInProgress)
        {
            return;
        }

        CalculateArcInfo();
        Collider2D[] hits = Physics2D.OverlapCircleAll(arcInfo.center, arcInfo.radius - RaycastController.skinWidth, enemyMask);

        for(int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];

            Hitbox hitbox = hit.GetComponent<Hitbox>();

            if (hitbox && !hitEnemies.Contains(hitbox))
            {
                hitEnemies.Add(hitbox);
                hitbox.Damage(damage, knockbackEffect, controller.collisions.faceDir, knockbackDuration);
            }
        }
    }

    void CalculateArcInfo()
    {
        print(controller);
        arcInfo.radius = attackRange = attackRange > 0 ? attackRange : (controller.raycastOrigins.topRight.x - controller.raycastOrigins.topLeft.x) / 2;
        arcInfo.center = (Vector2)controller.transform.position + Vector2.right * arcInfo.radius * controller.collisions.faceDir;
    }

    void PositionAndScaleDebugWeapon()
    {
        debugWeaponInstance.transform.position = new Vector3(
            transform.position.x + controller.collisions.faceDir * attackRange,
            transform.position.y,
            transform.position.z
        );
        debugWeaponInstance.transform.localScale = new Vector3(debugWeaponInstance.transform.localScale.x, attackRange * 2);
    }

    struct ArcInfo
    {
        public float radius;
        public Vector2 center;
    }
}
