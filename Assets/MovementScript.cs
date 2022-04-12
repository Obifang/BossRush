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
    private float _horizontal;
    private float _vertical;
    private SpriteRenderer _renderer;
    public delegate void Action(bool value);
    public event Action Fliped;
    private Movement_Dash _dash;
    // Start is called before the first frame update
    void Start()
    {
        _jumping = false;
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _dash = GetComponent<Movement_Dash>();
    }

    private void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        FlipSprite();

        //Falling
        if (_rb.velocity.y < 0.0f) {
            _jumping = false;
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && _grounded) {
            _rb.AddForce(Vector2.up * JumpSpeed, ForceMode2D.Impulse);
            _jumping = true;
        }

        //LR movement
        var dir = MovementSpeed * _horizontal;
        _rb.velocity = new Vector2(dir, _rb.velocity.y);

        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _dash.Dash(_horizontal, _rb);
        }

        IsGrounded();
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

    void FlipSprite()
    {
        if (_horizontal == 0)
        {
            return;
        }

        bool oldValue = _renderer.flipX;

        if (_horizontal < 0) {
            _renderer.flipX = true;

        }
        else if (_horizontal > 0) {
            _renderer.flipX = false;
        }

        if (oldValue != _renderer.flipX)
        {
            Fliped.Invoke(_renderer.flipX);
        }
        
    }
}
