using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class AttackAgent : MonoBehaviour
{
    public float damage;
    public Vector2 knockbackEffect;
    public float knockbackDuration = .2f;
    public float attackRange;

    protected Controller2D controller;

    protected virtual void Start()
    {
        controller = GetComponent<Controller2D>();
    }

    public virtual void Attack(LayerMask collisionMask)
    {

    }

    public virtual void Attack(Collider2D target, LayerMask collisionMask)
    {

    }
}
