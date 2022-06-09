using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float MovementSpeed = 5.0f;
    public float JumpSpeed = 8.0f;
    public float GroundCheckDistance = 2.0f;

    private bool _jumping;
    private Rigidbody2D _rb;
    private bool _grounded = false;
    private Collider2D _collider;
    private float _currentDashTime = 0f;
    private float _dashCooldown;
    private Movement_Dash _dash;
    private Animator _animator;
    public bool Grounded { get => _grounded;}
    public bool Jumping { get => _jumping;}

    // Start is called before the first frame update
    void Start()
    {
        _jumping = false;
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //Falling
        if (_rb.velocity.y < 0.0f) {
            if (_animator != null)
                _animator.SetFloat("AirSpeedY", _rb.velocity.y);
           _jumping = false;
        }

        IsGrounded();

        if (_grounded && !_jumping) {
            if (_animator != null)
                _animator.SetBool("Grounded", true);
        }
    }

    public void Move(float horizontal, float vertical)
    {
        var dir = MovementSpeed * horizontal;
        _rb.velocity = new Vector2(dir, _rb.velocity.y);
        if (horizontal != 0) {
            if (_animator != null)
                _animator.SetInteger("AnimState", 1);
        } else {
            if (_animator != null)
                _animator.SetInteger("AnimState", 0);
        }
    }

    public void StopMoving()
    {
        StopMovingHorizontally();
        StopMovingVertically();
    }

    public void StopMovingHorizontally()
    {
        _rb.velocity = new Vector2(0, _rb.velocity.y);
        if (_animator != null)
            _animator.SetInteger("AnimState", 0);

    }

    public void StopMovingVertically()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, 0);
        if (_animator != null)
            _animator.SetInteger("AnimState", 0);

    }

    public void Jump()
    {
        if (_grounded && !_jumping) {
            _rb.AddForce(Vector2.up * JumpSpeed, ForceMode2D.Impulse);
            _jumping = true;
            if (_animator != null) {
                _animator.SetTrigger("Jump");
                _animator.SetBool("Grounded", false);
            }
        }
    }

    public void KnockBack(Vector2 dir, float force)
    {
        StopMoving();
        _rb.AddForce(dir * force, ForceMode2D.Impulse);
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
}
