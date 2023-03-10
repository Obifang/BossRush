using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionable
{
    public int GetID { get;}
    public string GetName { get;}
    /// <summary>
    /// Use to start an Action. Should not require Deactivate to stop once completed.
    /// </summary>
    /// <param name="position"></param>
    public void Activate(Vector2 position);
    /// <summary>
    /// Use to stop an Action.
    /// </summary>
    /// <param name="direction"></param>
    public void Deactivate(Vector2 direction);
    public bool CanActivate(Vector2 direction);
    public bool IsActive { get;}
}
