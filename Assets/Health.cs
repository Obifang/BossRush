using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float StartingHealthValue = 10.0f;
    public float MaxHealthValue = 10.0f;

    private float _currentHealth;

    public delegate void HealthChange(float value);
    public event HealthChange ChangeInHealth;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = StartingHealthValue;
    }

    public void CalculateHealthChange(float damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0f || _currentHealth - damage < 0f) {
            _currentHealth = 0f;
            Debug.Log("Dead");
        }

        if (ChangeInHealth != null) {
            ChangeInHealth.Invoke(_currentHealth);
        }
    }
}
