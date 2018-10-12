using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D))]
public class Hitbox : MonoBehaviour
{
    public float maxHealthPoints = 100;
    float currentHealthPoints;

    float knockbackTimeLeft = 0;
    Vector2 knockbackVelocity;

    Controller2D controller;

    [HideInInspector]
    protected bool invulnerable;
    public float invulnerableDurationOnDamage = 2f;
    float invulnerableTimeLeft = 0;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();
        currentHealthPoints = maxHealthPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if(invulnerableTimeLeft <= 0)
        {
            invulnerable = false;
        } else
        {
            invulnerableTimeLeft -= Time.deltaTime;
        }

        if (knockbackTimeLeft <= 0)
            return;

        controller.Move(knockbackVelocity * Time.deltaTime, false);
        knockbackTimeLeft -= Time.deltaTime;
    }

    public void Damage(float amount, Vector2 knockbackEffect, float hitXDirection, float knockbackDuration)
    {
        if(invulnerable)
        {
            return;
        }

        currentHealthPoints -= amount;
        print(currentHealthPoints);
        if(knockbackEffect != Vector2.zero)
        {
            knockbackVelocity = knockbackEffect;
            knockbackVelocity.x *= hitXDirection;
            knockbackTimeLeft = knockbackDuration;

            invulnerable = true;
            invulnerableTimeLeft = invulnerableDurationOnDamage;
        }
        
        if(currentHealthPoints <= 0)
        {
            currentHealthPoints = 0;
            Destroy(gameObject);
        }
    }

    public void Die()
    {
        currentHealthPoints = 0;
        Destroy(gameObject);
    }

    public bool GetKnockbackStatus()
    {
        return knockbackTimeLeft > 0;
    }

    public void Invulnerable(bool status)
    {
        invulnerable = status;
    }
}
