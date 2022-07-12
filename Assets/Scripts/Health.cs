using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Health : MonoBehaviour
{
    public string DeathAnimationName;
    public float StartingHealthValue = 10.0f;
    public float MaxHealthValue = 10.0f;

    private float _currentHealth;
    private Animator _animator;
    private BaseController _controller;
    private ActionHandler _handler;
    private bool _deathTriggered = false;

    public delegate void HealthChange(float value);
    public event HealthChange ChangeInHealth;

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
        if (_deathTriggered) {
            return;
        }

        _currentHealth -= damage;
        _animator.SetTrigger("Hurt");
        _handler.CurrentAction.Deactivate(Vector2.zero);
        if (_currentHealth <= 0f) {
            _currentHealth = 0f;
            Death();
            Debug.Log("Dead");
        }

        if (ChangeInHealth != null) {
            ChangeInHealth.Invoke(_currentHealth);
        }
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
