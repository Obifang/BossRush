using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlippable
{
    public event Action Fliped;
    public delegate void Action(bool value);
}
