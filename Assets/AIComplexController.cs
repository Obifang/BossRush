using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIComplexController : MonoBehaviour, IFlippable, IController
{
    private enum States
    {
        MoveTowardsEnemy,
        Attacking,
        Evading,
        Roaming,
        Dead
    }

    private PatternHandler _patterns;
    public event IFlippable.Action Fliped;
    public float RandomDirectionMoveTime;
    public float DistanceToAgro;
    public float DistanceToSwitchToAttackState;
    public float DistanceForMeleeAttack = 2.0f;


    private float _horizontal;
    private float _vertical;
    private Health _health;
    private Animator _animator;
    private States _states = States.Roaming;
    private GameObject _enemy;
    private MovementForceBased _movement;
    private Vector2 _movementDirection = Vector2.right;
    private float _timer;
    private float DistanceToEnemy;
    private SpriteRenderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _patterns = GetComponent<PatternHandler>();
        _health = GetComponent<Health>();
        _animator = GetComponent<Animator>();
        _enemy = FindObjectOfType<PlayerController>().gameObject;
        _movement = GetComponent<MovementForceBased>();
        _renderer = GetComponent<SpriteRenderer>();
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
        _movementDirection = (_enemy.transform.position - transform.position).normalized;
        _horizontal = _movementDirection.x;
        Debug.Log("Enemy State: " + _states);
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
                    _patterns.HandlePatternsWithinRange(_enemy.transform.position);
                }
                break;
            case States.Evading:
                break;
            case States.Dead:
                break;
        }

        FlipSprite();
    }

    void MoveTowardsPlayer()
    {
        _movement.Move(_movementDirection.x, _movementDirection.y);

        if (DistanceToEnemy < DistanceForMeleeAttack) {
            _states = States.Attacking;
            _movement.StopMoving();
            _movement.UpdateState(MovementState.Idle);
            Debug.Log("Stopped Moving");
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

    void FlipSprite()
    {
        if (_horizontal == 0) {
            return;
        }

        bool oldValue = _renderer.flipX;

        if (_horizontal < 0) {
            _renderer.flipX = true;

        } else if (_horizontal > 0) {
            _renderer.flipX = false;
        }

        if (oldValue != _renderer.flipX && Fliped != null) {
            Fliped.Invoke(_renderer.flipX);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_states == States.Roaming) {
            _movementDirection = new Vector2(_movementDirection.x * -1f, 0);
        }
    }

    public void SetActive(bool value)
    {
        _patterns.StopCurrentAction();
        this.enabled = value;
    }
}
