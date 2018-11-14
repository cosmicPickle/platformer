using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BotLocator))]
[RequireComponent(typeof(IBotNavigator))]
[RequireComponent(typeof(BotAttacker))]
public class BotStateMachine : MonoBehaviour {

    public bool showDecisionPath = true;
    public float idleTimeMin = .5f;
    public float idleTimeMax = 1f;
    float idleTime;

    public float patrolTimeMin = 10f;
    public float patrolTimeMax = 20f;
    float patrolTime;

    State currentState = State.None;
    State previousState = State.None;

    BotLocator locator;
    IBotNavigator navigator;
    BotAttacker attacker;
    DecisionTree<BotStateMachine, State> stateDecision;

    public delegate void OnStateChanged(State state);
    public OnStateChanged onStateChanged;

    public void Awake()
    {
        locator = GetComponent<BotLocator>();
        navigator = GetComponent<IBotNavigator>();
        attacker = GetComponent<BotAttacker>();

        stateDecision = new DecisionTree<BotStateMachine, State>
        {
            showDebugInfo = showDecisionPath,
            root = new DecisionTree<BotStateMachine, State>.Node
            {
                Condition = bot => bot.currentState == State.None,
                positive = new DecisionTree<BotStateMachine, State>.Node
                {
                    decision = State.Idle
                },
                negative = new DecisionTree<BotStateMachine, State>.Node
                {
                    Condition = bot => bot.locator.LocateEnemy() != null,
                    positive = new DecisionTree<BotStateMachine, State>.Node
                    {
                        Condition = bot => bot.navigator.IsTargetOnPath(bot.locator.LocateEnemy().transform.position),
                        positive = new DecisionTree<BotStateMachine, State>.Node
                        {
                            Condition = bot => bot.attacker.EnemyInAttackRange(),
                            positive = new DecisionTree<BotStateMachine, State>.Node
                            {
                                decision = State.Attack
                            },
                            negative = new DecisionTree<BotStateMachine, State>.Node
                            {
                                decision = State.KeepAttackDistance
                            }
                        },
                        negative = new DecisionTree<BotStateMachine, State>.Node
                        {
                            decision = State.Patrol
                        }
                    },
                    negative = new DecisionTree<BotStateMachine, State>.Node
                    {
                        Condition = bot => bot.currentState == State.KeepAttackDistance || bot.currentState == State.Attack,
                        positive = new DecisionTree<BotStateMachine, State>.Node
                        {
                            decision = State.Idle
                        },
                        negative = new DecisionTree<BotStateMachine, State>.Node
                        {
                            Condition = bot => bot.currentState == State.Idle,
                            positive = new DecisionTree<BotStateMachine, State>.Node
                            {
                                Condition = bot => bot.idleTime > 0,
                                positive = new DecisionTree<BotStateMachine, State>.Node
                                {
                                    decision = State.Idle
                                },
                                negative = new DecisionTree<BotStateMachine, State>.Node
                                {
                                    decision = State.Patrol
                                }
                            },
                            negative = new DecisionTree<BotStateMachine, State>.Node
                            {
                                Condition = bot => bot.currentState == State.Patrol,
                                positive = new DecisionTree<BotStateMachine, State>.Node
                                {
                                    Condition = bot => bot.patrolTime > 0,
                                    positive = new DecisionTree<BotStateMachine, State>.Node
                                    {
                                        decision = State.Patrol
                                    },
                                    negative = new DecisionTree<BotStateMachine, State>.Node
                                    {
                                        decision = State.Idle
                                    }
                                },
                                negative = new DecisionTree<BotStateMachine, State>.Node
                                {
                                    decision = State.Idle
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private void Update()
    {
        previousState = currentState;
        currentState = stateDecision.Evaluate(this);   

        if(currentState != previousState)
        {
            if(currentState == State.Idle)
            {
                idleTime = Random.Range(idleTimeMin, idleTimeMax);
            }

            if (currentState == State.Patrol)
            {
                patrolTime = Random.Range(patrolTimeMin, patrolTimeMax);
            }

            
            if (onStateChanged != null)
            {
                onStateChanged(currentState);
            }
        }

        if(idleTime > 0)
        {
            idleTime -= Time.deltaTime;
        } else
        {
            idleTime = 0;
        }

        if(patrolTime > 0)
        {
            patrolTime -= Time.deltaTime;
        } else
        {
            patrolTime = 0;
        }
    }

    public enum State
    {
        None,
        Idle,
        Patrol,
        KeepAttackDistance,
        Attack
    }
}
