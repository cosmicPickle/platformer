using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackAgent : AttackAgent
{

    public AttackDirection attackDirection;
    AttackArea attackArea = new AttackArea();

    protected override void Update()
    {
        base.Update();

        if (isAttacking)
        {
            RecalculateAttackArea();

            Vector2[] positions = new[] { attackArea.leadingPoing, attackArea.trailingPoint };
            List<float> metrics = new List<float> { attackArea.size.x, attackArea.size.y };
            string layer = LayerMask.LayerToName(gameObject.layer);

            //Telegrams.TelegramType type = layer == "Player" || layer == "Friendly" ? Telegrams.TelegramType.Friendly : Telegrams.TelegramType.Hostile;

            //Telegrams.instance.DrawRetangle("Attack_" + gameObject.GetInstanceID(), positions.Centroid(), type, metrics);

            Collider2D[] hits = Physics2D.OverlapAreaAll(attackArea.leadingPoing, attackArea.trailingPoint, enemyMask);

            for (int i = 0; i < hits.Length; i++)
            {
                Vector2 heading = hits[i].transform.position - transform.position;
                RaycastHit2D obstacle = Physics2D.Raycast(transform.position, heading.normalized, heading.magnitude, obstacleMask & ~enemyMask);

                Hitbox enemyCharacter = hits[i].GetComponent<Hitbox>();

                if (!obstacle && !hitTargets.Contains(hits[i]))
                {
                    enemyCharacter = hits[i].GetComponent<Hitbox>();

                    if (enemyCharacter != null)
                    {
                        enemyCharacter.Damage(this);
                        hitTargets.Add(hits[i]);
                    }
                }
            }
        }
        else
        {
           // Telegrams.instance.Hide("Attack_" + gameObject.GetInstanceID());
        }
    }

    protected override void OnAttack()
    {
        RecalculateAttackArea();
    }

    void RecalculateAttackArea()
    {
        Vector2 fDir = Vector2.zero;
        if (!target)
            fDir = controller.collisions.faceDir * Vector2.right;
        else 
            fDir = -Mathf.Sign(transform.position.x - target.position.x) * Vector2.right;

        Vector2 fDirNormalLeft = Vector2.Perpendicular(fDir);
        Vector2 fDirNormalRight = -fDirNormalLeft;
        Vector2 bottomRight = (Vector2)transform.position + fDir * controller.ctrlCollider.size / 2 + Vector2.up * range / 2;
        Vector2 topLeft = bottomRight + fDir * range - Vector2.up * range;
        attackArea.size = new Vector2(
            Mathf.Abs(topLeft.x - bottomRight.x),
            Mathf.Abs(topLeft.y - bottomRight.y)
        );

        switch (attackDirection)
        {
            case AttackDirection.Static:
                attackArea.leadingPoing = topLeft;
                attackArea.trailingPoint = bottomRight;
                break;
            case AttackDirection.LeftToRight:
                attackArea.leadingPoing = topLeft - fDir * range + fDirNormalRight * range * (1 - timeToAttackComplete / attackDuration);
                attackArea.trailingPoint = topLeft;

                if (fDir.x != 0)
                {
                    attackArea.size.y *= (1 - timeToAttackComplete / attackDuration);
                }
                else if (fDir.y != 0)
                {
                    attackArea.size.x *= (1 - timeToAttackComplete / attackDuration);
                }

                break;
            case AttackDirection.RightToLeft:
                attackArea.leadingPoing = bottomRight + fDirNormalLeft * range * (1 - timeToAttackComplete / attackDuration);
                attackArea.trailingPoint = bottomRight + fDir * range;

                if (fDir.x != 0)
                {
                    attackArea.size.y *= (1 - timeToAttackComplete / attackDuration);
                }
                else if (fDir.y != 0)
                {
                    attackArea.size.x *= (1 - timeToAttackComplete / attackDuration);
                }

                break;
            case AttackDirection.Outward:
                attackArea.leadingPoing = topLeft - fDir * range + fDir * range * (1 - timeToAttackComplete / attackDuration);
                attackArea.trailingPoint = bottomRight;

                if (fDir.x != 0)
                {
                    attackArea.size.x *= (1 - timeToAttackComplete / attackDuration);
                }
                else if (fDir.y != 0)
                {
                    attackArea.size.y *= (1 - timeToAttackComplete / attackDuration);
                }
                break;
        }



    }

    void OnDrawGizmos()
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

    public enum AttackDirection
    {
        Static,
        LeftToRight,
        RightToLeft,
        Outward
    }

    struct AttackArea
    {
        public Vector2 leadingPoing;
        public Vector2 trailingPoint;
        public Vector2 size;
    }
}
