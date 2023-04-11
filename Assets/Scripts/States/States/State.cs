using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static XNode.Node;

[CreateAssetMenu(menuName = "FSM/State")]
public sealed class State : BaseState
{
    public List<SMAction> Actions = new List<SMAction>();
    public List<Transition> Transitions = new List<Transition>();
    // Start is called before the first frame update
    public override void Execute(StateMachine fsm)
    {
        foreach(var action in Actions) {
            action.Execute(fsm);
        }

        foreach(var transition in Transitions) {
            transition.Execute(fsm);
        }
    }

    public override void OnEnter(StateMachine fsm)
    {
        foreach (var action in Actions) {
            action.OnActionStart(fsm);
        }
    }

    public override void OnExit(StateMachine fsm)
    {
        foreach (var action in Actions) {
            action.OnActionEnd();
        }
    }
}
