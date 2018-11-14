using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedAttackAgent : AttackAgent {

    [Header("ExtendedAttackAgent Settings")]
    public bool autoTarget = false;
    public float attackWidth = .5f;

    protected override void Update()
    {
        base.Update();

        if (isAttacking)
        {
            RecalculateAttackArea();
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
    }

    protected override void OnAttack()
    {
        RecalculateAttackArea();
    }

    protected override void RecalculateAttackArea()
    {
        Vector2 fDir = Vector2.zero;

        if (!target || !autoTarget)
            fDir = controller.collisions.faceDir * Vector2.right;
        else
            fDir = -Mathf.Sign(transform.position.x - target.position.x) * Vector2.right;

        Vector2 fDirNormal = Vector2.Perpendicular(fDir);
        Vector2 directionModifier = fDirNormal * fDir.x;

        Vector2 bottomRight = (Vector2)transform.position + fDir * controller.ctrlCollider.size / 2 + directionModifier * attackWidth / 2;
        Vector2 topLeft = bottomRight + fDir * range - directionModifier * attackWidth;
        attackArea.size = new Vector2(
            Mathf.Abs(topLeft.x - bottomRight.x),
            Mathf.Abs(topLeft.y - bottomRight.y)
        );

        float rangeModifier = 2 * range * (1 - timeToAttackComplete / attackDuration);

        if (timeToAttackComplete > attackDuration / 2)
        {
            attackArea.leadingPoing = topLeft - fDir * range + fDir * rangeModifier;
        } else
        {
            attackArea.leadingPoing = topLeft + fDir * range - fDir * rangeModifier;
        }
        attackArea.trailingPoint = bottomRight;

        attackArea.size.x *= (1 - timeToAttackComplete / attackDuration);
    }
}
