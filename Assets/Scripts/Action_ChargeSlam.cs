using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_ChargeSlam : MonoBehaviour, IActionable
{
    public int ID;
    public string Name;
    public float ChargeDuration = 4.0f;
    public float Damage = 3.0f;
    public float StaminaUsage = 2;
    public float StaminaReduction = 5;
    public string ChargeAnimation;
    public string SlamAnimation;
    public float CooldownDuration;
    public Transform AttackPoint;
    public float AttackRange;
    public LayerMask HitableLayers;
    public ParticleSystem ChargeParticles;
    public ParticleSystem SlamParticles;
    public string ChargeSoundEffect;
    public string SlamSoundEffect;

    public int GetID { get => ID;}
    public string GetName { get => Name;}
    public bool IsActive { get; private set; }

    private Animator _animator;
    private float _timer;
    private ParticleSystem _particles;
    private BaseController _baseController;
    private IFlippable Flippable;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _particles = GetComponent<ParticleSystem>();
        _baseController = GetComponent<BaseController>();
        Flippable = GetComponent<IFlippable>();
        Flippable.Fliped += FlipAttackPointAndParticles;
    }

    void FlipAttackPointAndParticles(bool value)
    {
        var chargePosX = ChargeParticles.transform.localPosition.x;

        if ((_baseController.StartingSpriteIsFacingRight && ((value && chargePosX > 0) || (!value && chargePosX < 0))) ||
           (!_baseController.StartingSpriteIsFacingRight && ((value && chargePosX < 0) || (!value && chargePosX > 0)))) {
            var newPos = new Vector2(ChargeParticles.transform.localPosition.x * -1, ChargeParticles.transform.localPosition.y);
            ChargeParticles.transform.localPosition = newPos;
            SlamParticles.transform.localPosition = newPos;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate(Vector2 direction)
    {
        if (IsActive) {
            return;
        }
        StartCoroutine(Use());
        StartCoroutine(Cooldown(CooldownDuration));
    }

    private IEnumerator Use()
    {
        _animator.SetBool(ChargeAnimation, true);
        _animator.SetTrigger(SlamAnimation);
        if (ChargeParticles != null) {
            ChargeParticles.Play();
        }
        Manager_Audio.Instance.PlaySoundEffect(ChargeSoundEffect);
        yield return new WaitForSeconds(ChargeDuration);
        
        if (ChargeParticles != null) {
            ChargeParticles.Stop();
        }

        if (SlamParticles != null) {
            SlamParticles.Play();
        }
        _animator.SetBool(ChargeAnimation, false);
        Manager_Audio.Instance.PlaySoundEffect(SlamSoundEffect);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange, HitableLayers);

        foreach (Collider2D hitObject in hitObjects) {
            if (hitObject.TryGetComponent<Controller_Combat>(out Controller_Combat combat)) {
                combat.ApplyDamage(Damage, StaminaReduction, transform);
            }
        }
    }

    private IEnumerator Cooldown(float duration)
    {
        IsActive = true;
        yield return new WaitForSeconds(duration);
        Deactivate(Vector2.zero);
    }

    private void OnDrawGizmos()
    {
        if (AttackPoint == null) {
            return;
        }

        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    }

    public void Deactivate(Vector2 direction)
    {
        StopAllCoroutines();
        if (ChargeParticles != null) {
            ChargeParticles.Stop();
        }

        if (SlamParticles != null) {
            SlamParticles.Stop();
        }

        _animator.SetBool(ChargeAnimation, false);
        IsActive = false;
    }

    public bool CanActivate(Vector2 direction)
    {
        return true;
    }
}
