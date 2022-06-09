using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionHandler : MonoBehaviour
{
    private Dictionary<int, IActionable> _actionsByID;
    private IActionable _currentAction;

    public IActionable CurrentAction { get => _currentAction;}
    public bool IsActive { get => _currentAction.IsActive;}

    // Start is called before the first frame update
    void Start()
    {
        _actionsByID = new Dictionary<int, IActionable>();
        SetupActionDictionaries();
        _currentAction = _actionsByID[_actionsByID.Keys.First()];
    }

    void SetupActionDictionaries()
    {
        var actionables = GetComponents<IActionable>().ToList();

        foreach (var actionable in actionables) {
            if (!_actionsByID.ContainsKey(actionable.GetID)) {
                _actionsByID.Add(actionable.GetID, actionable);
            } else {
                Debug.LogWarning("Warning: Multiple Actions share the same ID." + "\nID: " + actionable.GetID);
            }
        }
    }

    public void ActivateActionByID(Vector2 direction, int id)
    {
        if (_currentAction != null && _currentAction.IsActive) {
            return;
        }

        if (!_actionsByID.ContainsKey(id)) {
            return;
        }

        _currentAction = _actionsByID[id];
        _currentAction.Activate(direction);
    }

    public void AddAction(IActionable action)
    {
        _actionsByID.Add(action.GetID, action);
    }

    public void RemoveAction(int id)
    {
        _actionsByID.Remove(id);
    }
}
