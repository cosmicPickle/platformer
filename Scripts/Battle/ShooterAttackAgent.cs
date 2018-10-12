using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterAttackAgent : AttackAgent
{
    public bool trackingBullet = true;
    public bool targetLookahead = true;
    public GameObject bullet;
    public float bulletSpeed = 2;

    public override void Attack(LayerMask collisionMask)
    {
        GameObject bulletInstance = (GameObject)Instantiate(bullet, transform.position, transform.rotation);

        Vector2 direction = Vector2.right * controller.collisions.faceDir;

        bulletInstance.GetComponent<Bullet>().damage = damage;
        bulletInstance.GetComponent<Bullet>().knockbackEffect = knockbackEffect;
        bulletInstance.GetComponent<Bullet>().knockbackDuration = knockbackDuration;
        bulletInstance.GetComponent<Bullet>().Shoot(direction, bulletSpeed, attackRange, collisionMask);
    }

    public override void Attack(Collider2D target, LayerMask collisionMask)
    {
        base.Attack(target, collisionMask);

        GameObject bulletInstance = (GameObject)Instantiate(bullet, transform.position, transform.rotation);

        Vector2 direction;
        if (trackingBullet)
        {
            Vector2 targetPos;
            Controller2D ctrl = target.GetComponent<Controller2D>();
            if (targetLookahead && ctrl)
            {
                targetPos = new Vector2(
                    target.transform.position.x + ctrl.cachedVelocity.x * Time.deltaTime,
                    target.transform.position.y + ctrl.cachedVelocity.y * Time.deltaTime
                );
            }
            else
            {
                targetPos = new Vector2(
                    target.transform.position.x,
                    target.transform.position.y
                );
            }

            direction = new Vector2(
                targetPos.x - transform.position.x,
                targetPos.y - transform.position.y
            ).normalized;
        } else
        {
            if (transform.position.x < target.transform.position.x)
                controller.collisions.faceDir = 1;
            else
                controller.collisions.faceDir = -1;

            direction = Vector2.right * controller.collisions.faceDir;
        }

        bulletInstance.GetComponent<Bullet>().damage = damage;
        bulletInstance.GetComponent<Bullet>().knockbackEffect = knockbackEffect;
        bulletInstance.GetComponent<Bullet>().knockbackDuration = knockbackDuration;
        bulletInstance.GetComponent<Bullet>().Shoot(direction, bulletSpeed, attackRange, collisionMask);
    }
}
