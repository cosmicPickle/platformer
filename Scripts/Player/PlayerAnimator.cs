using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MeleeAttackAgent))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerAnimator : MonoBehaviour {

    Animator animator;
    MeleeAttackAgent lightAttack;
    PlayerInput playerInput;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        lightAttack = GetComponent<MeleeAttackAgent>();
        playerInput = GetComponent<PlayerInput>();

        playerInput.onDirectionalInputChange += OnDirectionalInputChange;
        lightAttack.onAttackStart += OnAttackStart;
        lightAttack.onAttackComplete += OnAttackComplete;
    }

    private void OnAttackComplete()
    {
        animator.SetBool("lightAttack", false);
    }

    private void OnAttackStart()
    {
        animator.SetBool("lightAttack", true);
    }

    void OnDirectionalInputChange(Vector2 directionalInput)
    {
        animator.SetFloat("velocity", Mathf.Abs(directionalInput.x));
    }
}
