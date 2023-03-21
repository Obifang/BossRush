using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ActionMonitorer : MonoBehaviour
{
    public static ActionMonitorer Instance { get; private set; }

    private event Action ActionCalled;
    public delegate void Action(GameObject subscriber, string action);

    private Dictionary<GameObject, Action> _storedEvents;

    public void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
            _storedEvents = new Dictionary<GameObject, Action>();
        }
    }

    /// <summary>
    /// Used to call all delegates subscribed to the the GameObject caller.
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="action">A simple string passed that references the action called.</param>
    public void Broadcast(GameObject caller, string action)
    {
        if (_storedEvents.ContainsKey(caller)) {
            if (_storedEvents[caller] != null) {
                _storedEvents[caller].Invoke(caller, action);
            }
        }
    }

    /// <summary>
    /// Adds a delegate listener to be called when the target is used in the Broadcast method
    /// </summary>
    /// <param name="subscriberFunction"></param>
    /// <param name="target"></param>
    public void Subscribe(Action subscriberFunction, GameObject target)
    {
        if (_storedEvents.ContainsKey(target)) {
            _storedEvents[target] += subscriberFunction;
        } else {
            _storedEvents.Add(target, subscriberFunction);
        }
    }

    /// <summary>
    /// Removes a delegate listener from the target.
    /// </summary>
    /// <param name="subscriberFunction"></param>
    /// <param name="target"></param>
    public void UnSubscribe(Action subscriberFunction, GameObject target)
    {
        if (_storedEvents.ContainsKey(target)) {
            _storedEvents[target] -= subscriberFunction;
        }
    }
    
}
