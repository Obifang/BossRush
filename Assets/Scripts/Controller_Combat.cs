using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Combat : MonoBehaviour
{
    private Health _health;
    private Stamina _stamina;
    private BaseController _baseController;

    private float _totalStaminaReduction;
    private float _defense;

    public delegate void DamageSequence(float value, float stamina);
    public event DamageSequence DamageBeingApplied;

    // Start is called before the first frame update
    void Start()
    {
        _health = GetComponent<Health>();
        _stamina = GetComponent<Stamina>();
        _baseController = GetComponent<BaseController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage(float damage, float staminaReduction)
    {
        var stam = CalculateStaminaReduction(staminaReduction);
        var dmg = CalculateDamageValue(damage);

        if (DamageBeingApplied != null)
            DamageBeingApplied.Invoke(dmg, stam);

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
}
