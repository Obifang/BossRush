using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_PlungeAttack : MonoBehaviour, IActionable
{
    public int ID;
    public string Name;

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
    private IHasState<MovementState> _movementState;
    private Collider2D _collider;
    public int GetID { get => ID; }

    public string GetName { get => Name; }

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
        _movementState = GetComponent<IHasState<MovementState>>();
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (IsActive) {
            var cState = _movementState.GetCurrentState();
            if (cState == MovementState.Falling) {
                if (CheckForEnemies()) {
                    Attack();
                    IsActive = false;
                    SetAnimation(AssociatedAnimationName, false);
                    SetAnimation("PlungeAttack", true);
                }
            } else {
                IsActive = false;
                Debug.Log("Plunge");
                SetAnimation(AssociatedAnimationName, false);
            }
        }
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

        Begin();
    }

    public void Deactivate(Vector2 direction)
    {
        StopAllCoroutines();
        IsActive = false;
    }

    private void Begin()
    {
        if (_stamina != null) {
            if (_stamina.GetCurrentStamina - StaminaUsage <= 0) {
                return;
            }
            _stamina.ReduceStamina(StaminaUsage);
        }

        SetAnimation(AssociatedAnimationName, true);

        IsActive = true;
    }

    private void Attack()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange, HitableLayers);

        foreach (Collider2D hitObject in hitObjects) {
            if (hitObject.TryGetComponent<Controller_Combat>(out Controller_Combat combat)) {
                combat.ApplyDamage(Damage, StaminaReduction);
            }
        }
    }

    void SetAnimation(string name, bool value)
    {
        if (AssociatedAnimationName != "" &&  _animator != null) {
            _animator.SetBool(name, value);
        }
    }

    private bool CheckForEnemies()
    {
        var halfWidth = _collider.bounds.size.x * 0.5f;
        var center = _collider.bounds.center;
        var left = new Vector2(center.x - halfWidth, center.y);
        var right = new Vector2(center.x + halfWidth, center.y);
        var leftHit = Physics2D.Raycast(left, Vector2.down, AttackRange, HitableLayers);
        var rightHit = Physics2D.Raycast(right, Vector2.down, AttackRange, HitableLayers);
        var centerHit = Physics2D.Raycast(center, Vector2.down, AttackRange, HitableLayers);

        Debug.DrawLine(left, new Vector3(left.x, left.y - AttackRange), Color.red);
        Debug.DrawLine(center, new Vector3(center.x, center.y - AttackRange), Color.red);
        Debug.DrawLine(right, new Vector3(right.x, right.y - AttackRange), Color.red);

        return (leftHit || rightHit || centerHit);
    }

    private IEnumerator StartCooldown(float value)
    {
        IsActive = true;
        yield return new WaitForSecondsRealtime(value);
        IsActive = false;
    }

    private void OnDrawGizmos()
    {
        if (AttackPoint == null) {
            return;
        }

        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    }
}
