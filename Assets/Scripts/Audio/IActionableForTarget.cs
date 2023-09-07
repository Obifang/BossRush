using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface used for when an IActionable requires a target rather than a position.
/// </summary>
public interface IActionableForTarget : IActionable
{
    /// <summary>
    /// Use to start an Action. Should not require Deactivate to stop once completed.
    /// </summary>
    /// <param name="position"></param>
    public void Activate(Transform target);
}
