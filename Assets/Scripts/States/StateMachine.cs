using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField]
    private BaseState _initialState;

    private Dictionary<Type, Component> _cachedComponents;

    public BaseState CurrentState { get; set; }
    void Awake()
    {
        Init();
        _cachedComponents = new Dictionary<Type, Component>();
    }

    #region Xnode Code Implimentation
    public virtual void Init()
    {
        CurrentState = _initialState;
    }

    public virtual void Execute()
    {
        CurrentState.Execute(this);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        Execute();
    }

    public virtual void ChangeState(BaseState newState)
    {
        if (CurrentState == newState) {
            return;
        }

        CurrentState.OnExit(this);
        CurrentState = newState;
        CurrentState.OnEnter(this);
    }

    public new T GetComponent<T>() where T : Component
    {
        if (_cachedComponents.ContainsKey(typeof(T)))
            return _cachedComponents[typeof(T)] as T;

        var component = base.GetComponent<T>();
        if (component != null) {
            _cachedComponents.Add(typeof(T), component);
        }
        return component;
    }

    public T FindComponent<T>() where T : Component
    {
        if (_cachedComponents.ContainsKey(typeof(T)))
            return _cachedComponents[typeof(T)] as T;

        var component = FindObjectOfType<T>();
        if (component != null) {
            _cachedComponents.Add(typeof(T), component);
        }
        return component;
    }
}
