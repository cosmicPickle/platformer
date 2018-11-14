using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttackAgent : AttackAgent {

    public float chargeSpeed = 10;
    public Vector2 allowedChargeDirection = Vector2.right;

    private Vector2 chargeVelocity;
    private Vector2 navDirection;
    private ContactFilter2D contact;

    public override void Init(LayerMask enemies, LayerMask obstacles)
    {
        base.Init(enemies, obstacles);
        contact = new ContactFilter2D();
        contact.layerMask = enemies;
    }

    protected override void Update()
    {
        base.Update();

        if(isAttacking)
        {
            controller.Move(chargeVelocity * Time.deltaTime, navDirection);

            Collider2D[] hits = new Collider2D[5];

            controller.ctrlCollider.OverlapCollider(contact, hits);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hitTargets.Contains(hits[i]) || hits[i] == null)
                    continue;

                hitTargets.Add(hits[i]);

                Hitbox hitbox = hits[i].GetComponent<Hitbox>();

                if (hitbox == null)
                    continue;

                hitbox.Damage(attack, new Hitbox.Knockback
                {
                    direction = hitbox.transform.position - transform.position,
                    duration = knockback,
                    speed = knockbackSpeed
                });

                chargeVelocity = Vector2.zero;
            }
        }
    }

    protected override void OnAttack()
    {
        base.OnAttack();
        CalculateChargeVelocity();
    }

    private void CalculateChargeVelocity()
    {
        Vector2 direction = (target.transform.position - transform.position);
        navDirection = new Vector2(Mathf.Sign(direction.x), 0);
        chargeVelocity = direction.normalized * chargeSpeed * allowedChargeDirection;    
    }
}
