using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(FreezeController))]
public class Hazard : MonoBehaviour
{
    public LayerMask collisionMask;
    public float damage;
    public Vector2 knockbackEffect;
    public float knockbackDuration = .2f;
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
                print("Ouch");
                hitbox.Damage(damage, knockbackEffect, 0, knockbackDuration);
            }
        }
    }
}
