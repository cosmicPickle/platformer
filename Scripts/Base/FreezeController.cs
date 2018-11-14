using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeController : MonoBehaviour
{
    public Player playerObject;
    public float unfreezeDistance = 20f;
    protected bool freeze = false;

    public bool getFreeze()
    {
        return freeze;
    }
    // Update is called once per frame
    void Update()
    {
        if (!playerObject)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerObject.transform.position);
        if(distanceToPlayer >= unfreezeDistance)
        {
            freeze = true;
        } else
        {
            freeze = false;
        }

    }
}
