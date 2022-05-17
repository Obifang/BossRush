using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionable
{
    public int GetID { get;}
    public string GetName { get;}
    public void Activate(Vector2 direction);
    public bool IsActive { get;}
}
