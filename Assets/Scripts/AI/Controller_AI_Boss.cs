using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_AI_Boss : BaseController
{
    private enum States
    {
        MoveTowardsEnemy,
        Attacking,
        Evading,
        Roaming,
        Dead
    }

    //Public variables
    public float RandomDirectionMoveTime;
    public float DistanceToAgro;
    public float DistanceToSwitchToAttackState;
    public float DistanceForMeleeAttack = 2.0f;

    //Private variables
    private PatternHandler _patterns;
    private Health _health;
    private States _states = States.Roaming;
    private GameObject _enemy;
    private Controller_Movement _movement;
    private Vector2 _movementDirection = Vector2.right;
    private float _timer;
    private float DistanceToEnemy;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _patterns = GetComponent<PatternHandler>();
        _health = GetComponent<Health>();
        _animator = GetComponent<Animator>();
        _enemy = FindObjectOfType<Controller_Player>().gameObject;
        _movement = GetComponent<Controller_Movement>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        HandleStates();
    }

    public override void SetActive(bool value)
    {
        this.enabled = value;
    }

    void HandleStates()
    {
        if (_enemy == null) {
            return;
        }
        Debug.Log("Current State: " + _states);
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
                if (DistanceToEnemy > DistanceForMeleeAttack) {
                    if (!_patterns.IsCurrentActionActive()) {
                        _states = States.MoveTowardsEnemy;
                        _movement.UpdateState(MovementState.Moving);
                        _patterns.StopCurrentAction();
                    }
                } else {
                    _movement.UpdateState(MovementState.PerformingAction);
                    _patterns.HandlePatternsWithinRange(_enemy.transform.position);
                }
                break;
            case States.Evading:
                break;
            case States.Dead:
                break;
        }
    }

    void MoveTowardsPlayer()
    {
        _movement.Move(_movementDirection.x, _movementDirection.y);
        _movementDirection = (_enemy.transform.position - transform.position).normalized;
        _horizontal = _movementDirection.x;

        if (DistanceToEnemy < DistanceForMeleeAttack) {
            _states = States.Attacking;
            _movement.StopMoving();
            _movement.UpdateState(MovementState.PerformingAction);
            return;
        }

        if (DistanceToEnemy > DistanceToAgro) {
            _states |= States.Roaming;
            _movement.UpdateState(MovementState.Moving);
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
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_states == States.Roaming) {
            _movementDirection = new Vector2(_movementDirection.x * -1f, 0);
        }
    }
}
