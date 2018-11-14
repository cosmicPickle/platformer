using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BotLocator))]
[RequireComponent(typeof(IBotNavigator))]
[RequireComponent(typeof(Collider2D))]
public class BotAttacker : MonoBehaviour {

    public AttackAgentSequenceType attackSequenceType;
    public float range
    {
        get
        {
            return agent != null ? agent.range : 0;
        }
    }

    public Stat attackOnTouch = 5;
    public Stat knockbackOnTouch = 0.05f;
    public Stat knockbackSpeedOnTouch = 30;

    protected BotLocator locator;
    protected Collider2D ctrlCollider;
    protected ContactFilter2D contact;
    protected IBotNavigator navigator;

    protected AttackAgent agent;

    public enum AttackAgentSequenceType
    {
        Random,
        Rotation
    }

    public bool isAttacking { get; protected set; }

    protected virtual void Awake()
    {
        locator = GetComponent<BotLocator>();
        navigator = GetComponent<IBotNavigator>();
        ctrlCollider = GetComponent<Collider2D>();

        contact = new ContactFilter2D();
        contact.layerMask = locator.enemyMask;

        agent = GetComponent<AttackAgent>();
        agent.onAttackStart = OnAttackStart;
        agent.onAttackComplete = OnAttackComplete;

        if (agent != null)
            agent.Init(locator.enemyMask, locator.obstacleMask);
    }

    protected virtual void Update()
    {
        if(agent != null && isAttacking && !navigator.GetKnockbackStatus() && locator.LocateEnemy() != null)
        {
            agent.Attack(locator.LocateEnemy().transform);
        }

        HandleAttackOnTouch();
    }

    public virtual bool EnemyInAttackRange()
    {
        return locator.GetDistanceToTarget() <= range;
    }

    public virtual void Attack()
    {
        isAttacking = true;
    }

    public virtual void Stop()
    {
        isAttacking = false;
    }

    protected virtual void HandleAttackOnTouch()
    {
        if (attackOnTouch <= 0)
            return;

        if (navigator.GetKnockbackStatus())
        {
            return;
        }

        Collider2D[] hits = new Collider2D[5];
        List<Collider2D> processedHits = new List<Collider2D>();
        ctrlCollider.OverlapCollider(contact, hits);

        for(int i = 0; i < hits.Length; i++)
        {
            if (processedHits.Contains(hits[i]) || hits[i] == null)
                continue;

            if (agent != null && agent.GetHitTargets() != null && agent.GetHitTargets().Contains(hits[i]))
                continue;

            processedHits.Add(hits[i]);

            Hitbox hitbox = hits[i].GetComponent<Hitbox>();

            if (hitbox == null)
                continue;

            hitbox.Damage(attackOnTouch, new Hitbox.Knockback
            {
                direction = hitbox.transform.position - transform.position,
                duration = knockbackOnTouch,
                speed = knockbackSpeedOnTouch
            });
        }
    }

    protected void OnAttackStart()
    {
        navigator.Pause();
    }

    protected void OnAttackComplete()
    {
        navigator.Resume();
    }
   
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
