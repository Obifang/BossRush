using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasState<T>
{
    public T GetCurrentState();
    public void UpdateState(T state);
    public T GetPreviousState();
}
