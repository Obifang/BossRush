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

    public void Broadcast(GameObject caller, string action)
    {
        if (_storedEvents.ContainsKey(caller)) {
            if (_storedEvents[caller] != null) {
                _storedEvents[caller].Invoke(caller, action);
            }
        }
    }

    public void Subscribe(GameObject subscriber, Action subscriberFunction, GameObject target)
    {
        if (_storedEvents.ContainsKey(target)) {
            _storedEvents[target] += subscriberFunction;
        } else {
            _storedEvents.Add(target, subscriberFunction);
        }
    }

    public void UnSubscribe(GameObject subscriber, Action subscriberFunction, GameObject target)
    {
        if (_storedEvents.ContainsKey(target)) {
            _storedEvents[target] -= subscriberFunction;
        }
    }
    
}
