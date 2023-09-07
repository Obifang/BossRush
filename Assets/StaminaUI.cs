using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    private enum HidingUI
    {
        ShowUI,
        HideUI
    }


    public Stamina StaminaObject;
    public Slider Slider;
    public bool AnchorAboveObject;
    public Vector2 Offset;
    public bool HideUIIfFullForATime = false;
    public float TimeToHideAfter = 1.0f;
    public float FadeDuration = 1.0f;
    public bool FlipWithCharacter = false;

    private float _currentValue;
    private Camera _camera;
    private IFlippable _characterController;
    private float _counter;
    private HidingUI _UIState = HidingUI.ShowUI;
    private CanvasGroup _canvasGroup;
    private Vector2 _offset;

    void Start()
    {

    }

    public void Setup()
    {
        _camera = Camera.main;
        Slider.maxValue = StaminaObject.MaxStamina;
        Slider.value = StaminaObject.StartingStamina;
        StaminaObject.StaminaAdjusted += AdjustStaminaValue;
        _characterController = StaminaObject.GetComponent<IFlippable>();
        _characterController.Fliped += FlipOffSet;
        _canvasGroup = GetComponent<CanvasGroup>();
        _counter = TimeToHideAfter;
        _UIState = HidingUI.ShowUI;
        _offset.x = (Offset.x / Screen.currentResolution.width) * Screen.width;
        _offset.y = (Offset.y / Screen.currentResolution.height) * Screen.height;
        Debug.Log("Player Pos: " + StaminaObject.transform.position);
    }

    private void FlipOffSet(bool value)
    {
        if (!FlipWithCharacter)
            return;

        _offset = new Vector2(-_offset.x, _offset.y);
    }

    private void Update()
    {
        if (StaminaObject == null) {
            return;
        }
        if (AnchorAboveObject) {
            FollowObject();
        }

        if (HideUIIfFullForATime) {
            HideUI();
        }
    }

    public void HideUI()
    {
        switch (_UIState) {
            case HidingUI.HideUI:
                if (_counter > 0) {
                    _counter -= Time.deltaTime;
                    _canvasGroup.alpha =  Mathf.Lerp(1, 0, (FadeDuration - _counter) / FadeDuration);
                }
                break;
            case HidingUI.ShowUI:
                _counter -= Time.deltaTime;

                if (_counter <= 0) {
                    _counter = FadeDuration;
                    _UIState = HidingUI.HideUI;
                }
                break;
        }
    }

    public void AdjustStaminaValue(float value)
    {
        Slider.value = value;
        _UIState = HidingUI.ShowUI;
        _canvasGroup.alpha = 1;
        _counter = TimeToHideAfter;
    }

    private void FollowObject()
    {
        ((RectTransform)Slider.transform).position = RectTransformUtility.WorldToScreenPoint(_camera, StaminaObject.transform.position) + _offset;
    }
}
