using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Base State", fileName ="BaseState")]
public class BaseState : ScriptableObject
{
    public virtual void Execute(StateMachine fsm)
    {
    }

    public virtual void OnEnter(StateMachine fsm)
    {
    }

    public virtual void OnExit(StateMachine fsm)
    {
    }
}
