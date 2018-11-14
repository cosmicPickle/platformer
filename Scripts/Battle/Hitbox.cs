using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float maxHealthPoints = 100;
    public float invulnerableDurationOnDamage = 2f;

    float currentHealthPoints;

    bool invulnerable;
    float invulnerableTimeLeft = 0;

    bool forcedInvulnerable;

    bool frozen = false;

    public delegate void OnDamageTaken(float amount, Knockback knockback);
    public OnDamageTaken onDamageTaken;

    Knockback currentKnockback;

    // Start is called before the first frame update
    void Start()
    {
        currentHealthPoints = maxHealthPoints;
    }

    bool GetFrozen()
    {
        return frozen;
    }

    // Update is called once per frame
    void Update()
    {
        if (invulnerableTimeLeft <= 0)
        {
            invulnerable = false;
        } else
        {
            invulnerableTimeLeft -= Time.deltaTime;
        }
    }

    public void Damage(AttackAgent attackAgent)
    {
        if (invulnerable || forcedInvulnerable)
        {
            return;
        }

        DoDamage(attackAgent.attack);

        currentKnockback = new Knockback();
        if (attackAgent.knockback > 0)
        {

            currentKnockback.direction = (transform.position - attackAgent.transform.position).normalized;
            currentKnockback.duration = attackAgent.knockback;
            currentKnockback.speed = attackAgent.knockbackSpeed;
        }

        if (currentKnockback.duration > 0)
        {
            frozen = true;
        }

        if (onDamageTaken != null)
        {
            onDamageTaken(attackAgent.attack, currentKnockback);
        }
        
    }

    public void Damage(float amount, Knockback knockback)
    {
        if (invulnerable || forcedInvulnerable)
        {
            return;
        }

        DoDamage(amount);

        currentKnockback = knockback;

        if(knockback.duration > 0)
        {
            frozen = true;
        }
        onDamageTaken(amount, knockback);
    }

    protected void DoDamage(float amount)
    {
        invulnerable = true;
        invulnerableTimeLeft = invulnerableDurationOnDamage;

        currentHealthPoints -= amount;

        if (currentHealthPoints <= 0)
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

    public void Invulnerable(bool status)
    {
        forcedInvulnerable = status;
    }

    public struct Knockback
    {
        public const float Angle = 10 * Mathf.Deg2Rad;
        public Vector2 direction;
        public float duration;
        public float speed;
    }
}
