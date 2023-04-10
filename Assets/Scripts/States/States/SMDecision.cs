using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SMDecision : ScriptableObject
{
    public abstract bool Decide(StateMachine stateMachine);
}
