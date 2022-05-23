using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shake : MonoBehaviour
{
    public float Speed = 1.0f;
    public float ShakeAmount = 1.0f;
    public float ShakeDuration = 1.0f;
    public float ShakeRange = 1.0f;

    private Vector2 _originalPos;
    private bool _shake = false;
    private float _timer = 0f;
    // Start is called before the first frame update
    private void Start()
    {
        _originalPos = transform.position;
    }

    void ShakeButton()
    {
        var pos = transform.position;
        pos.x += Mathf.Sin(Time.time * Speed * Random.Range(-ShakeRange, ShakeRange)) * ShakeAmount;
        pos.y += Mathf.Sin(Time.time * Speed * Random.Range(-ShakeRange, ShakeRange)) * ShakeAmount;
        transform.position = pos;
    }

    private void Update()
    {
        if (_shake) {
            _timer += Time.deltaTime;
            if (_timer >= ShakeDuration) {
                transform.position = _originalPos;
                _timer = 0f;
            }
            ShakeButton();
        }
    }

    public void ContinousShake()
    {
        _shake = true;
    }

    public void StopContinousShake()
    {
        _shake = false;
        transform.position = _originalPos;
    }
}
