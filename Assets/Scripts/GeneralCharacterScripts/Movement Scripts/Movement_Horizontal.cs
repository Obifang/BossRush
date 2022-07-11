using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Horizontal : MonoBehaviour, IActionable
{
    public int ID;
    public string Name;
    public string WalkingAnimationName = "Walk";
    public float MovementSpeed;

    public int GetID { get => ID; }
    public string GetName { get => Name; }
    public bool IsActive { get; private set; }

    private Rigidbody2D _rb;
    private Animator _animator;
    private IHasState<MovementState> _hasState;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Move(Vector2 direction)
    {
        var dir = MovementSpeed * direction.x;
        _rb.velocity = new Vector2(dir, _rb.velocity.y);
        //_rb.velocity = _rb.velocity + new Vector2(dir, 0);
        //_rb.AddForce(new Vector2(dir, 0));
        //_rb.velocity = Vector2.ClampMagnitude(_rb.velocity, MaxSpeed);// new Vector2(Mathf.Clamp(_rb.velocity.x, -MaxSpeed, MaxSpeed), _rb.velocity.y);
        if (direction.x != 0) {
            if (_animator != null)
                _animator.SetInteger("AnimState", 1);
        } else if (direction.y == 0) {
            if (_animator != null) {
                _animator.SetInteger("AnimState", 0);
                _hasState.UpdateState(MovementState.Idle);
                _rb.velocity = Vector2.zero;
            }
        }
    }

    public void Activate(Vector2 direction)
    {
        IsActive = true;
        Move(direction);
    }

    public void Deactivate(Vector2 direction)
    {
        StopAllCoroutines();
        IsActive = false;
    }
}
