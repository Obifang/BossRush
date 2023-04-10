using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Actions/Move To Player")]
public class State_MoveToPlayer : SMAction
{
    public override void Execute(StateMachine stateMachine)
    {
        var movementDirection = (stateMachine.FindComponent<Controller_Player>().transform.position - stateMachine.transform.position).normalized;
        var controller = stateMachine.GetComponent<Controller_Movement>();

        controller.Move(movementDirection.x, movementDirection.y);
    }
}
