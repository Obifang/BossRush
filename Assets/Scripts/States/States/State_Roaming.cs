using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Actions/Roaming")]
public class State_Roaming : SMAction
{
    public override void Execute(StateMachine stateMachine)
    {
        var rand = Random.Range(-1, 1);
        var movementDirection = Vector2.zero;
        if (rand != 0) {
            movementDirection = new Vector2(rand, 0);
        }
        var controller = stateMachine.GetComponent<Controller_Movement>();

        controller.Move(movementDirection.x, movementDirection.y);
    }
}
