using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Dash : MonoBehaviour, IActionable
{
    public int ID;
    public string Name;
    public string AnimationName;
    public float DashSpeed = 25.0f;
    public float DashCooldown = 1.0f;

    public int GetID { get => ID; }
    public string GetName { get => Name; }
    public bool IsActive { get => _isDashing; }

    private float _currentCooldown;
    private Rigidbody2D _rb;
    private Vector2 _dir;
    private Animator _animator;
    private bool _isDashing;
    private IHasState<MovementState> _hasState;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isDashing) {
            return;
        }

        if (_currentCooldown > 0f) {
            _currentCooldown -= Time.deltaTime;
        } else if (_currentCooldown <= 0f){
            _currentCooldown = 0f;
            _rb.velocity = Vector2.zero;
            _isDashing = false;
        }
    }

    private void FixedUpdate()
    {
        if (_isDashing) {
            _rb.velocity =  new Vector2(_dir.x * DashSpeed, _rb.velocity.y);
        }
    }

    public void Dash(float horizontalValue, Rigidbody2D rb)
    {
        if (_isDashing || _currentCooldown > 0f) {
            return;
        }
        _isDashing = true;
        _currentCooldown = DashCooldown;
        _animator.SetTrigger(AnimationName);
        if (DashUI.instance != null) {
            DashUI.instance.Dashed(DashCooldown);
        }
        
        _rb = rb;
        _dir = new Vector2(horizontalValue, 0).normalized;
    }

    public void Activate(Vector2 direction)
    {
        Dash(direction.x, _rb);
    }

    public void Deactivate(Vector2 direction)
    {
        _isDashing = false;
        _currentCooldown = 0;
        StopAllCoroutines();
    }

    public bool CanActivate(Vector2 direction)
    {
        return true;
    }
}
