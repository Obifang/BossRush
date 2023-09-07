using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Combat : MonoBehaviour
{
    private Health _health;
    private Stamina _stamina;
    private Controller_Movement _movement;
    private BaseController _baseController;

    private float _totalStaminaReduction;
    private float _defense;

    public delegate void DamageSequence(float value, float stamina, Transform damageSource = null);
    public event DamageSequence DamageBeingApplied;

    public delegate void StaggeredEnd();
    public event StaggeredEnd StaggeredEnded;

    public float StunDurationFromStaminaOut = 1.0f;
    public bool Staggered { get; private set; }
    public float Defense { get => _defense; set => _defense = value; }

    // Start is called before the first frame update
    void Start()
    {
        _health = GetComponent<Health>();
        _stamina = GetComponent<Stamina>();
        _baseController = GetComponent<BaseController>();
        _movement = GetComponent<Controller_Movement>();
        _stamina.AddToOutOfStaminaListener(PlayWhenOutOfStamina);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage(float damage, float staminaReduction, Transform DamageSource = null, bool IgnoreDefense = false)
    {
        var stam = CalculateStaminaReduction(staminaReduction);
        var dmg = CalculateDamageValue(damage);

        if (IgnoreDefense) {
            dmg = damage;
        }

        if (DamageBeingApplied != null)
            DamageBeingApplied.Invoke(dmg, stam, DamageSource);

        _health.CalculateHealthChange(dmg);
        _stamina.ReduceStamina(stam);
    }

    public float CalculateDamageValue(float damage)
    {
        return damage - _defense;
    }

    public float CalculateStaminaReduction(float staminaReduction)
    {
        return staminaReduction - _totalStaminaReduction;
    }

    public void AdjustDefense(float value)
    {
        _defense += value;
    }

    public void AdjustStaminaReduction(float value)
    {
        _totalStaminaReduction += value;
    }

    public bool HasRequiredStaminaToUse(float staminaToUse)
    {
        if (_stamina.GetCurrentStamina - staminaToUse <= 0f) {
            return false;
        }

        return true;
    }

    public bool UseStamina(float staminaToUse)
    {
        if (!HasRequiredStaminaToUse(staminaToUse)) {
            PlayWhenOutOfStamina();
            return false;
        } else {
            _stamina.ReduceStamina(staminaToUse);
            return true;
        }
    }

    private void PlayWhenOutOfStamina()
    {
        _stamina.PlayOutOfStaminaAnimation();
        _movement.LockMovementForDuration(StunDurationFromStaminaOut);
        StartCoroutine(BeginStaggeredCooldown(StunDurationFromStaminaOut));
    }

    private IEnumerator BeginStaggeredCooldown(float value)
    {
        Staggered = true;
        yield return new WaitForSecondsRealtime(value);
        if (StaggeredEnded != null) {
            StaggeredEnded.Invoke();
        }
        Staggered = false;
    }
}
