using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterAttackAgent : AttackAgent
{
    public enum TrackType { None, Directional, Track}

    [Header("ShooterAttackAgent Settings")]
    public TrackType trackType;
    public float trackDuration = 0.8f;

    public GameObject bullet;
    public float bulletSpeed = 2;

    protected override void OnAttack()
    {
        GameObject bulletInstance = (GameObject)Instantiate(bullet, transform.position, transform.rotation);

        bulletInstance.GetComponent<Bullet>().Shoot(this);    
    }
}
