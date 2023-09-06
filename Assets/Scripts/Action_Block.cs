using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class Action_Block : MonoBehaviour, IActionable
{
    public int ID;
    public string Name;
    public string AssociatedAnimationName;
    public string BlockedAttackAnimationName;
    public string OnBlockSoundEffect;

    public LayerMask HitableLayers;
    public float DefenseRange;
    public float DamageBlocked;
    public float StaminaReducedByOnBlock = 2.0f;
    public float Duration = 1f;
    public int FrameCheckCount = 2;
    public int FramesInActiveWhenTakingDamage = 5;
    public int FramesInActiveWhenStaggered = 15;

    public int GetID { get => ID; }

    public string GetName { get => Name; }

    public bool IsActive { get; private set; }
    private Animator _animator;
    private Health _health;
    private BaseController _controller;
    Controller_Combat _combatController;
    private int _damagedFrameCounter = 0;
    private Stamina _stamina;
    private BlockState _currentState;
    private float _damageAdjustedValue;

    private enum BlockState
    {
        Starting,
        Blocking,
        Staggered,
        Damaged
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _combatController = GetComponent<Controller_Combat>();
        _controller = GetComponent<BaseController>();
        _combatController.DamageBeingApplied += OnHit;
        _stamina = GetComponent<Stamina>();
        _stamina.AddToOutOfStaminaListener(OutOfStamina);
        _combatController.StaggeredEnded += StaggeredEnded;
    }

    private void Update()
    {
        if (!IsActive) {
            return;
        }

        HandleStates();
    }

    private void HandleStates()
    {
        switch (_currentState) {
            case BlockState.Starting:
                break;
            case BlockState.Blocking:
                break;
            case BlockState.Staggered:
                break;
            case BlockState.Damaged:
                if (_damagedFrameCounter > 0) {
                    _damagedFrameCounter--;
                    if (_damagedFrameCounter == 0) {
                        _animator.SetBool(AssociatedAnimationName, true);
                        _currentState = BlockState.Blocking;
                    }  
                }
                break;
        }
    }

    private void OutOfStamina()
    {
        if (!IsActive || _currentState != BlockState.Blocking) {
            return;
        }
        _animator.SetBool(AssociatedAnimationName, false);
        AdjustDefense(-DamageBlocked);
        _currentState = BlockState.Staggered;
        _damagedFrameCounter = FramesInActiveWhenStaggered;
    }

    private void StaggeredEnded()
    {
        if (!IsActive || _currentState != BlockState.Staggered) {
            return;
        }
        _currentState = BlockState.Blocking;
        Deactivate(Vector2.zero);
        /*_animator.SetBool(AssociatedAnimationName, true);
        _currentState = BlockState.Blocking;
        AdjustDefense(DamageBlocked);*/
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
            Manager_Audio.Instance.PlaySoundEffect(OnBlockSoundEffect);
            _stamina.ReduceStamina(StaminaReducedByOnBlock);
            _stamina.IncreaseStamina(1);
        } else if (facingDir < 0) {
            _animator.SetBool(AssociatedAnimationName, false);
            _damagedFrameCounter = FramesInActiveWhenTakingDamage;
            _health.CalculateHealthChange(damage + DamageBlocked);
        }
    }

    public void Activate(Vector2 direction)
    {
        AdjustDefense(DamageBlocked);
        _currentState = BlockState.Blocking;
        if (_animator != null) {
            _animator.SetBool(AssociatedAnimationName, true);
        }

        IsActive = true;
    }

    public void Deactivate(Vector2 direction)
    {
        StopAllCoroutines();
        IsActive = false;
        if (_currentState == BlockState.Staggered) {
            return;
        }
        _damagedFrameCounter = 0;
        if (_damageAdjustedValue > 0) {
            AdjustDefense(-DamageBlocked);
            _damageAdjustedValue = 0;
        }
        
        if (_animator != null) {
            _animator.SetBool(AssociatedAnimationName, false);
            _animator.SetBool("BlockHit", false);
        }
    }

    void AdjustDefense(float value)
    {
        _damageAdjustedValue += value;
        _combatController.AdjustDefense(value);
    }

    private void OnDrawGizmos()
    {

    }

    public bool CanActivate(Vector2 direction)
    {
        if (_combatController.Staggered) {
            return false;
        }
        return true;
    }
}
