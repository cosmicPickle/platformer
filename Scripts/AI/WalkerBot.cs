using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WalkerBot : Bot
{
    [Header("Ability Settings")]
    public bool canJump = true;
    public bool canWallJump = true;
    public bool canDoPerfectSlideJumps = false;
    public bool canFallThroughPlatform = true;

    [Header("General Movement Settings")]
    public Stat maxJumpHeight = 4;
    public Stat minJumpHeight = 1;
    public Stat timeToJumpApex = .4f;
    public Stat moveSpeed = 6;

    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;

    const float targetPositionMarginX = 0.25f;
    const float targetPositionMarginY = 0.25f;
    Vector2 targetPosition;

    [Header("Wall Movement Settings")]
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTme = .25f;

    float timeToWallUnstick;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    protected Vector2 velocity;
    float velocityXSmoothing;

    protected Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    float jumpLookAheadRays = 3;
    float jumpLookAheadDistance = 2;
    
    protected override void Idle()
    {
        base.Idle();
        directionalInput.x = 0;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.green;
        float size = .3f;

        if (targetPosition != Vector2.zero)
        {
            Gizmos.DrawLine(targetPosition - Vector2.up * size, targetPosition + Vector2.up * size);
            Gizmos.DrawLine(targetPosition - Vector2.left * size, targetPosition + Vector2.left * size);
        }
    }

    protected override void StartAttack()
    {
        base.StartAttack();
        targetPosition = detectionInfo.selectedEnemy.transform.position;
        directionalInput.x = targetPosition.x < transform.position.x ? -1 : 1;

        targetPosition += -directionalInput * Vector2.right * attackAgent.range;
        directionalInput.x = targetPosition.x < transform.position.x ? -1 : 1;

        TraverseTerrain(targetPosition);
    }

    protected override void Patrol()
    {
        base.Patrol();
        if (directionalInput.x == 0)
        {
            directionalInput.x = 1;
        }

        if (initialPosition.x - patrolRadius >= transform.position.x)
        {
            directionalInput.x = 1;
        }

        if(initialPosition.x + patrolRadius <= transform.position.x)
        {
            directionalInput.x = -1;
        }

        if(directionalInput.x == 1)
        {
            TraverseTerrain(new Vector2(initialPosition.x + patrolRadius, initialPosition.y));
        } else
        {
            TraverseTerrain(new Vector2(initialPosition.x - patrolRadius, initialPosition.y));
        }

        
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    protected override void Update()
    {
        base.Update();

        if (freezeController.getFreeze())
        {
            return;
        }

        CalculateVelocity();
        HandleWallSliding();

       
        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }

    void JumpStart()
    {
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpClimb.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }

        if (controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                {
                    //not jumping against max slope
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
    }

    void JumpStop()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTme;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTme;
            }
        }
    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;

        if (currentKnockback.duration > 0)
        {
            if (currentKnockback.direction.x == 0)
            {
                currentKnockback.direction.x = -controller.collisions.faceDir;
            }

            float dirX = Mathf.Sign(currentKnockback.direction.x);
            float angle = -dirX * Hitbox.Knockback.Angle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
            Vector2 baseDirection = dirX * Vector2.right * currentKnockback.speed;

            Vector2 targetVelocity = new Vector2(baseDirection.x * cos + baseDirection.y * sin, -baseDirection.x * sin + baseDirection.y * cos);
            targetVelocityX = targetVelocity.x;
            velocity.y = targetVelocity.y;

            currentKnockback.duration -= Time.deltaTime;
        }
        else
        {
            currentKnockback.duration = 0;
        }

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    void TraverseTerrain(Vector2 target)
    {
        if(
            target.x - targetPositionMarginX < transform.position.x && 
            target.x + targetPositionMarginX > transform.position.x &&
            target.y - targetPositionMarginY < transform.position.y &&
            target.y + targetPositionMarginY > transform.position.y
        )
        {
            velocity.x = 0;
            //We have arrived
            return;
        }

        directionalInput.y = 0;

        if (target.y > transform.position.y + targetPositionMarginY)
        {
            //The target is higher than the bot
            if (canJump)
            {
                //But we cannot jump so whatever
                for (int i = 0; i < jumpLookAheadRays; i++)
                {
                    float rayLength = maxJumpHeight;
                    float originModifier = i * (jumpLookAheadDistance / jumpLookAheadRays);
                    Vector2 rayOrigin = (directionalInput.x == -1) ? controller.raycastOrigins.topLeft : controller.raycastOrigins.topRight;
                    rayOrigin += Vector2.right * directionalInput.x * originModifier;

                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, controller.collisionMask);

                    Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.red);

                    if (hit)
                    {
                        if (hit.collider.tag == "Through")
                        {
                            JumpStart();
                            break;
                        }
                    }
                }
            }
        }

        if(target.y > transform.position.y + targetPositionMarginY || (!canJump && target.y >= transform.position.y))
        {
            //We want to constrain the bot to the current platform and prevent it from falling if:
            //1. The target is higher than a set margin
            //2. The target is higher or on the same level and the bot can't jump

            if (controller.collisions.below)
            {
                Vector2 rayOriginDown = (directionalInput.x == -1) ? controller.raycastOrigins.bottomLeft : controller.raycastOrigins.bottomRight;
                RaycastHit2D hitDown = Physics2D.Raycast(rayOriginDown, Vector2.up * -1, RaycastController.skinWidth * 2, controller.collisionMask);

                Debug.DrawRay(rayOriginDown, Vector2.up * -1, Color.red);

                if (!hitDown)
                {
                    velocity.x = 0;
                    directionalInput.x = -directionalInput.x;
                }
            }
        }

        if(canFallThroughPlatform && target.y < transform.position.y - targetPositionMarginY && controller.collisions.below)
        {
            //If the target is lower and the bot can fall through platforms
            directionalInput.y = -1;
        }

        if (canWallJump)
        {
            if ((controller.collisions.left && target.x < transform.position.x) || (controller.collisions.right && target.x > transform.position.x))
            {
                //We have a wall
                if (canDoPerfectSlideJumps)
                {
                    //If we allow the bot to cheat with perfect timing on slopes higher than max slope
                    //We don't care about anything else
                    JumpStart();
                }
                else
                {
                    //Otherwise we need to check whether that is a slope or a wall
                    Vector2 rayOriginCheckSlope = (controller.collisions.left) ? controller.raycastOrigins.bottomLeft : controller.raycastOrigins.bottomRight;
                    int direction = (controller.collisions.left) ? -1 : 1;
                    RaycastHit2D hitCheckSlope = Physics2D.Raycast(rayOriginCheckSlope, Vector2.right * direction, RaycastController.skinWidth * 2, controller.collisionMask);

                    if (hitCheckSlope)
                    {
                        float slopeAngle = Mathf.RoundToInt(Vector2.Angle(hitCheckSlope.normal, Vector2.up));
                        if (slopeAngle <= controller.maxSlopeAngle || slopeAngle == 90)
                        {
                            JumpStart();
                        }
                        else
                        {
                            velocity.x = 0;
                            directionalInput.x = -directionalInput.x;
                        }
                    }
                }
            }
        }
    }
}
