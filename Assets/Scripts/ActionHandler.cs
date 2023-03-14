using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionHandler : MonoBehaviour
{
    private Dictionary<int, IActionable> _actionsByID;
    private Dictionary<string, IActionable> _actionsByName;
    private IActionable _currentAction;
    private bool _interuptFlag = false;

    public IActionable CurrentAction { get => _currentAction;}
    public bool IsActive { get => _currentAction.IsActive;}
    public bool IsInteruptAble { get => _interuptFlag;}

    // Start is called before the first frame update
    void Start()
    {
        _actionsByID = new Dictionary<int, IActionable>();
        _actionsByName = new Dictionary<string,IActionable>();
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

            if (!_actionsByName.ContainsKey(actionable.GetName)) {
                _actionsByName.Add(actionable.GetName, actionable);
            } else {
                Debug.LogWarning("Warning: Multiple Actions share the same Name." + "\nName: " + actionable.GetName);
            }
        }
    }

    public bool ActivateActionByID(Vector2 direction, int id, bool interupt = false)
    {
        if (_interuptFlag == false && _currentAction != null && _currentAction.IsActive) {
            return false;
        }

        if (!_actionsByID.ContainsKey(id) || !_actionsByID[id].CanActivate(direction)) {
            return false;
        }

        _interuptFlag = interupt;
        _currentAction.Deactivate(direction);
        _currentAction = _actionsByID[id];
        _currentAction.Activate(direction);

        return true;
    }

    public bool ActivateActionByName(Vector2 direction, string name, bool interupt = false)
    {
        if (_interuptFlag == false && _currentAction != null && _currentAction.IsActive) {
            return false;
        }

        if (!_actionsByName.ContainsKey(name) || !_actionsByName[name].CanActivate(direction)) {
            return false;
        }

        _interuptFlag = interupt;
        _currentAction.Deactivate(direction);
        _currentAction = _actionsByName[name];
        _currentAction.Activate(direction);

        return true;
    }

    public void AddAction(IActionable action)
    {
        _actionsByID.Add(action.GetID, action);
    }

    public void RemoveAction(int id)
    {
        _actionsByID.Remove(id);
    }

    public bool CanActivateAction(Vector2 direction, int id)
    {
        return _actionsByID[id].CanActivate(direction);
    }

    public bool CanActivateAction(Vector2 direction, string name)
    {
        return _actionsByName[name].CanActivate(direction);
    }
}
