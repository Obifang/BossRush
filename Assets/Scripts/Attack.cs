using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IActionable
{
    public string AssociatedAnimationName;
    public LayerMask HitableLayers;
    public Transform AttackPoint;
    public float AttackRange;
    public SpriteRenderer AttackerRenderer;
    public float Cooldown = 1f;
    public float Damage = 1.0f;
    public float ApplyDamageAfterTime = 0f;
    public float StaminaUsage = 2;
    public float StaminaReduction = 5;
    private IFlippable Flippable;

    public int ID;
    public string Name;

    public int GetID { get => ID;}

    public string GetName { get => Name;}

    public bool IsActive { get; private set; }
    private Animator _animator;
    private Stamina _stamina;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _stamina = GetComponent<Stamina>();
        Flippable = GetComponent<IFlippable>();
        Flippable.Fliped += FlipAttackPoint;
    }

    void FlipAttackPoint(bool value)
    {
        var dir = AttackPoint.localPosition.x * transform.localScale.normalized.x;
        if ((value && dir > 0) || (!value && dir < 0)) {
            AttackPoint.localPosition = new Vector2(AttackPoint.localPosition.x * -1, AttackPoint.localPosition.y);
        }
    }

    public void Activate(Vector2 direction)
    {
        if (IsActive) {
            return;
        }

        StartCoroutine(StartCooldown(Cooldown));
        StartCoroutine(Use());
    }

    public void Deactivate(Vector2 direction)
    {
        StopAllCoroutines();
        IsActive = false;
    }

    private IEnumerator Use()
    {
        if (_stamina != null) {
            if (_stamina.GetCurrentStamina - StaminaUsage <= 0) {
                yield break;
            }
            _stamina.ReduceStamina(StaminaUsage);
        }

        if (AssociatedAnimationName != "") {
            _animator.SetTrigger(AssociatedAnimationName);
        }
        yield return new WaitForSeconds(ApplyDamageAfterTime);
        Collider2D [] hitObjects = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange, HitableLayers);

        foreach(Collider2D hitObject in hitObjects) {
            if (hitObject.TryGetComponent<Controller_Combat>(out Controller_Combat combat)) {
                combat.ApplyDamage(Damage, StaminaReduction);
            }
            /* if (hitObject.TryGetComponent<Health>(out Health health)) {
                 health.CalculateHealthChange(Damage);
             }

             if (hitObject.TryGetComponent<Stamina>(out Stamina stamina)) {
                 stamina.ReduceStamina(1.0f);
             }*/
        }
    }

    private IEnumerator StartCooldown(float value)
    {
        IsActive = true;
        yield return new WaitForSecondsRealtime(value);
        IsActive = false;
    }

    private void OnDrawGizmos()
    {
        if (AttackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    }

    public bool CanActivate(Vector2 direction)
    {
        return true;
    }
}
