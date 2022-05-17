using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIComplexController : MonoBehaviour, IFlippable
{
    /// <summary>
    /// A representation of Data with regards to an Attack pattern that makes use of multiple ID for Actions.
    /// </summary>
    [System.Serializable]
    public struct Pattern
    {
        public int ID;
        public List<int> ActionIDs;
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
    public event IFlippable.Action Fliped;

    private Dictionary<int, Pattern> _patternsByID;
    private Dictionary<int, IActionable> _actionables;
    private List<int> _potentialPatterns;
    private List<int> _unUsedPatterns;
    private float _horizontal;
    private float _vertical;
    private Health _health;
    private Animator _animator;
    private int _actionIndex = 0;
    private ActionState _actionState;
    // Start is called before the first frame update
    void Start()
    {
        _patternsByID = new Dictionary<int, Pattern>();
        _actionables = new Dictionary<int, IActionable>();
        _potentialPatterns = new List<int>();
        _unUsedPatterns = new List<int>();
        _health = GetComponent<Health>();
        SetupIActionables();
        SetupPatterns();
        _health.ChangeInHealth += RemovePatterns;
        _health.ChangeInHealth += AddPatterns;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleAttack();
    }

    void HandleAttack()
    {
        var pattern = _patternsByID[_potentialPatterns[0]];
        var action = _actionables[pattern.ActionIDs[_actionIndex]];

        if (_actionState == ActionState.Ready) {
            if (!action.IsActive) {
                Debug.Log("Action Index: " + action.GetName);
                action.Activate(Vector2.right);
                _animator.SetTrigger("Attack" + 1);
                _actionState = ActionState.Waiting;
            }
        } else if (_actionState ==  ActionState.Waiting) {
            if (!action.IsActive) {
                _actionIndex++;
                if (_actionIndex >= pattern.ActionIDs.Count) {
                    _actionIndex = 0;
                }
                _actionState = ActionState.Ready;
            }
        }
    }

    void ExecuteCurrentPattern()
    {

    }

    /// <summary>
    /// Gets a list of all "IActionable" Components attached to this object and adds them to a dictionary based on ID.
    /// </summary>
    void SetupIActionables()
    {
        var actionables = GetComponents<IActionable>().ToList();

        foreach (var actionable in actionables) {
            _actionables.Add(actionable.GetID, actionable);
        }
    }

    void SetupPatterns()
    {
        foreach (var pattern in Patterns) {
            _patternsByID.Add(pattern.ID, pattern);
            _unUsedPatterns.Add(pattern.ID);
        }

        AddPatterns(_health.MaxHealthValue);
    }

    void AddPatterns(float health)
    {
        var hp = ConvertHPtoPercentage(health);
        var temp = _unUsedPatterns.Where(x => _patternsByID[x].ActivatableRangeMax <= hp && hp > _patternsByID[x].ActivatableRangeMin).ToArray();
        foreach (int i in temp) {
            Debug.Log(i);
            _potentialPatterns.Add(i);
            _unUsedPatterns.Remove(i);
        }
    }

    void RemovePatterns(float health)
    {
        var hp = ConvertHPtoPercentage(health);
        var temp = _potentialPatterns.Where(x => _patternsByID[x].ActivatableRangeMin > hp).ToArray();
        foreach(int i in temp) {
            _potentialPatterns.Remove(i);
        }
    }

    void FlipSprite()
    {
        if (_horizontal == 0) {
            return;
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
