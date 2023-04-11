using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("Transition"), NodeTint("#4B8761")]
public class TransitionNode : FSMNodeBase
{
    public SMDecision Decision;
    [Output] public StateNode TrueState;
    [Output] public StateNode FalseState;
    public void Execute(StateMachineGraph stateMachine)
    {
        var trueState = GetFirst<BaseStateNode>(nameof(TrueState));
        var falseState = GetFirst<BaseStateNode>(nameof(FalseState));
        var decision = Decision.Decide(stateMachine);
        if (decision && !(trueState is RemainInStateNode)) {
            stateMachine.ChangeState(trueState);
        } else if (!decision && !(falseState is RemainInStateNode))
            stateMachine.ChangeState(falseState);
    }
}
