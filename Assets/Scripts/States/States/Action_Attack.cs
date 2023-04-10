using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Actions/Attack")]
public class Action_Attack : SMAction
{
    public string AssociatedAnimationName;
    
    public float AttackRange;
    public LayerMask HitableLayers;
    public float Damage = 1.0f;
    public float ApplyDamageAfterTime = 0f;
    public float StaminaUsage = 2;
    public float StaminaReduction = 5;
    public float AttackSpeed = 1.0f;
    public float Cooldown = 1.5f;

    private Animator _animator;
    private Transform AttackPoint;
    private Controller_Movement _movement;
    private bool _isActive = false;
    public override void Execute(StateMachine stateMachine)
    {
        if (_isActive) {
            return;
        }

        if (_animator == null) {
            _animator = stateMachine.GetComponent<Animator>();
        }

        if (AttackPoint == null) {
            AttackPoint = stateMachine.transform.Find("AttackPoint").transform;
        }

        if (_movement == null) {
            _movement = stateMachine.GetComponent<Controller_Movement>();
        }

        stateMachine.StartCoroutine(Use(stateMachine));
        stateMachine.StartCoroutine(StartCooldown(Cooldown));
    }

    private IEnumerator Use(StateMachine stateMachine)
    {
        _movement.StopMoving();
        if (AssociatedAnimationName != "") {
            _animator.SetTrigger(AssociatedAnimationName);
            _animator.speed = 1 / (1 / AttackSpeed);
        }
        yield return new WaitForSeconds(ApplyDamageAfterTime / AttackSpeed);
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange, HitableLayers);

        foreach (Collider2D hitObject in hitObjects) {
            if (hitObject.TryGetComponent<Controller_Combat>(out Controller_Combat combat)) {
                combat.ApplyDamage(Damage, StaminaReduction, stateMachine.transform);
            }
        }
    }

    public override void OnActionStart(StateMachine stateMachine)
    {
        _isActive = false;
    }

    private IEnumerator StartCooldown(float value)
    {
        _isActive = true;
        yield return new WaitForSecondsRealtime(value / AttackSpeed);
        _isActive = false;
        _animator.speed = 1;
    }
}
