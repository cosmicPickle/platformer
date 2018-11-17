using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public float radius = 5f;

    bool isFocused = false;
    bool hasInteracted = false;
    Transform player;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    protected virtual void Interact()
    {
        Debug.Log("Interacting with " + transform.name);
    }

    protected virtual void Update()
    {
        if(isFocused)
        {
            float distance = Vector2.Distance(player.position, transform.position);

            if(distance <= radius && !hasInteracted)
            {
                hasInteracted = true;
                Interact();
            } 

            if(distance > radius)
            {
                onDefocused();
            }
        }

        
    }

    public void OnFocused(Transform playerTransform)
    {
        isFocused = true;
        player = playerTransform;
        hasInteracted = false;
    }

    public void onDefocused()
    {
        isFocused = false;
        player = null;
        hasInteracted = false;
    }

}
