using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IFlippable, IController
{
    enum MovementState
    {
        Idle,
        Moving,
        Dashing,
        PerformingAction
    }

    public MovementScript _movementForce;
    public MovementForceBased _movement;
    private Movement_Dash _dash;

    private Rigidbody2D _rb;
    private SpriteRenderer _renderer;
    private Vector2 _facingDirection = Vector2.right;
    private Animator _animator;
    private ActionHandler _actionHandler;

    private float _horizontal;
    private float _vertical;
    private MovementState _movementState = MovementState.Idle;

    public event IFlippable.Action Fliped;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _dash = GetComponent<Movement_Dash>();
        _animator = GetComponent<Animator>();
        _actionHandler = GetComponent<ActionHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();

        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        _movement.Move(_horizontal, _vertical);
        /*switch (_movementState){
            case MovementState.Idle:
                if (_horizontal != 0 || _vertical != 0) {
                    _movementState = MovementState.Moving;
                }
                _movement.Move(0, 0);
                break;
            case MovementState.Moving:
                if (_horizontal == 0 && _vertical == 0) {
                    _movementState = MovementState.Idle;
                }
                _movement.Move(_horizontal, _vertical);
                break;
            case MovementState.Dashing:
                if (!_dash.IsActive) {
                    _movementState = MovementState.Idle;
                }
                break;
            case MovementState.PerformingAction:
                if (!_actionHandler.IsActive) {
                    _movementState = MovementState.Idle;
                }
                if (_movement.Grounded && !_movement.Jumping) {
                    _movement.StopMoving();
                }
                break;
        }*/

        //Jump
        if (Input.GetKeyDown(KeyCode.Space)) {
            _movement.Jump();
        }

        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            _movementState = MovementState.Dashing;
            _actionHandler.ActivateAction(_facingDirection, 10);
            //_animator.SetTrigger("Roll");
        }

        if (_actionHandler.IsActive) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            _animator.SetTrigger("Attack" + 2);
            _movementState = MovementState.PerformingAction;
            _actionHandler.ActivateAction(_facingDirection, 1);
        } else if (Input.GetMouseButtonDown(2)) {
            _animator.SetTrigger("Attack" + 3);
            _movementState = MovementState.PerformingAction;
            _actionHandler.ActivateAction(_facingDirection, 2);
        } else if (Input.GetKeyDown(KeyCode.Mouse1)) {
            _animator.SetTrigger("Attack" + 1);
            _movementState = MovementState.PerformingAction;
            _actionHandler.ActivateAction(_facingDirection, 0);
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

    public void SetActive(bool value)
    {
        this.enabled = value;
        _actionHandler.CurrentAction.Deactivate(Vector2.zero);
    }
}
