using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Dash : MonoBehaviour
{
    public float DashSpeed = 25.0f;
    public float DashCooldown = 1.0f;
    public bool IsDashing {
        get; private set;
    }

    private float _currentCooldown;
    private Rigidbody2D _rb;
    private Vector2 _dir;
    // Update is called once per frame
    void Update()
    {
        if (_currentCooldown > 0f) {
            _currentCooldown -= Time.deltaTime;
            _rb.velocity = _dir * DashSpeed;
        } else if (_currentCooldown < 0f){
            _currentCooldown = 0f;
            IsDashing = false;
        }
    }

    public void Dash(float horizontalValue, Rigidbody2D rb)
    {
        if (_currentCooldown > 0f) {
            return;
        }
        IsDashing = true;
        _currentCooldown = DashCooldown;
        if (DashUI.instance != null) {
            DashUI.instance.Dashed(DashCooldown);
        }
        
        _rb = rb;
        _dir = new Vector2(horizontalValue, 0).normalized;
    }


}
