using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(FreezeController))]
public class Hazard : MonoBehaviour
{
    public LayerMask collisionMask;

    public Stat damage = 10;
    public Stat knockbackDuration = 0.05f;
    public Stat knockbackSpeed = 20f;

    public bool instakill = false;

    BoxCollider2D ctrlCollider;
    FreezeController freezeController;

    void Start()
    {
        ctrlCollider = GetComponent<BoxCollider2D>();
        freezeController = GetComponent<FreezeController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (freezeController.getFreeze())
        {
            return;
        }

        Collider2D[] results = new Collider2D[5];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(collisionMask);
        ctrlCollider.OverlapCollider(filter, results);

        for(int i = 0; i< results.Length; i++)
        {
            Collider2D hit = results[i];
            if(!hit)
            {
                continue;
            }

            Hitbox hitbox = hit.GetComponent<Hitbox>();
            if (!hitbox)
            {
                continue;
            }

            if (instakill)
            {
                hitbox.Die();
            }
            else
            {
                Controller2D ctrl = hitbox.GetComponent<Controller2D>();
                Vector2 direction = ctrl == null ? Vector2.right : -ctrl.collisions.faceDir * Vector2.right;

                if(ctrl.transform.position.y < transform.position.y)
                {
                    direction.y = -1;
                }

                hitbox.Damage(damage.GetValue(), new Hitbox.Knockback
                {
                    duration = knockbackDuration.GetValue(),
                    direction = direction,
                    speed = knockbackSpeed.GetValue()
                });
            }
        }
    }
}
