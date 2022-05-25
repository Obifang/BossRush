using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Dash : MonoBehaviour, IActionable
{
    public string AnimationName;
    public float DashSpeed = 25.0f;
    public float DashCooldown = 1.0f;
    public bool IsDashing {
        get; private set;
    }

    public int ID;
    public string Name;

    public int GetID { get => ID; }

    public string GetName { get => Name; }

    public bool IsActive { get => IsDashing; }

    private float _currentCooldown;
    private Rigidbody2D _rb;
    private Vector2 _dir;
    private Animator _animator;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentCooldown > 0f) {
            _currentCooldown -= Time.deltaTime;
            _rb.velocity = _dir * DashSpeed;
        } else if (_currentCooldown < 0f){
            _currentCooldown = 0f;
            _rb.velocity = Vector2.zero;
            IsDashing = false;
        }
    }

    public void Dash(float horizontalValue, Rigidbody2D rb)
    {
        if (IsDashing || _currentCooldown > 0f) {
            return;
        }
        IsDashing = true;
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
}
