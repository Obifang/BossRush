using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Transition")]
public class Transition : ScriptableObject
{
    public SMDecision Decision;
    public BaseState TrueState;
    public BaseState FalseState;

    public void Execute(StateMachine stateMachine)
    {
        if (Decision.Decide(stateMachine) && !(TrueState is RemainInState)) {
            stateMachine.ChangeState(TrueState);
        } else if (!(FalseState is RemainInState)) {
            stateMachine.ChangeState(FalseState);
        }
    }
}
