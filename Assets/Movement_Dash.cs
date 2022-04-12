using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Dash : MonoBehaviour
{
    public float DashSpeed = 5.0f;
    public float DashCooldown = 2.0f;
    public float DashTickrate = 0.1f;

    private float _currentCooldown;
    private float _currentTickrate;
    private Rigidbody2D _rb;
    private float _hDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentCooldown > 0f) {
            _currentCooldown -= Time.deltaTime;
            _currentTickrate -= Time.deltaTime;

            if (_currentTickrate > 0) {
                Dash();
            }
        }
    }

    public void BeginDash(float horizontalValue)
    {
        if (_currentCooldown > 0f) {
            return;
        }
        _hDirection = horizontalValue;
        _currentCooldown = DashCooldown;
        _currentTickrate = DashTickrate;
    }

    private void Dash()
    {
        if (_currentCooldown > 0f) {
            return;
        }
        _rb.velocity = new Vector2(_hDirection, 0).normalized * DashSpeed;
    }
}
