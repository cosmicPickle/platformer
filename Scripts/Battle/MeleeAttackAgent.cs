using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackAgent : AttackAgent
{
    [Header("MeleeAttackAgent Settings")]
    public bool autoTarget = false;

    public float attackWidth = 1f;
    public AttackDirection attackDirection;
    AttackType attackType = new AttackType();


    public void Attack(AttackType type)
    {
        attackType = type;
        base.Attack();
    }

    public void Attack(Transform target, AttackType type)
    {
        attackType = type;
        base.Attack(target);
    }


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

        if (attackType == AttackType.Default)
        {
            if (!target || !autoTarget)
                fDir = controller.collisions.faceDir * Vector2.right;
            else
                fDir = -Mathf.Sign(transform.position.x - target.position.x) * Vector2.right;
        } else 
        {
            fDir = (attackType == AttackType.Above) ? Vector2.up : -Vector2.up;
        }

        Vector2 fDirNormal = Vector2.Perpendicular(fDir);
        Vector2 directionModifier = fDirNormal * fDir.x;
        if (directionModifier == Vector2.zero)
            directionModifier = fDirNormal * fDir.y;


        Vector2 localScale = new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        Vector2 absFDir = new Vector2(Mathf.Abs(fDir.x), Mathf.Abs(fDir.y));

        Vector2 bottomRight = (Vector2)transform.position 
            + fDir * controller.ctrlCollider.size / 2 * localScale 
            + controller.ctrlCollider.offset * localScale 
            + directionModifier * attackWidth / 2;

        Vector2 topLeft = bottomRight + fDir * range - directionModifier * attackWidth;
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
            case AttackDirection.BottomToTop:
                attackArea.leadingPoing = topLeft - fDir * range + directionModifier * attackWidth * (1 - timeToAttackComplete / attackDuration);
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
            case AttackDirection.TopToBottom:
                attackArea.leadingPoing = bottomRight - directionModifier * attackWidth * (1 - timeToAttackComplete / attackDuration);
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
                attackArea.leadingPoing = topLeft - fDir * attackWidth + fDir * range * (1 - timeToAttackComplete / attackDuration);
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

        if(onRecalculateAttackArea != null)
        {
            onRecalculateAttackArea(attackArea);
        }
    }

    public enum AttackDirection
    {
        Static,
        BottomToTop,
        TopToBottom,
        Outward
    }

    public enum AttackType
    {
        Default,
        Above,
        Below
    }
}
