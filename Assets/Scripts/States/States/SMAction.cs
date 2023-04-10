using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SMAction : ScriptableObject
{
    public abstract void Execute(StateMachine stateMachine);
    public virtual void OnActionStart(StateMachine stateMachine) { }
    public virtual void OnActionEnd() { }
}
