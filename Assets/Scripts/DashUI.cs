using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    public Slider _slider;

    private float _currentValue;


    public static DashUI instance;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _slider.value = 1;
    }

    private IEnumerator CoolDown(float value)
    {
        yield return new WaitForEndOfFrame();

        while(_currentValue < value)
        {
            _currentValue += Time.deltaTime;
            _slider.value = value - _currentValue;
            yield return new WaitForEndOfFrame();
        }

        _slider.value = 1;
    }

    public void Dashed(float value)
    {
        _currentValue = 0f;
        _slider.value = 0f;
        _slider.maxValue = value;
        StartCoroutine(CoolDown(value));
    }
}
