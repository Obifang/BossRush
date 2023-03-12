using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Controller_Movement : MonoBehaviour, IHasState<MovementState>
{
    private enum SlidingSide
    {
        Left, Right
    }

    public bool Grounded { get => _grounded; }
    public bool Sliding { get => _sliding; }

    public float MovementSpeed = 5.0f;
    [Range(0f, 1f)]
    public float StrafeSpeedModifier;
    public float GroundCheckDistance = 2.0f;
    public string JumpActionName = "Jump";
    public string DashActionName = "Dash";

    private bool _grounded = false;
    private bool _sliding = false;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Collider2D _collider;
    private MovementState _movementState = MovementState.Idle;
    private float _horizontal;
    private float _vertical;
    private float _timer;
    private Vector2 _knockbackDir;
    private float _knockbackForce;
    private MovementState _previousState;
    private ActionHandler _actionHandler;
    private BaseController _controller;
    private SlidingSide _directionWhenWallSliding;
    private SlidingSide _slidingDirection;

    private Sensor _groundSensor;
    private Sensor _wallSensorR1;
    private Sensor _wallSensorR2;
    private Sensor _wallSensorL1;
    private Sensor _wallSensorL2;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();

        var actionables = GetComponents<IActionable>();
        _actionHandler = GetComponent<ActionHandler>();

        _groundSensor = transform.Find("GroundSensor").GetComponent<Sensor>();
        _wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor>();
        _wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor>();
        _wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor>();
        _wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor>();
    }

    // Update is called once per frame
    void Update()
    {
        //Falling
        if (!_grounded && IsFalling() && _movementState != MovementState.WallSlide) {
            UpdateState(MovementState.Falling);
        }

        IsGrounded();
        IsSliding();

        if (_grounded && _movementState != MovementState.Jumping) {
            if (_animator != null)
                _animator.SetBool("Grounded", true);
        }
    }

    public void UpdateState(MovementState mS)
    {
        if (_movementState == mS)
            return;

        _previousState = _movementState;
        _movementState = mS;
        OnStateExit(_previousState);
        OnStateEnter(mS);
    }

    private void OnStateExit(MovementState ms)
    {
        switch (ms) {
            case MovementState.Locked:
                break;
            case MovementState.Idle:
                break;
            case MovementState.Moving:
                break;
            case MovementState.Jumping:
                break;
            case MovementState.Knockback:
                break;
            case MovementState.Falling:
                break;
            case MovementState.Dashing:
                break;
            case MovementState.PerformingAction:
                StopMoving();
                break;
            case MovementState.WallSlide:
                _animator.SetBool("WallSlide", false);
                break;
            case MovementState.WallJump:
                StopMoving();
                break;
            case MovementState.Strafe:
                break;
        }
    }

    private void OnStateEnter(MovementState ms)
    {
        switch (ms) {
            case MovementState.Locked:
                break;
            case MovementState.Idle:
                _animator.SetInteger("AnimState", 0);
                _rb.velocity = Vector2.zero;
                break;
            case MovementState.Moving:
                _animator.SetInteger("AnimState", 1);
                break;
            case MovementState.Jumping:
                break;
            case MovementState.Knockback:
                break;
            case MovementState.Falling:
                _animator.SetFloat("AirSpeedY", _rb.velocity.y);
                _animator.SetBool("Grounded", false);
                break;
            case MovementState.Dashing:
                break;
            case MovementState.PerformingAction:
                StopMoving();
                break;
            case MovementState.WallSlide:
                _animator.SetBool("WallSlide", true);
                _directionWhenWallSliding = _slidingDirection;
                break;
            case MovementState.WallJump:
                break;
            case MovementState.Strafe:
                break;
        }
    }

    private void HandleMovementStates()
    {
        switch (_movementState) {
            case MovementState.Locked:
                break;
            case MovementState.Idle:
                Idle();
                break;
            case MovementState.Moving:
                Moving();
                break;
            case MovementState.Jumping:
                InAirMoving();
                if (_sliding && _previousState != MovementState.WallSlide) {
                    UpdateState(MovementState.WallSlide);
                }
                break;
            case MovementState.Knockback:
                KnockBack(_knockbackDir, _knockbackForce);
                break;
            case MovementState.Falling:
                if (!IsFalling() && _grounded) {
                    if (_horizontal == 0) {
                        UpdateState(MovementState.Idle);
                        StopMovingHorizontally();
                    }
                    else
                        UpdateState(MovementState.Moving);
                } else {
                    if (_sliding) {
                        UpdateState(MovementState.WallSlide);
                    } else {
                        InAirMoving();
                    }
                }
                break;
            case MovementState.Dashing:
                Dash(_horizontal);
                break;
            case MovementState.PerformingAction:
                IsPerformingAction();
                break;
            case MovementState.WallSlide:
                if (_grounded || !_sliding) {
                    UpdateState(MovementState.Idle);
                }
                break;
            case MovementState.WallJump:
                if (_sliding && _directionWhenWallSliding != _slidingDirection)
                    UpdateState(MovementState.WallSlide);

                if (IsFalling()) 
                    UpdateState(MovementState.Falling);
                break;
            case MovementState.Strafe:
                Strafe();
                break;
        }
    }

    private void IsPerformingAction()
    {
        if (!_actionHandler.IsActive)
            UpdateState(MovementState.Idle);
    }

    private bool IsFalling()
    {
        if (_rb.velocity.y < 0.0f) {
            return true;
        } else {
            return false;
        }
    }

    private void InAirMoving()
    {
        var dir = MovementSpeed * _horizontal;
        _rb.velocity = new Vector2(dir, _rb.velocity.y);
    }

    private void Idle()
    {
        if (_horizontal != 0) {
            UpdateState(MovementState.Moving);
        }
    }
    
    private void Strafe()
    {
        var dir = MovementSpeed * _horizontal;
        _rb.velocity = new Vector2(dir, _rb.velocity.y) * StrafeSpeedModifier;
        if (_horizontal == 0 && _vertical == 0) {
            if (_animator != null) {
                UpdateState(MovementState.Idle);
            }
        }
    }

    private void IsSliding()
    {
        if (_grounded) {
            _sliding = false;
            return;
        }

        var right = (_wallSensorR1.IsColliding() && _wallSensorR2.IsColliding());
        var left = (_wallSensorL1.IsColliding() && _wallSensorL2.IsColliding());

        if (right)
            _slidingDirection = SlidingSide.Right;

        if (left)
            _slidingDirection = SlidingSide.Left;

        _sliding =  left || right;
    }

    public void Jump()
    {
        if (_movementState == MovementState.Jumping || _movementState == MovementState.Falling) {
            return;
        }
        var result = _actionHandler.ActivateActionByName(new Vector2(_horizontal, _vertical), JumpActionName);

        if (result) {
            if (!_sliding)
                UpdateState(MovementState.Jumping);
            else if (_movementState == MovementState.WallSlide)
                UpdateState(MovementState.WallJump);
        }
    }

    public void Dash(float facingDirection)
    {
        if (_movementState != MovementState.Dashing) {
            var result = _actionHandler.ActivateActionByName(new Vector2(facingDirection, _vertical), DashActionName);
            if (result)
                UpdateState(MovementState.Dashing);
        }
        
        if (!_actionHandler.IsActive) {
            UpdateState(MovementState.Idle);
        }
    }

    private void Moving()
    {
        var dir = MovementSpeed * _horizontal;
        _rb.velocity = new Vector2(dir, _rb.velocity.y);
        if (_horizontal == 0 && _vertical == 0) {
            if (_animator != null) {
                UpdateState(MovementState.Idle);
            }
        }
    }

    public void KnockBack(Vector2 dir, float force, float time = 1)
    {
        if (_movementState != MovementState.Knockback) {
            UpdateState(MovementState.Knockback);
            StopMoving();
            _knockbackDir = dir;
            _knockbackForce = force;
            _horizontal = 0;
            _vertical = 0;
            _timer = time;
        }

        //_rb.AddForce(dir * force, ForceMode2D.Impulse);
        _timer -= Time.fixedDeltaTime;

        if (_timer > 0f) {
            _rb.velocity = _knockbackDir * _knockbackForce;
        } else if (_timer <= 0f) {
            _timer = 0f;
            _rb.velocity = Vector2.zero;
            _horizontal = 0;
            _vertical = 0;
            UpdateState(MovementState.Idle);
        }
    }

    public void Move(float horizontal, float vertical)
    {
        _horizontal = horizontal;
        _vertical = vertical;
    }

    private void FixedUpdate()
    {
        HandleMovementStates();
    }

    public void StopMoving()
    {
        StopMovingHorizontally();
        StopMovingVertically();
    }

    public void StopMovingHorizontally()
    {
        _rb.velocity = new Vector2(0, _rb.velocity.y);
        _horizontal = 0;
        if (_animator != null)
            _animator.SetInteger("AnimState", 0);
    }

    public void StopMovingVertically()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, 0);
        _vertical = 0;
        if (_animator != null)
            _animator.SetInteger("AnimState", 0);
    }

    //Grounded Check
    public void IsGrounded()
    {
        var groundLayer = 1 << LayerMask.NameToLayer("Ground");
        var halfWidth = _collider.bounds.size.x * 0.5f;
        var center = _collider.bounds.center;
        var left = new Vector2(center.x - halfWidth, center.y);
        var right = new Vector2(center.x + halfWidth, center.y);
        var leftHit = Physics2D.Raycast(left, Vector2.down, GroundCheckDistance, groundLayer);
        var rightHit = Physics2D.Raycast(right, Vector2.down, GroundCheckDistance, groundLayer);
        var centerHit = Physics2D.Raycast(center, Vector2.down, GroundCheckDistance, groundLayer);

        Debug.DrawLine(left, new Vector3(left.x, left.y - GroundCheckDistance), Color.red);
        Debug.DrawLine(center, new Vector3(center.x, center.y - GroundCheckDistance), Color.red);
        Debug.DrawLine(right, new Vector3(right.x, right.y - GroundCheckDistance), Color.red);

        _grounded = (leftHit || rightHit || centerHit);
    }

    public MovementState GetCurrentState()
    {
        return _movementState;
    }

    public MovementState GetPreviousState()
    {
        return _previousState;
    }
}
