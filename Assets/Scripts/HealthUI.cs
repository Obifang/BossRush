using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Health HealthObject;
    public Slider Slider;
    public bool AnchorAboveObject;
    public Vector2 Offset;

    private float _currentValue;
    private Camera _camera;

    void Start()
    {
        Slider.maxValue = HealthObject.MaxHealthValue;
        Slider.value = HealthObject.StartingHealthValue;
        HealthObject.ChangeInHealth += AdjustHealthValue;
        _camera = Camera.main;
    }

    private void Update()
    {
        if (AnchorAboveObject) {
            FollowObject();
        }
    }

    public void AdjustHealthValue(float value)
    {
        Slider.value = value;
    }

    private void FollowObject()
    {
        ((RectTransform)Slider.transform).anchoredPosition = _camera.WorldToScreenPoint(HealthObject.transform.position) + (Vector3)Offset;
    }
}
