using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{

    public Stamina StaminaObject;
    public Slider Slider;
    public bool AnchorAboveObject;
    public Vector2 Offset;

    private float _currentValue;
    private Camera _camera;

    void Start()
    {
        Slider.maxValue = StaminaObject.MaxStamina;
        Slider.value = StaminaObject.StartingStamina;
        StaminaObject.StaminaAdjusted += AdjustStaminaValue;
        _camera = Camera.main;
    }

    private void Update()
    {
        if (AnchorAboveObject) {
            FollowObject();
        }
    }

    public void AdjustStaminaValue(float value)
    {
        Slider.value = value;
    }

    private void FollowObject()
    {
        ((RectTransform)Slider.transform).anchoredPosition = _camera.WorldToScreenPoint(StaminaObject.transform.position) + (Vector3)Offset;
    }
}
