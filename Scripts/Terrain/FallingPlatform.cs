using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : RaycastController
{
    public float gravity = 10;
    public float stableTime = .5f;
    public float dissapearDistance = 10;

    public float shakeAmount = 0.2f;

    bool triggerFall = false;
    float timeLeftToFall = 0;
    float currentFallDistance;
    Vector2 velocity;

    public bool enableReset = true;
    public float resetTime = 3f;
    bool triggerReset;
    float timeLeftToReset = 0;
    Vector3 initialPosition;
    Vector3 initialScale;

    public override void Start()
    {
        base.Start();

        initialPosition = transform.position;
        initialScale = transform.localScale;
        Reset(); 
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigins();
        if(CheckForCharacters())
        {
            triggerFall = true;
        } 

        if(triggerFall)
        {
            velocity.x = Random.insideUnitCircle.x * (Time.deltaTime * shakeAmount);
            if (timeLeftToFall <= 0)
            {
                float deltaDistance = gravity * Time.deltaTime;
                velocity.y += -deltaDistance;
                currentFallDistance += deltaDistance;

                if (currentFallDistance > dissapearDistance)
                {
                    Reset();
                    transform.localScale = Vector3.zero;

                    if (enableReset)
                    {
                        triggerReset = true;
                    }
                }
            } else
            {
                timeLeftToFall -= Time.deltaTime;
            }

            transform.Translate(velocity);
        }

        if (triggerReset)
        {
            if (timeLeftToReset <= 0)
            {
                Reset();
                gameObject.SetActive(true);
                
            }
            else
            {
                timeLeftToReset -= Time.deltaTime;
            }
        }
    }

    void Reset()
    {
        print("Called Reset");
        triggerFall = false;
        triggerReset = false;
        timeLeftToFall = stableTime;
        timeLeftToReset = resetTime;
        currentFallDistance = 0;


        velocity.x = 0;
        velocity.y = 0;
        transform.position = initialPosition;
        transform.localScale = initialScale;
    }


    bool CheckForCharacters()
    {
        if (freezeController.getFreeze())
        {
            return false;
        }

        if (triggerReset)
            return false;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, skinWidth * 10, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * skinWidth * 10, Color.red);

            if(hit)
            {
                return true;
            }
        }

        return false;
    }
}
