using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OnContactDamage : MonoBehaviour
{
    public float Damage;
    public LayerMask DamagingLayer;
    public float KnockbackForce = 0f;
    public float Duration = 1.0f;

    private float _timer;
    private bool _locked;
    private IHasState<MovementState> _hasState;

    // Start is called before the first frame update
    void Start()
    {
        _hasState = GetComponentInParent<IHasState<MovementState>>();
    }

    private void Update()
    {
        if (_locked) {
            _timer += Time.deltaTime;

            if (_timer >= Duration) {
                _locked = false;
                _timer = 0f;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (1 << collision.gameObject.layer == DamagingLayer) {
            DoDamage(collision);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (1 << collision.gameObject.layer == DamagingLayer) {
            DoDamage(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer == DamagingLayer) {
            //StartCoroutine(DoDamage(collision.gameObject));
        }
    }

    void DoDamage(Collision2D col)
    {
        if (_locked || (_hasState != null && _hasState.GetCurrentState()  == MovementState.Dashing)) {
            return;
        }

        _locked = true;
        var combatController = col.gameObject.GetComponent<Controller_Combat>();
        if (combatController != null)
            combatController.ApplyDamage(Damage, 0);

        var mv = col.gameObject.GetComponent<Controller_Movement>();

        if (mv != null) {
            mv.StopMoving();
            mv.KnockBack(CalculateDirection(col), KnockbackForce, Duration);
        }
    }

    Vector2 CalculateDirection(Collision2D collision)
    {
        Vector2 direction = Vector2.zero;

        var dirFromContact = (collision.gameObject.transform.position - transform.position).normalized;

        direction = dirFromContact;

        return direction;
    }
}
