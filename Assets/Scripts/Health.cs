using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Health : MonoBehaviour
{
    public bool Immortal = false;
    public string DeathAnimationName;
    public string HurtAnimationName = "Hurt";
    public float StartingHealthValue = 10.0f;
    public float MaxHealthValue = 10.0f;

    private float _currentHealth;
    private Animator _animator;
    private BaseController _controller;
    private ActionHandler _handler;
    private bool _deathTriggered = false;
    private float _damageReductionAmount;

    public delegate void HealthChange(float value);
    public event HealthChange ChangeInHealth;

    public delegate float PreHealthChange(float value);
    public event PreHealthChange GetReductions;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = StartingHealthValue;
        _animator = GetComponent<Animator>();
        _controller = GetComponent<BaseController>();
        _handler = GetComponent<ActionHandler>();
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
            _animator.SetTrigger(HurtAnimationName);
            _handler.CurrentAction.Deactivate(Vector2.zero);
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

        Destroy(gameObject);
    }
}
