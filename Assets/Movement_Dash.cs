using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Dash : MonoBehaviour
{
    public float DashSpeed = 25.0f;
    public float DashCooldown = 1.0f;

    private float _currentCooldown;

    // Update is called once per frame
    void Update()
    {
        if (_currentCooldown > 0f) {
            _currentCooldown -= Time.deltaTime;
        }
    }

    private void BeginDash()
    {
        if (_currentCooldown > 0f) {
            return;
        }
        Debug.Log("Dashing");
        _currentCooldown = DashCooldown;
        DashUI.instance.Dashed(DashCooldown);
    }

    public void Dash(float horizontalValue, Rigidbody2D rb)
    {
        if (_currentCooldown > 0f) {
            return;
        }

        BeginDash();
        rb.AddForce(new Vector2(horizontalValue, 0).normalized * DashSpeed, ForceMode2D.Impulse);
    }
}
