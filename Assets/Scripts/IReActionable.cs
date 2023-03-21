using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReActionable : IActionable
{
    public void UpdateConditionFromAction(GameObject user, string Action);
    public bool ConditionsMet();
}
