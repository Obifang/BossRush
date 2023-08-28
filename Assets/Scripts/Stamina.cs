using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [SerializeField]
    public float MaxStamina = 10f;
    [SerializeField]
    public float StartingStamina = 10f;
    [SerializeField]
    private float TickRate;
    [SerializeField]
    private float TickAmount = 1.0f;

    public string OutOfStaminaAnimationTrigger;
    public float GetCurrentStamina { get => _stamina; }
    public delegate void OutOfStamina();
    public event OutOfStamina OnOutOfStamina;

    public delegate void StaminaChange(float value);
    public event StaminaChange StaminaAdjusted;

    float _stamina;
    float _timer;
    private Animator _animator;
    private Controller_Movement _movementController;

    // Start is called before the first frame update
    void Start()
    {
        _timer = 0f;
        _stamina = StartingStamina;
        _animator = GetComponent<Animator>();
        OnOutOfStamina += PlayOutOfStaminaAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        if (_stamina < MaxStamina) {
            _timer += Time.deltaTime;

            if (_timer > TickRate) {
                _timer = 0f;
                IncreaseStamina(TickAmount);
            }
        }
    }

    public void PlayOutOfStaminaAnimation()
    {
        if (OutOfStaminaAnimationTrigger != "") {
            _animator.SetTrigger(OutOfStaminaAnimationTrigger);
        }
    }
    
    public void ReduceStamina(float value)
    {
        _stamina -= value;

        if (StaminaAdjusted != null) {
            StaminaAdjusted.Invoke(_stamina);
        }

        if (_stamina <= 0f) {
            _stamina = 0f;
            if (OnOutOfStamina != null)
                OnOutOfStamina.Invoke();
        }
    }
    public void IncreaseStamina(float value)
    {
        _stamina += value;

        if (_stamina > MaxStamina)
            _stamina = MaxStamina;

        if (StaminaAdjusted != null) {
            StaminaAdjusted.Invoke(_stamina);
        }
    }
    public void SetStamina(float value)
    {
        _stamina = value;

        if (_stamina > MaxStamina)
            _stamina = MaxStamina;

        if (StaminaAdjusted != null) {
            StaminaAdjusted.Invoke(_stamina);
        }
    }
    public void SetMaxStamina(float value)
    {
        MaxStamina = value;
        if (StaminaAdjusted != null) {
            StaminaAdjusted.Invoke(_stamina);
        }
    }

    public void AddToOutOfStaminaListener(OutOfStamina e)
    {
        OnOutOfStamina += e;
    }

    public void RemoveOutOfStaminaListener(OutOfStamina e)
    {
        OnOutOfStamina -= e;
    }
}
