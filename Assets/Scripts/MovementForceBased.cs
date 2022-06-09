using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementForceBased : MonoBehaviour, IHasState<MovementState>
{
    public bool Grounded { get => _grounded; }
    public bool Jumping { get => _jumping; }

    public float MovementSpeed = 5.0f;
    public float JumpSpeed = 8.0f;
    public float GroundCheckDistance = 2.0f;
    public float MaxSpeed;

    private bool _jumping;
    private bool _grounded = false;
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

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Falling
        if (!_grounded && IsFalling()) {
            if (_animator != null)
                _animator.SetFloat("AirSpeedY", _rb.velocity.y);
            _movementState = MovementState.Falling;
        }

        IsGrounded();

        if (_grounded && _movementState != MovementState.Jumping) {
            if (_animator != null)
                _animator.SetBool("Grounded", true);
        }
    }

    public void UpdateState(MovementState mS)
    {
        _previousState = _movementState;
        _movementState = mS;
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
                break;
            case MovementState.Knockback:
                KnockBack(_knockbackDir, _knockbackForce);
                break;
            case MovementState.Falling:
                if (!IsFalling() && _grounded) {
                    UpdateState(MovementState.Idle);
                    StopMoving();
                }
                break;
            case MovementState.Dashing:
                break;
        }
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
        //_rb.AddForce(new Vector2(dir, 0));
        //_rb.velocity = Vector2.ClampMagnitude(_rb.velocity, JumpSpeed);
    }

    public void Jump()
    {
        if (_movementState == MovementState.Jumping) {
            return;
        }

        if (_grounded) {
            UpdateState(MovementState.Jumping);
            _rb.AddForce(Vector2.up * JumpSpeed, ForceMode2D.Impulse);
            //_jumping = true;
            if (_animator != null) {
                _animator.SetTrigger("Jump");
                _animator.SetBool("Grounded", false);
            }
        }
    }

    private void Idle()
    {
        if (_horizontal != 0 || _vertical != 0) {
            UpdateState(MovementState.Moving);
        }
    }

    private void Moving()
    {
        var dir = MovementSpeed * _horizontal;
        _rb.velocity = new Vector2(dir, _rb.velocity.y);
        //_rb.velocity = _rb.velocity + new Vector2(dir, 0);
        //_rb.AddForce(new Vector2(dir, 0));
        //_rb.velocity = Vector2.ClampMagnitude(_rb.velocity, MaxSpeed);// new Vector2(Mathf.Clamp(_rb.velocity.x, -MaxSpeed, MaxSpeed), _rb.velocity.y);
        if (_horizontal != 0) {
            if (_animator != null)
                _animator.SetInteger("AnimState", 1);
        } else if (_vertical == 0){
            if (_animator != null) {
                _animator.SetInteger("AnimState", 0);
                UpdateState(MovementState.Idle);
                _rb.velocity = Vector2.zero;
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
    void IsGrounded()
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
