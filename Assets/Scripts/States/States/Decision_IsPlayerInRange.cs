using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/WithinRange")]
public class Decision_IsPlayerInRange : SMDecision
{
    public float RangeCheck = 10.0f;
    public override bool Decide(StateMachine stateMachine)
    {
        return Vector2.Distance(stateMachine.FindComponent<Controller_Player>().transform.position, stateMachine.transform.position) < RangeCheck;
    }
}
