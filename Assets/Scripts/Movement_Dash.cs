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
    public int FramesImmortalFor = 0;
    public int FramesBeforeImmortalStart = 0;
    public float StaminaUsage = 0;
    public int GetID { get => ID; }
    public string GetName { get => Name; }
    public bool IsActive { get => _isDashing; }

    private float _currentCooldown;
    private Rigidbody2D _rb;
    private Vector2 _dir;
    private Animator _animator;
    private bool _isDashing;
    private int _immortalFrameCounter;
    private int _preImmortalFrameCounter;
    private IHasState<MovementState> _hasState;
    private Health _health;
    private Controller_Combat _combatController;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _combatController = GetComponent<Controller_Combat>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isDashing) {
            return;
        }

        _preImmortalFrameCounter--;
        if (_preImmortalFrameCounter <= 0) {
            _health.Immortal = true;
            _immortalFrameCounter--;
            if (_immortalFrameCounter <= 0) {
                _health.Immortal = false;
            }
        }

        if (_currentCooldown > 0f) {
            _currentCooldown -= Time.deltaTime;
        } else if (_currentCooldown <= 0f){
            _currentCooldown = 0f;
            _rb.velocity = Vector2.zero;
            _isDashing = false;
            ShouldIgnoreLayers(false);
        }
    }

    private void FixedUpdate()
    {
        if (_isDashing) {
            _rb.velocity =  new Vector2(_dir.x * DashSpeed, _rb.velocity.y);
        }
    }

    public void Dash(float horizontalValue)
    {
        ShouldIgnoreLayers(true);

        _isDashing = true;
        _currentCooldown = DashCooldown;
        _animator.SetTrigger(AnimationName);

        if (DashUI.instance != null) {
            DashUI.instance.Dashed(DashCooldown);
        }
        
        if (FramesImmortalFor > 0) {
            _immortalFrameCounter = FramesImmortalFor;
            _preImmortalFrameCounter = FramesBeforeImmortalStart;
        }

        if (horizontalValue > 1 || horizontalValue < -1) {
            horizontalValue = horizontalValue - transform.position.x;
        }

        _dir = new Vector2(horizontalValue, 0).normalized;
    }
    
    /// <summary>
    /// Used to allow for rolling through enemies.
    /// </summary>
    /// <param name="value"></param>
    public void ShouldIgnoreLayers(bool value)
    {
        Physics2D.IgnoreLayerCollision(3, 9, value);
        Physics2D.IgnoreLayerCollision(8, 9, value);
        Physics2D.IgnoreLayerCollision(8, 10, value);
    }

    public void Activate(Vector2 direction)
    {
        var success = _combatController.UseStamina(StaminaUsage);

        if (!success) {
            Deactivate(Vector2.zero);
            return;
        }

        if (_isDashing || _currentCooldown > 0f) {
            return;
        }

        Dash(direction.x);
    }

    public void Deactivate(Vector2 direction)
    {
        _isDashing = false;
        _currentCooldown = 0;
        _immortalFrameCounter = 0;
        _preImmortalFrameCounter = 0;
        _health.Immortal = false;
        ShouldIgnoreLayers(false);
        StopAllCoroutines();
    }

    public bool CanActivate(Vector2 direction)
    {
        return true;
    }
}
