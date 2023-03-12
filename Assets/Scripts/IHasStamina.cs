using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasStamina
{
    public delegate void OutOfStamina();
    public event OutOfStamina OnOutOfStamina;

    public float GetCurrentStamina { get; }
    public void ReduceStamina(float value);
    public void IncreaseStamina(float value);
    public void SetStamina(float value);
    public void SetMaxStamina(float value);
}
