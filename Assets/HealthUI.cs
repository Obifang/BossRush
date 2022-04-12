using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Health HealthObject;
    public Slider Slider;

    private float _currentValue;

    void Start()
    {
        Slider.maxValue = HealthObject.MaxHealthValue;
        Slider.value = HealthObject.StartingHealthValue;
        HealthObject.ChangeInHealth += AdjustHealthValue;
    }

    public void AdjustHealthValue(float value)
    {
        Slider.value = value;
    }
}
