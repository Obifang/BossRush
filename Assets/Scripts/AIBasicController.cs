using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBasicController : MonoBehaviour, IFlippable
{
    private enum State
    {
        Roaming,
        MovingTowardsPlayer,
        Attacking
    }

    public LayerMask EnemyLayers;
    public MovementScript _movement;
    private Movement_Dash _dash;
    public float AttackDistance;
    public float AttackRate = 1.0f;


    private Rigidbody2D _rb;
    private SpriteRenderer _renderer;
    private IActionable _attackRangeProjectile;
    private IActionable _attackRangeHitScan;
    private IActionable _attack;

    private float _horizontal;
    private float _vertical;

    public event IFlippable.Action Fliped;
    public float RandomDirectionMoveTime;

    private float _timer;
    private Vector2 _movementDirection = Vector2.right;
    private State _state = State.Roaming;
    private GameObject _player;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _dash = GetComponent<Movement_Dash>();
        _attack = GetComponent<Attack>();
        _attackRangeHitScan = GetComponent<AttackRangeHitScan>();
        _attackRangeProjectile = GetComponent<AttackRangeProjectile>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();

        switch (_state) {
            case State.Roaming:
                Roaming();
                break;
            case State.MovingTowardsPlayer:
                MoveTowardsPlayer();
                break;
            case State.Attacking:
                AttackPlayer();
                break;
        }
       
    }

    void AttackPlayer()
    {
        if (_attack == null || _player == null) {
            _state = State.Roaming;
            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= AttackRate) {
            var dir = (_player.transform.position - transform.position).normalized;
            _horizontal = dir.x;
            _animator.SetTrigger("Attack" + 1);
            _attack.Activate(dir);
            _timer = 0f;
        }
    }

    void MoveTowardsPlayer()
    {
        if (_player == null) {
            _state = State.Roaming;
            return;
        }
        _movementDirection = (_player.transform.position - transform.position).normalized;
        _movement.Move(_movementDirection.x, _movementDirection.y);
        _horizontal = _movementDirection.x;

        if (Vector2.Distance(transform.position, _player.transform.position) <= AttackDistance) {
            _timer = AttackRate;
            _state = State.Attacking;
            _horizontal = _movementDirection.x;
            _movement.StopMoving();
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
        if (_state == State.Roaming) {
            _movementDirection = new Vector2(_movementDirection.x * -1f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_state == State.Roaming && 1 << collision.gameObject.layer == EnemyLayers) {
            _movementDirection = (collision.transform.position - transform.position).normalized;
            _state = State.MovingTowardsPlayer;
            _player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer == EnemyLayers) {
            _state = State.Roaming;
        }
    }
}
