using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squasher : RaycastController
{
    [Range(-1, 1)]
    public int directionX;
    [Range(-1, 1)]
    public int directionY;

    public float squashRange = 5f;
    public float squashSpeed = 10f;
    public float retractSpeed = 2f;

    public float damageRangeAroundTarget = 0.5f;
    public Stat damage;
    public bool instakill = true;

    bool retracting;
    Vector2 moveAmount;
    Vector3 initialPosition;


    public override void Start()
    {
        base.Start();
        initialPosition = transform.position;
    }
    void OnDrawGizmos()
    {

        if(!Application.isPlaying)
        {
            ctrlCollider = GetComponent<BoxCollider2D>();
        } else
        {
            if (freezeController.getFreeze())
            {
                return;
            }
        }

        Vector3 sizeOffset = new Vector3(
            ctrlCollider.bounds.size.x / 2 * directionX,
            ctrlCollider.bounds.size.y / 2 * directionY
        );

        Vector3 start = ((Application.isPlaying) ? initialPosition : transform.position) + sizeOffset;
        Vector3 end = start + new Vector3(directionX, directionY) * squashRange;

        Debug.DrawLine(start, end, Color.red);
    }

    // Update is called once per frame
    void Update()
    {
        if (freezeController.getFreeze())
        {
            return;
        }

        Vector3 heading = transform.position - initialPosition;
        UpdateRaycastOrigins();
        if (!retracting)
        {
            moveAmount.x = directionX * squashSpeed * Time.deltaTime;
            moveAmount.y = directionY * squashSpeed * Time.deltaTime;

            if(heading.magnitude >= squashRange)
            {
                retracting = true;
            }
        } else
        {
            moveAmount.x = -directionX * retractSpeed * Time.deltaTime;
            moveAmount.y = -directionY * retractSpeed * Time.deltaTime;


            Vector3 retractingHeading = initialPosition - initialPosition + new Vector3(directionX, directionY);

            if (heading.normalized != retractingHeading.normalized) 
            {
               retracting = false;
            }
        }

        

        if (moveAmount.y != 0)
        {
            float dirY = Mathf.Sign(moveAmount.y);
            float rayLength = moveAmount.y;
            
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (dirY > 0 ? raycastOrigins.topLeft : raycastOrigins.bottomLeft) + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, dirY * Vector2.up, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, dirY * Vector2.up * rayLength, Color.red);
                if (hit)
                {

                    Controller2D hitCtrl = hit.collider.GetComponent<Controller2D>();
                    if (hitCtrl)
                    {
                        hit.collider.GetComponent<Controller2D>().Move(moveAmount, false);
                    }

                    if (squashRange - heading.magnitude <= hit.collider.bounds.size.y + damageRangeAroundTarget)
                    {
                        //Actually hit at the peak of the squash distance
                        Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
                        if (hitbox)
                        {
                            if (instakill)
                            {
                                hitbox.Die();
                            }
                            else
                            {
                                hitbox.Damage(damage.GetValue(), new Hitbox.Knockback());
                            }
                        }

                        retracting = !retracting;
                        moveAmount.y = moveAmount.y - dirY * (hit.distance - skinWidth);
                    }

                    break;
                }
            }
        }

        if(moveAmount.x != 0)
        {
           
            float dirX = Mathf.Sign(moveAmount.x);
            float rayLength = moveAmount.x;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (dirX < 0 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, dirX * Vector3.right, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, dirX * Vector3.right * rayLength, Color.red);

                if(hit)
                {
                    
                    Controller2D hitCtrl = hit.collider.GetComponent<Controller2D>();
                    if (hitCtrl)
                    {
                        hit.collider.GetComponent<Controller2D>().Move(moveAmount, false);
                    }

                    if(squashRange - heading.magnitude <= hit.collider.bounds.size.x + damageRangeAroundTarget)
                    {
                        //Actually hit at the peak of the squash distance
                        Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
                        if(hitbox)
                        {
                            if(instakill)
                            {
                                hitbox.Die();
                            } else
                            {
                                hitbox.Damage(damage.GetValue(), new Hitbox.Knockback());
                            } 
                        }

                        retracting = !retracting;
                        moveAmount.x = moveAmount.x - dirX * (hit.distance - skinWidth);
                    }
                    break;
                }
            }
        }

        transform.Translate(moveAmount);
    }
}
