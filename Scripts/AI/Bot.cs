using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BotStateMachine))]
[RequireComponent(typeof(IBotNavigator))]
[RequireComponent(typeof(BotAttacker))]
public class Bot : MonoBehaviour {

    public bool isStatic = false;
    BotStateMachine stateMachine;
    IBotNavigator navigator;
    BotAttacker attacker;

    // Use this for initialization
    void Awake () {
        stateMachine = GetComponent<BotStateMachine>();
        navigator = GetComponent<IBotNavigator>();
        attacker = GetComponent<BotAttacker>();

        stateMachine.onStateChanged -= OnStateChanged;
        stateMachine.onStateChanged += OnStateChanged;
	}
	
	void OnStateChanged(BotStateMachine.State currentState)
    {
        switch (currentState)
        {
            case BotStateMachine.State.Idle:
                attacker.Stop();
                navigator.Stop();
                break;
            case BotStateMachine.State.Patrol:
                attacker.Stop();

                if (!isStatic)
                    navigator.Patrol();
                break;
            case BotStateMachine.State.KeepAttackDistance:
                attacker.Stop();

                if (!isStatic)
                    navigator.KeepAttackDistance();
                break;
            case BotStateMachine.State.Attack:
                attacker.Attack();
                break;
        }
    }
}
