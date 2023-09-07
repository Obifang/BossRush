using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Health : MonoBehaviour
{
    public bool Immortal = false;
    public string DeathAnimationName;
    public string HurtAnimationName = "Hurt";
    public string HurtSoundEffect;
    public bool PlayHurtAnimationWhenTakingDamage = true;
    public bool StopCurrentActionWhenTakingDamage = true;
    public float StartingHealthValue = 10.0f;
    public float MaxHealthValue = 10.0f;
    public bool FlashOnHit;
    public Material MaterialToFlash;
    [Range(0f, 1f)]
    public float FlashAmount;
    public float FlashTime;


    private float _currentHealth;
    private Animator _animator;
    private BaseController _controller;
    private ActionHandler _handler;
    private bool _deathTriggered = false;
    private float _damageReductionAmount;
    private Material _originalMaterial;
    private SpriteRenderer _spriteRenderer;

    public delegate void HealthChange(float value);
    public event HealthChange ChangeInHealth;

    public delegate float PreHealthChange(float value);
    public event PreHealthChange GetReductions;

    public delegate void ZeroHealth();
    public event ZeroHealth DeathEvent;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = StartingHealthValue;
        _animator = GetComponent<Animator>();
        _controller = GetComponent<BaseController>();
        _handler = GetComponent<ActionHandler>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private IEnumerator Flash(float time)
    {
        _originalMaterial = _spriteRenderer.material;
        _spriteRenderer.material = MaterialToFlash;
        _spriteRenderer.material.SetFloat("_FlashAmount", FlashAmount);
        yield return new WaitForSeconds(time);
        _spriteRenderer.material.SetFloat("_FlashAmount", 0);
        _spriteRenderer.material = _originalMaterial;
    }

    public void BeginFlash(float time)
    {
        StartCoroutine(Flash(time));
    }

    public void CalculateHealthChange(float damage)
    {
        if (Immortal) {
            damage = 0f;
            return;
        }
        if (_deathTriggered) {
            _damageReductionAmount = 0;
            return;
        }

        float damageAdjustments = 0;

        if (GetReductions != null) {
            damageAdjustments += GetReductions.Invoke(damage);
        }

        var cHP = _currentHealth;
        _currentHealth -= Mathf.Clamp(damage - damageAdjustments, 0, damage);

        if (cHP != _currentHealth) {
            if (PlayHurtAnimationWhenTakingDamage) {
                _animator.SetTrigger(HurtAnimationName);
            }

            if (FlashOnHit) {
                BeginFlash(FlashTime);
            }

            Manager_Audio.Instance.PlaySoundEffect(HurtSoundEffect);

            if (StopCurrentActionWhenTakingDamage) {
                _handler.DeactivateCurrentAction(Vector2.zero);
            }
        }
        
        
        if (_currentHealth <= 0f) {
            _currentHealth = 0f;
            Death();
            Debug.Log("Dead");
        }

        if (ChangeInHealth != null) {
            ChangeInHealth.Invoke(_currentHealth);
        }
    }
    
    public void AddDamageReduction(float value)
    {
        _damageReductionAmount += value;
    }

    public void Death()
    {
        _deathTriggered = true;
        _controller.SetActive(false);
        if (_animator != null) {
            _animator.SetBool(DeathAnimationName, true);
        }
        StartCoroutine(Dying());
    }

    private IEnumerator Dying()
    {
        if (_animator != null) {
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length *  (1 - _animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        }

        DeathEvent.Invoke();
        //Manager_GameState.Instance.ChangeGameState(Manager_GameState.GameState.Gameover);

        Destroy(gameObject);
    }
}
