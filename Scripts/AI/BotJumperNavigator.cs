using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotJumperNavigator : BotNavigator {

    public Stat maxJumpHeight = 4;
    public Stat minJumpHeight = 1;
    public Stat timeToJumpApex = .4f;

    protected float maxJumpVelocity;
    protected float minJumpVelocity;

    protected bool stopOnGrounded;

    protected override void Awake()
    {
        base.Awake();
        RecalculateGravity();
    }

    protected override void Update()
    {

        base.Update();
        RecalculateGravity();

        if (GetGroundCollision() && stopOnGrounded)
        {
            stopped = true;
            stopOnGrounded = false;
        }

        Jump();
    }

    public override void Resume()
    {
        base.Resume();

        stopOnGrounded = false;
    }

    public override void Pause()
    {
        base.Pause();

        stopped = false;
        stopOnGrounded = true;
    }

    void RecalculateGravity()
    {
        gravity = (2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    bool GetGroundCollision()
    {
        return gravityDirection.y != 0
            ? gravityDirection.y < 0
                ? controller.collisions.below
                : controller.collisions.above
            : gravityDirection.x < 0
                ? controller.collisions.left
                : controller.collisions.right;
    }

    void Jump()
    {
        if (GetGroundCollision() && !stopped)
        {
            if(gravityDirection.y != 0)
                velocity.y = gravityDirection.y < 0 ? maxJumpVelocity : -maxJumpVelocity;
            else
                velocity.x = gravityDirection.x < 0 ? maxJumpVelocity : -maxJumpVelocity;
        }
    }

    protected override void HandleDestinationArrival(ref Vector2 moveAmount) { }


    protected override void ResetPatrolTarget()
    {
        if (gravityDirection == Vector2.zero)
        {
            Debug.LogWarning("Gravity not setup corectly");
            return;
        }

        if(!GetGroundCollision())
        {
            return;
        }

        if (gravityDirection.y != 0 && Mathf.Sign(transform.position.x - currentTarget.x) == Mathf.Sign(navDirection))
        {
            currentPatrolTarget++;

            if (currentPatrolTarget >= patrolTargets.Length)
            {
                currentPatrolTarget = 0;
            }

            currentTarget = globalPatrolTargets[currentPatrolTarget];
            stopped = false;
        }
        else if (gravityDirection.x != 0 && Mathf.Sign(transform.position.y - currentTarget.y) == Mathf.Sign(navDirection))
        {
            currentPatrolTarget++;

            if (currentPatrolTarget >= patrolTargets.Length)
            {
                currentPatrolTarget = 0;
            }

            currentTarget = globalPatrolTargets[currentPatrolTarget];
            stopped = false;
        }
    }

    protected override void ChangeNavDirection()
    {
        if (!GetGroundCollision())
        {
            return;
        }

        base.ChangeNavDirection();
    }
}
