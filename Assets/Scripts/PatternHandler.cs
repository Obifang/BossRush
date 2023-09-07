using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PatternHandler : MonoBehaviour
{
    /// <summary>
    /// A representation of Data with regards to an Attack pattern that makes use of multiple ID for Actions.
    /// </summary>
    [System.Serializable]
    public struct Pattern
    {
        public List<string> ActionIDs;
        public int Weighting;
        public float MaxUsableRangeFromTarget;
        public float MinUsableRangeFromTarget;
    }

    public enum ActionState
    {
        Ready,
        Waiting
    }

    [SerializeField]
    public List<Pattern> Patterns;
    public bool RandomPatternIndex = true;

    public int GetCurrentPatternCount { get => _potentialPatterns.Count; }
    public int GetCurrentPatternCountDistance { get => _distancePatterns.Count; }
    public string GetCurrentActionName { get => _actionHandler.CurrentAction.GetName; }

    private List<Pattern> _potentialPatterns;
    private List<Pattern> _distancePatterns;
    private ActionHandler _actionHandler;
    private ActionState _actionState;
    private int _patternIndex;
    private int _actionIndex;

    // Start is called before the first frame update
    void Awake()
    {
        _potentialPatterns = new List<Pattern>();
        _distancePatterns = new List<Pattern>();
        _actionHandler = GetComponent<ActionHandler>();
    }

    public bool IsCurrentActionActive()
    {
        return _actionHandler.IsActive;
    }

    public void StopCurrentAction()
    {
        _actionHandler.CurrentAction.Deactivate(Vector2.zero);
    }

    public bool HandlePatternsWithinRange(Vector2 target)
    {
        if (_distancePatterns.Count == 0) {
            var distance = Vector2.Distance(transform.position, target);
            
            AddPatternsDistance(distance);
            _actionIndex = 0;
            _patternIndex = 0;
        }
        var pattern = _distancePatterns[_patternIndex];

        switch (_actionState) {
            case ActionState.Ready:
                if (!_actionHandler.IsActive) {
                    _actionState = ActionState.Waiting;
                    if (!_actionHandler.CanActivateAction(target, pattern.ActionIDs[_actionIndex])) {
                        return false;
                    }
                    _actionHandler.ActivateAction(target, pattern.ActionIDs[_actionIndex]);
                    _actionState = ActionState.Waiting;
                }
                break;
            case ActionState.Waiting:
                if (_actionHandler.IsActive) {
                    break;
                }

                _actionIndex++;
                
                //Early break if there are still actions to be done.
                if (_actionIndex < pattern.ActionIDs.Count) {
                    pattern = _distancePatterns[_patternIndex];
                    _actionState = ActionState.Ready;
                    break;
                } else {
                    _actionIndex = 0;
                }

                var distance = Vector2.Distance(transform.position, target);
                AddPatternsDistance(distance);

                if (_distancePatterns.Count != 0) {
                    if (RandomPatternIndex) {
                        var patternSet = _distancePatterns.ToDictionary(x => x, x => x.Weighting);
                        pattern = Random_Weighted<Pattern>.GetRandomObject(patternSet);

                        _patternIndex = _distancePatterns.IndexOf(pattern);
                    }
                    else {
                        _patternIndex++;
                    }
                }

                if (_patternIndex >= _distancePatterns.Count) {
                    _actionIndex = 0;
                    _patternIndex = 0;
                }

                pattern = _distancePatterns[_patternIndex];

                _actionState = ActionState.Ready;
                break;
        }

        return true;
    }

    public void HandlePatterns()
    {
        var pattern = _potentialPatterns[_patternIndex];

        switch (_actionState) {
            case ActionState.Ready:
                if (!_actionHandler.IsActive) {
                    _actionHandler.ActivateAction(Vector2.right, pattern.ActionIDs[_actionIndex]);
                    Debug.Log("Action Index: " + _actionHandler.CurrentAction.GetName);
                    _actionState = ActionState.Waiting;
                }
                break;
            case ActionState.Waiting:
                if (_actionHandler.IsActive) {
                    break;
                }

                _actionIndex++;

                if (_actionIndex >= pattern.ActionIDs.Count) {
                    _actionIndex = 0;

                    if (RandomPatternIndex)
                        _patternIndex = Random.Range(0, _potentialPatterns.Count);
                    else {
                        _patternIndex++; 
                    }

                    if (_patternIndex >= _potentialPatterns.Count) {
                        _patternIndex = 0;
                    }
                }
                _actionState = ActionState.Ready;
                break;
        }
    }

    void AddPatternsDistance(float distance)
    {
        _distancePatterns = _potentialPatterns.Where(x => (distance <= x.MaxUsableRangeFromTarget) &&
                                                                (distance > x.MinUsableRangeFromTarget)).ToList();
        /*Debug.Log("Potential Pattern Count: " + _potentialPatterns.Count);
        Debug.Log("Distance Pattern Count: " + _distancePatterns.Count);*/
        if (_distancePatterns.Count == 0) {
            _distancePatterns = _potentialPatterns;
        }
    }

    bool IsWithinDistance(int ID, float distance)
    {
        var pattern = Patterns[ID];
        return (distance <= pattern.MaxUsableRangeFromTarget && distance > pattern.MinUsableRangeFromTarget);
    }

    public void SetPatternByIDs(List<int> ids)
    {
        _potentialPatterns.Clear();
        _distancePatterns.Clear();

        foreach (int i in ids) {
            _potentialPatterns.Add(Patterns[i]);
        }
    }

    public void AddPatternToCurrentPool(int ID)
    {
        if (ID >= 0 && ID < Patterns.Count) {
            var pattern = Patterns[ID];
            var newPattern = _potentialPatterns.Find(x => x.ActionIDs == pattern.ActionIDs);

            if (newPattern.Equals(null)) {
                _potentialPatterns.Clear();
                _distancePatterns.Clear();
                _potentialPatterns.Add(pattern);
            }
        }
    }

    public bool IsPatternCurrentlyUseable(int ID, bool requireRangeCheck = false)
    {
        if (requireRangeCheck) {
            if (_distancePatterns.Contains(Patterns[ID])) {
                Debug.Log("Pattern Useable: " + ID);
                return true;
            }

            return false;
        } else if (_potentialPatterns.Contains(Patterns[ID])) {
            return true;
        }

        return false;
    }

    public void ForcePatternStart(int ID, bool requireRangeCheck = false)
    {
        if (requireRangeCheck) {
            if (_distancePatterns.Contains(Patterns[ID])) {
                int indx = _distancePatterns.IndexOf(Patterns[ID]);
                _patternIndex = indx;
                _actionIndex = 0;
                _actionState = ActionState.Ready;
            }
        } else if (_potentialPatterns.Contains(Patterns[ID])) {
            int indx = _potentialPatterns.IndexOf(Patterns[ID]);
            _patternIndex = indx;
            _actionIndex = 0;
            _actionState = ActionState.Ready;
        }
    }
}
