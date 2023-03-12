using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Block : MonoBehaviour, IActionable
{
    public int ID;
    public string Name;
    public string AssociatedAnimationName;
    public string BlockedAttackAnimationName;

    public LayerMask HitableLayers;
    public float DefenseRange;
    public float DamageBlocked;
    public float Duration = 1f;
    public int FrameCheckCount = 2;
    public int FramesInActiveWhenTakingDamage = 5;

    public int GetID { get => ID; }

    public string GetName { get => Name; }

    public bool IsActive { get; private set; }
    private Animator _animator;
    private Health _health;
    private BaseController _controller;
    Controller_Combat _combatController;
    private int _frameCounter = 0;
    private int _damagedFrameCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _combatController = GetComponent<Controller_Combat>();
        _controller = GetComponent<BaseController>();
        _combatController.DamageBeingApplied += OnHit;
        //_health.ChangeInHealth += ChangeInHealthDetected;
        //_health.GetReductions += ChangeInHealthDetected;
    }

    private void Update()
    {
        if (IsActive) {
            if(_damagedFrameCounter > 0) {
                _damagedFrameCounter--;
                if (_damagedFrameCounter == 0)
                    _animator.SetBool(AssociatedAnimationName, true);
            }

            _frameCounter++;
            if (_frameCounter >= FrameCheckCount) {
                IsActive = false;
                if (_animator != null) {
                    _animator.SetBool(AssociatedAnimationName, false);
                    _animator.SetBool("BlockHit", false);
                    _combatController.AdjustDefense(-DamageBlocked);
                }
            }
        }
    }

    float ChangeInHealthDetected(float damage)
    {
        if (!IsActive)
            return 0;
        _health.AddDamageReduction(1f);
        
        _animator.SetBool(BlockedAttackAnimationName, true);
        _animator.SetBool("BlockHit", true);
        return DamageBlocked;
    }

    void OnHit(float damage, float StaminaReduction, Transform damageSource)
    {
        if (!IsActive)
            return;
        var facingDir = 1f;
        if (damageSource != null) {
            facingDir = Vector2.Dot(damageSource.transform.position - transform.position, _controller.FacingDirection);
        }

        if (facingDir > 0 && damage == 0) {
            _animator.SetBool(BlockedAttackAnimationName, true);
            _animator.SetBool("BlockHit", true);
        } else if (facingDir < 0){
            _animator.SetBool(AssociatedAnimationName, false);
            _damagedFrameCounter = FramesInActiveWhenTakingDamage;
            _health.CalculateHealthChange(damage + DamageBlocked);
        }
    }

    public void Activate(Vector2 direction)
    {
        if (!IsActive) {
            _combatController.AdjustDefense(DamageBlocked);
            if (_animator != null) {
                _animator.SetBool(AssociatedAnimationName, true);
            }
        }

        IsActive = true;
        _frameCounter = 0;
    }

    public void Deactivate(Vector2 direction)
    {
        if (_frameCounter <= FrameCheckCount && IsActive)
            return;
        StopAllCoroutines();
        IsActive = false;
        if (_animator != null) {
            _animator.SetBool(AssociatedAnimationName, false);
        }
    }

    private void OnDrawGizmos()
    {

    }

    public bool CanActivate(Vector2 direction)
    {
        return true;
    }
}
