using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineGraph : StateMachine
{
    public new BaseStateNode CurrentState { get; set; }

    [SerializeField] private FSMGraph _graph;
    public override void Init()
    {
        CurrentState = _graph.InitialState;
    }
    public override void Execute()
    {
        ((StateNode)CurrentState).Execute(this);
        Debug.Log(CurrentState);
    }

    public void ChangeState(BaseStateNode newState)
    {
        if (CurrentState == newState) {
            return;
        }

        ((StateNode)CurrentState).OnExit(this);
        CurrentState = newState;
        ((StateNode)CurrentState).OnEnter(this);
    }

}
