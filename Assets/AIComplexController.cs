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

    private enum States
    {
        MoveTowardsEnemy,
        Attacking,
        Evading,
        Roaming,
        Dead
    }

    [SerializeField]
    public List<Pattern> Patterns;
    public event IFlippable.Action Fliped;
    public float RandomDirectionMoveTime;
    public float DistanceToAgro;
    public float DistanceToSwitchToAttackState;
    public float DistanceForMeleeAttack = 2.0f;


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
    private States _states = States.Roaming;
    private GameObject _enemy;
    private MovementScript _movement;
    private Vector2 _movementDirection = Vector2.right;
    private float _timer;
    private float DistanceToEnemy;

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
        _enemy = FindObjectOfType<PlayerController>().gameObject;
        _movement = GetComponent<MovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleStates();
    }

    void HandleStates()
    {
        if (_enemy == null) {
            return;
        }

        HandleDistanceFromPlayer();
        DistanceToEnemy = Vector2.Distance(_enemy.transform.position, transform.position);

        switch (_states) {
            case States.MoveTowardsEnemy:
                MoveTowardsPlayer();
                break;
            case States.Roaming:
                Roaming();
                break;
            case States.Attacking:
                HandleAttack();
                break;
            case States.Evading:
                break;
            case States.Dead:
                break;
        }
    }

    void MoveTowardsPlayer()
    {
        _movementDirection = (_enemy.transform.position - transform.position).normalized;
        _horizontal = _movementDirection.x;
        _movement.Move(_movementDirection.x, _movementDirection.y);

        if (DistanceToEnemy < DistanceForMeleeAttack) {
            _states = States.Attacking;
            _movement.StopMoving();
            return;
        }
    }

    void HandleDistanceFromPlayer()
    {
        if (DistanceToEnemy > DistanceToAgro) {
            _states = States.Roaming;
            return;
        }
    }

    void Roaming()
    {
        if (_movement.enabled) {
            _timer += Time.deltaTime;

            if (_timer <= RandomDirectionMoveTime) {
                _movement.Move(_movementDirection.x, _movementDirection.y);
                _horizontal = _movementDirection.x;
            } else {
                _timer = 0;
                var rand = Random.Range(-1, 1);
                if (rand != 0) {
                    _movementDirection = new Vector2(rand, 0);
                }
            }
        }

        if (DistanceToEnemy < DistanceToAgro) {
            _states = States.MoveTowardsEnemy;
            Debug.Log("Deagro Player");
            return;
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_states == States.Roaming) {
            _movementDirection = new Vector2(_movementDirection.x * -1f, 0);
        }
    }
}
