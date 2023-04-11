using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateNodeMenu("State"), NodeTint("#2C4DAD")]
public class StateNode : BaseStateNode
{
    public List<SMAction> Actions;
    [Output] public List<TransitionNode> Transitions;
    public void Execute(StateMachineGraph baseStateMachine)
    {
        foreach (var action in Actions)
            action.Execute(baseStateMachine);
        foreach (var transition in GetAllOnPort<TransitionNode>(nameof(Transitions)))
            transition.Execute(baseStateMachine);
    }

    public void OnEnter(StateMachine fsm)
    {
        foreach (var action in Actions) {
            action.OnActionStart(fsm);
        }
    }

    public void OnExit(StateMachine fsm)
    {
        foreach (var action in Actions) {
            action.OnActionEnd();
        }
    }
}
