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
        public float UsableRangeFromTarget;
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
        _actionHandler = GetComponent<ActionHandler>();
        SetupPatterns();
        _health.ChangeInHealth += RemovePatterns;
        _health.ChangeInHealth += AddPatterns;
    }

    public void HandlePatternsWithinRange(Vector2 target)
    {
        var pattern = _potentialPatterns[_patternIndex];

        if (_actionState == ActionState.Ready) {
            if (!_actionHandler.IsActive) {
                _actionHandler.ActivateActionByID(Vector2.right, pattern.ActionIDs[_actionIndex]);
                Debug.Log("Action Index: " + _actionHandler.CurrentAction.GetName);
                _actionState = ActionState.Waiting;
            }
        } else if (_actionState == ActionState.Waiting) {
            if (!_actionHandler.IsActive) {
                _actionIndex++;

                var potPatterns = _potentialPatterns.Where(x => Vector2.Distance(transform.position, target) <= x.UsableRangeFromTarget).ToArray();

                if (potPatterns.Length != 0) {
                    if (RandomPatternIndex)
                        _patternIndex = Random.Range(0, potPatterns.Length);
                    else {
                        _patternIndex++;
                        if (_patternIndex >= potPatterns.Length) {
                            _patternIndex = 0;
                        }
                    }
                } else {
                    _patternIndex = 0;
                }

                pattern = potPatterns[_patternIndex];

                if (_actionIndex >= pattern.ActionIDs.Count) {
                    _actionIndex = 0;
                }
                _actionState = ActionState.Ready;
            }
        }
    }

    public void HandlePatterns()
    {
        var pattern = _potentialPatterns[_patternIndex];

        if (_actionState == ActionState.Ready) {
            if (!_actionHandler.IsActive) {
                _actionHandler.ActivateActionByID(Vector2.right, pattern.ActionIDs[_actionIndex]);
                Debug.Log("Action Index: " + _actionHandler.CurrentAction.GetName);
                _actionState = ActionState.Waiting;
            }
        } else if (_actionState == ActionState.Waiting) {
            if (!_actionHandler.IsActive) {
                _actionIndex++;

                if (RandomPatternIndex)
                    _patternIndex = Random.Range(0, _potentialPatterns.Count);
                else {
                    _patternIndex++;
                    if (_patternIndex >= _potentialPatterns.Count) {
                        _patternIndex = 0;
                    }
                }

                pattern = _potentialPatterns[_patternIndex];

                if (_actionIndex >= pattern.ActionIDs.Count) { 
                    _actionIndex = 0;
                }
                _actionState = ActionState.Ready;
            }
        }
    }

    void SetupPatterns()
    {
        foreach (var pattern in Patterns) {
            _unUsedPatterns.Add(pattern);
        }

        AddPatterns(_health.MaxHealthValue);
    }

    void AddPatterns(float healthThreshold)
    {
        var hp = ConvertHPtoPercentage(healthThreshold);
        var temp = _unUsedPatterns.Where(x => x.ActivatableRangeMax <= hp && hp > x.ActivatableRangeMin).ToArray();
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
