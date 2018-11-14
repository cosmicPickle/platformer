using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour
{
    public delegate void OnDirectionalInputChange(Vector2 directionalInput);
    public OnDirectionalInputChange onDirectionalInputChange;

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
        if (player.currentKnockback.duration > 0)
        {
            player.SetDirectionalInput(Vector2.zero);
            return;
        }

        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(onDirectionalInputChange != null)
        {
            onDirectionalInputChange(directionalInput);
        }
        
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

            MeleeAttackAgent.AttackType attackType = MeleeAttackAgent.AttackType.Default;

            if (directionalInput.y > 0)
            {
                attackType = MeleeAttackAgent.AttackType.Above;
            }
            else if (directionalInput.y < 0
                    && !player.controller.collisions.below
                    && !player.controller.collisions.left
                    && !player.controller.collisions.right)
            {
                attackType = MeleeAttackAgent.AttackType.Below;
            } 

            player.OnLightAttack(attackType);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            player.OnHeavyAttack();
        }
    }
}
