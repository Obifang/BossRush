using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Actions/Roaming")]
public class State_Roaming : SMAction
{
    public float RoamingDirectionTime;

    [NonSerialized]
    private bool _locked;
    [NonSerialized]
    private Vector2 _movementDirection;
    public override void Execute(StateMachine stateMachine)
    {
        if (!_locked) {
            var rand = UnityEngine.Random.Range(-1, 1);
            if (rand != 0) {
                _movementDirection = new Vector2(rand, 0);
                stateMachine.StartCoroutine(StartCooldown(RoamingDirectionTime));
            }
            
        }
        var controller = stateMachine.GetComponent<Controller_Movement>();
        controller.Move(_movementDirection.x, _movementDirection.y);
        Debug.Log(_locked);
    }

    private IEnumerator StartCooldown(float value)
    {
        _locked = true;
        yield return new WaitForSecondsRealtime(value);
        _locked = false;
    }

    public override void OnActionStart(StateMachine stateMachine)
    {
        _locked = false;
        Debug.Log("Unlocked");
    }
}
