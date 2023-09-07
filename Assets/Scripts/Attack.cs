using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using static UnityEngine.GraphicsBuffer;

public class Attack : MonoBehaviour, IActionable
{
    public string AssociatedAnimationName;
    public string SoundEffectOnActionName;
    public LayerMask HitableLayers;
    public Transform AttackPoint;
    public float AttackRange;
    public SpriteRenderer AttackerRenderer;
    public float Cooldown = 1f;
    public float Damage = 1.0f;
    public float ApplyDamageAfterTime = 0f;
    public float StaminaUsage = 2;
    public float StaminaReduction = 5;
    public float AttackSpeed = 1.0f;
    private IFlippable Flippable;

    public int ID;
    public string Name;

    public int GetID { get => ID;}

    public string GetName { get => Name;}

    public bool IsActive { get; private set; }
    private Animator _animator;
    private Stamina _stamina;
    private BaseController _baseController;
    private Controller_Combat _combatController;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _stamina = GetComponent<Stamina>();
        Flippable = GetComponent<IFlippable>();
        Flippable.Fliped += FlipAttackPoint;
        _baseController = GetComponent<BaseController>();
        _combatController = GetComponent<Controller_Combat>();
        _audioSource = GetComponent<AudioSource>();
    }

    void FlipAttackPoint(bool value)
    {
        var attckXPos = AttackPoint.localPosition.x;

        if ((_baseController.StartingSpriteIsFacingRight && ((value && attckXPos > 0) || (!value && attckXPos < 0))) ||
           (!_baseController.StartingSpriteIsFacingRight && ((value && attckXPos < 0) || (!value && attckXPos > 0)))) {
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
        var success = _combatController.UseStamina(StaminaUsage);

        if (!success) {
            Deactivate(Vector2.zero);
            yield break;
        }

        if (AssociatedAnimationName != "") {
            _animator.SetTrigger(AssociatedAnimationName);
            _animator.speed = 1 / (1 / AttackSpeed);
        }

        

        yield return new WaitForSeconds(ApplyDamageAfterTime / AttackSpeed);
        Collider2D [] hitObjects = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange, HitableLayers);
        Manager_Audio.Instance.PlaySoundEffect(SoundEffectOnActionName);
        foreach (Collider2D hitObject in hitObjects) {
            if (hitObject.TryGetComponent<Controller_Combat>(out Controller_Combat combat)) {
                combat.ApplyDamage(Damage, StaminaReduction, transform);
            }
        }
    }

    private IEnumerator StartCooldown(float value)
    {
        IsActive = true;
        yield return new WaitForSecondsRealtime(value / AttackSpeed);
        IsActive = false;
        _animator.speed = 1;
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
        if (_stamina.GetCurrentStamina < StaminaUsage) {
            return false;
        }

        if (direction.x <= 1 && direction.x >= -1) {
            return true;
        }

        if (Vector2.Distance(AttackPoint.position, direction) <= AttackRange) {
            return true;
        }

        if (Vector2.Distance(transform.position, direction) <= AttackRange) {
            return true;
        }
        return false;
    }
}
