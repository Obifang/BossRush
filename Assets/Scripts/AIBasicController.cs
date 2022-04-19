using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBasicController : MonoBehaviour, IFlippable
{
    public MovementScript _movement;
    private Movement_Dash _dash;

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

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _dash = GetComponent<Movement_Dash>();
        _attack = GetComponent<Attack>();
        _attackRangeHitScan = GetComponent<AttackRangeHitScan>();
        _attackRangeProjectile = GetComponent<AttackRangeProjectile>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_movement.enabled) {
            _timer += Time.deltaTime;

            if (_timer <= RandomDirectionMoveTime) {
                _movement.Move(_movementDirection.x, _movementDirection.y);
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

        if (oldValue != _renderer.flipX) {
            Fliped.Invoke(_renderer.flipX);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _movementDirection = new Vector2(_movementDirection.x * -1f, 0);
    }
}
