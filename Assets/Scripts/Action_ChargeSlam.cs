using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_ChargeSlam : MonoBehaviour, IActionable
{
    public int ID;
    public string Name;
    public float ChargeDuration = 4.0f;
    public float Damage = 3.0f;
    public string ChargeAnimation;
    public string SlamAnimation;
    public float CooldownDuration;
    public Transform AttackPoint;
    public float AttackRange;
    public LayerMask HitableLayers;
    public ParticleSystem ChargeParticles;
    public ParticleSystem SlamParticles;

    public int GetID { get => ID;}
    public string GetName { get => Name;}
    public bool IsActive { get; private set; }

    private Animator _animator;
    private float _timer;
    private ParticleSystem _particles;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _particles = GetComponent<ParticleSystem>();
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
        if (ChargeParticles != null) {
            ChargeParticles.Play();
        }
        yield return new WaitForSeconds(ChargeDuration);
        if (ChargeParticles != null) {
            ChargeParticles.Stop();
        }

        if (SlamParticles != null) {
            SlamParticles.Play();
        }
        _animator.SetBool(ChargeAnimation, false);
        _animator.SetTrigger(SlamAnimation);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange, HitableLayers);

        foreach (Collider2D hitObject in hitObjects) {
            if (hitObject.TryGetComponent<Health>(out Health health)) {
                health.CalculateHealthChange(Damage);
            }
        }
    }

    private IEnumerator Cooldown(float duration)
    {
        IsActive = true;
        yield return new WaitForSeconds(duration);
        if (SlamParticles != null) {
            SlamParticles.Stop();
        }
        IsActive = false;
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
