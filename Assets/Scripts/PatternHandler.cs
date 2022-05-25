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
        public List<int> ActionIDs;
        public float MaxUsableRangeFromTarget;
        public float MinUsableRangeFromTarget;
        [Range(0, 100)]
        public int ActivatableRangeMax;
        [Range(0, 100)]
        public int ActivatableRangeMin;
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
    private List<Pattern> _unUsedPatterns;
    private Health _health;
    private ActionHandler _actionHandler;
    private ActionState _actionState;
    private int _patternIndex;
    private int _actionIndex;

    // Start is called before the first frame update
    void Start()
    {
        _potentialPatterns = new List<Pattern>();
        _unUsedPatterns = new List<Pattern>();
        _distancePatterns = new List<Pattern>();
        _actionHandler = GetComponent<ActionHandler>();
        _health = GetComponent<Health>();
        SetupPatterns();
        _health.ChangeInHealth += RemovePatterns;
        _health.ChangeInHealth += AddPatterns;
    }

    public void HandlePatternsWithinRange(Vector2 target)
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
                    _actionHandler.ActivateActionByID((target - (Vector2)transform.position).normalized, pattern.ActionIDs[_actionIndex]);
                    Debug.Log("Pattern Index: " + _patternIndex);
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
                pattern = _distancePatterns[_patternIndex];

                if (_actionIndex >= pattern.ActionIDs.Count) {
                    _actionIndex = 0;

                    if (_distancePatterns.Count != 0) {
                        if (RandomPatternIndex)
                            _patternIndex = Random.Range(0, _distancePatterns.Count);
                        else {
                            _patternIndex++;
                        }
                    }
                    if (_patternIndex >= _distancePatterns.Count) {
                        _patternIndex = 0;
                    }
                }
                _actionState = ActionState.Ready;
                break;
        }
    }

    public void HandlePatterns()
    {
        var pattern = _potentialPatterns[_patternIndex];

        switch (_actionState) {
            case ActionState.Ready:
                if (!_actionHandler.IsActive) {
                    _actionHandler.ActivateActionByID(Vector2.right, pattern.ActionIDs[_actionIndex]);
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

    void SetupPatterns()
    {
        foreach (var pattern in Patterns) {
            _unUsedPatterns.Add(pattern);
        }

        AddPatterns(_health.MaxHealthValue);
    }

    void AddPatternsDistance(float distance)
    {
        _distancePatterns = _potentialPatterns.Where(x => distance <= x.MaxUsableRangeFromTarget &&
                                                                distance > x.MinUsableRangeFromTarget).ToList();

        if (_distancePatterns.Count == 0) {
            _distancePatterns = _potentialPatterns;
        }
    }

    void AddPatterns(float healthThreshold)
    {
        var hp = ConvertHPtoPercentage(healthThreshold);
        var temp = _unUsedPatterns.Where(x => hp <= x.ActivatableRangeMax && hp > x.ActivatableRangeMin).ToArray();
        foreach (Pattern i in temp) {
            _potentialPatterns.Add(i);
            _unUsedPatterns.Remove(i);
        }
    }

    void RemovePatterns(float healthThreshold)
    {
        var hp = ConvertHPtoPercentage(healthThreshold);
        var temp = _potentialPatterns.Where(x => x.ActivatableRangeMin > hp).ToArray();
        foreach (Pattern i in temp) {
            _potentialPatterns.Remove(i);
            _unUsedPatterns.Add(i);
        }
    }

    float ConvertHPtoPercentage(float health)
    {
        if (_health.MaxHealthValue == 0) {
            return 0;
        }

        return (health / _health.MaxHealthValue * 100);
    }
}
