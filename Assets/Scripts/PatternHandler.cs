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
        return _actionHandler.CurrentAction.IsActive;
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
            _patternIndex = 0;
        }
        var pattern = _distancePatterns[_patternIndex];

        switch (_actionState) {
            case ActionState.Ready:
                if (!_actionHandler.IsActive) {
                    var canActivate = _actionHandler.ActivateActionByName(target, pattern.ActionIDs[_actionIndex]);
                    if (!canActivate) {
                        return false;
                    }
                    _actionState = ActionState.Waiting;
                }
                break;
            case ActionState.Waiting:
                if (_actionHandler.IsActive) {
                    break;
                }

                _actionIndex++;
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
                    _patternIndex = 0;
                }

                pattern = _distancePatterns[_patternIndex];

                if (_actionIndex >= pattern.ActionIDs.Count) {
                    _actionIndex = 0;
                }
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
                    _actionHandler.ActivateActionByName(Vector2.right, pattern.ActionIDs[_actionIndex]);
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
        _distancePatterns = _potentialPatterns.Where(x => distance <= x.MaxUsableRangeFromTarget &&
                                                                distance > x.MinUsableRangeFromTarget).ToList();

        if (_distancePatterns.Count == 0) {
            _distancePatterns = _potentialPatterns;
        }
    }

    public void SetPatternByIDs(List<int> ids)
    {
        _potentialPatterns.Clear();
        _distancePatterns.Clear();

        foreach (int i in ids) {
            _potentialPatterns.Add(Patterns[i]);
        }
    }
}
