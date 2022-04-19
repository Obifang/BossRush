using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IFlippable
{
    public MovementScript _movement;
    private Movement_Dash _dash;

    private Rigidbody2D _rb;
    private SpriteRenderer _renderer;
    private IActionable _attackRangeProjectile;
    private IActionable _attackRangeHitScan;
    private IActionable _attack;
    private Vector2 _facingDirection = Vector2.right;
    private Animator _animator;

    private float _horizontal;
    private float _vertical;

    public event IFlippable.Action Fliped;

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

        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) {
            _movement.Jump();
        }

        _movement.Move(_horizontal, _vertical);

        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            _dash.Dash(_horizontal, _rb);
            _animator.SetTrigger("Roll");
        }

        if (Input.GetMouseButtonDown(0)) {
            _animator.SetTrigger("Attack" + 2);
            _attackRangeProjectile.Activate(_facingDirection);
        } else if (Input.GetMouseButtonDown(2)) {
            _animator.SetTrigger("Attack" + 3);
            _attackRangeHitScan.Activate(_facingDirection);
        } else if (Input.GetKeyDown(KeyCode.Mouse1)) {
            _animator.SetTrigger("Attack" + 1);
            _attack.Activate(_facingDirection);
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
            _facingDirection = Vector2.left;

        } else if (_horizontal > 0) {
            _renderer.flipX = false;
            _facingDirection = Vector2.right;
        }

        if (oldValue != _renderer.flipX && Fliped != null) {
            Fliped.Invoke(_renderer.flipX);
        }
    }
}
