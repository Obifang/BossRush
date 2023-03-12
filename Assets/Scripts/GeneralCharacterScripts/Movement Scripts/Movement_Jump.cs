using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Jump : MonoBehaviour, IActionable
{
    public int ID;
    public string Name;
    public string JumpAnimationName = "Jump";
    public string GroundedAnimationName = "Grounded";
    public float JumpSpeed;

    public int GetID { get => ID; }
    public string GetName { get => Name; }
    public bool IsActive { get; private set; }

    private Rigidbody2D _rb;
    private Animator _animator;
    private Vector2 _dir;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Jump()
    {
        if (IsActive) {
            return;
        }

        _rb.velocity = Vector2.zero;
        _rb.AddForce(_dir * JumpSpeed, ForceMode2D.Impulse);

        if (_animator != null) {
            _animator.SetTrigger(JumpAnimationName);
            _animator.SetBool(GroundedAnimationName, false);
        }
    }

    public void Activate(Vector2 direction)
    {
        _dir = new Vector2(direction.x, 1);
        Jump();
    }

    public void Deactivate(Vector2 direction)
    {
        StopAllCoroutines();
        IsActive = false;
    }

    public bool CanActivate(Vector2 direction)
    {
        return true;
    }
}
