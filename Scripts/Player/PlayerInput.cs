using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour
{
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //If we are knocked back - read no input
        if(player.currentKnockback.duration > 0)
        {
            player.SetDirectionalInput(Vector2.zero);
            return;
        }

        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if (Input.GetButtonDown("Jump"))
        {
            player.OnJumpInputDown();
        }

        if (Input.GetButton("Jump"))
        {
            player.OnGlide();
        }

        if (Input.GetButtonUp("Jump"))
        {
            player.OnJumpInputUp();
        }

        if (Input.GetButtonDown("Dash"))
        {
            player.OnDash();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            player.OnAttack();
        }

    }
}
