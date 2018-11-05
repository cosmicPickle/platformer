using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pendulum : MonoBehaviour {

    public Stat damage = 10;
    public Stat knockbackDuration = 0.05f;
    public Stat knockbackSpeed = 50f;

    [Range(-1, 1)]
    public int startDirection = 1;

    public bool instakill = false;

    public float swingAngle = 90;
    public float swingVelocity = 100;
    public LayerMask hitMask;

    Collider2D head;
    Rigidbody2D rb2d;
    

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.angularVelocity = Mathf.Sign(startDirection) * swingVelocity;
        head = GetComponentInChildren<Collider2D>();

        if(head == null)
        {
            Debug.LogWarning("Pendulum does not have head");
        }
    }

    private void Update()
    {
        Push();
        DetectHit();
    }

    void Push()
    {
        if (transform.rotation.z > 0
           && transform.eulerAngles.z < swingAngle / 2
           && rb2d.angularVelocity > 0
           && rb2d.angularVelocity < swingVelocity)
        {
            rb2d.angularVelocity = swingVelocity;
        }

        if (transform.rotation.z < 0
           && 365 - transform.eulerAngles.z < swingAngle / 2
           && rb2d.angularVelocity < 0
           && rb2d.angularVelocity > -swingVelocity)
        {
            rb2d.angularVelocity = -swingVelocity;
        }
    }

    void DetectHit()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = hitMask;

        Collider2D[] hits = new Collider2D[5];
        head.OverlapCollider(filter, hits);
        List<Hitbox> hitTargets = new List<Hitbox>(hits.Length);

        for (int i = 0; i < hits.Length; i++)
        {
            if(hits[i] == null)
            {
                continue;
            }

            Hitbox hitbox = hits[i].GetComponent<Hitbox>();

            if (!hitbox)
            {
                continue;
            }

            if(hitTargets.Contains(hitbox))
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
                    direction = rb2d.angularVelocity > 0 ? Vector2.right : - Vector2.right,
                    speed = knockbackSpeed.GetValue()
                });
            }
        }
    }
}
