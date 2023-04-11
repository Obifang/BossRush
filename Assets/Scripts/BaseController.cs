using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour, IFlippable
{
    public event IFlippable.Action Fliped;
    protected Vector2 _facingDirection = Vector2.right;
    protected float _horizontal;
    protected float _vertical;
    protected SpriteRenderer _renderer;
    protected Rigidbody2D _rb;
    protected Animator _animator;
    protected ActionHandler _actionHandler;

    public Animator GetAnimator { get => _animator;}
    public Rigidbody2D GetRb { get => _rb;}
    public ActionHandler GetActionHandler { get => _actionHandler; }
    public bool IsFlipable { get;set; }
    public float Horizontal { get => _horizontal; set { _horizontal = value; } }
    public float Vertical { get => _vertical; set { _vertical = value; } }
    public Vector2 FacingDirection { get => _facingDirection;}

    public bool StartingSpriteIsFacingRight = true;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _actionHandler = GetComponent<ActionHandler>();
        IsFlipable = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (IsFlipable) {
            FlipSprite();
        }
    }

    public virtual void FlipSprite()
    {
        if (_horizontal == 0) {
            return;
        }

        bool oldValue = _renderer.flipX;

        if (_horizontal < 0) {
            if (StartingSpriteIsFacingRight) {
                _renderer.flipX = true;
            } else {
                _renderer.flipX = false;
            }
            
            _facingDirection = Vector2.left;

        } else if (_horizontal > 0) {
            if (StartingSpriteIsFacingRight) {
                _renderer.flipX = false;
            } else {
                _renderer.flipX = true;
            }
            _facingDirection = Vector2.right;
        }

        if (oldValue != _renderer.flipX && Fliped != null) {
            Fliped.Invoke(_renderer.flipX);
        }
    }

    public void AddToFlipedEvent(IFlippable.Action e)
    {
        Fliped += e;
    }

    public void RemoveFromFlipedEvent(IFlippable.Action e)
    {
        Fliped -= e;
    }

    public void InvokeFlipedEvent(bool value)
    {
        if (Fliped != null) {
            Fliped.Invoke(value);
        }
    }

    public abstract void SetActive(bool value);
}
