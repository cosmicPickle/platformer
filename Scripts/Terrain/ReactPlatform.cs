using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactPlatform : RaycastController
{
    protected bool CheckForCharacters()
    {
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, skinWidth * 10, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * skinWidth * 10, Color.red);

            if (hit)
            {
                return true;
            }
        }

        return false;
    }
}
