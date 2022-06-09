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
            DoDamage(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (1 << collision.gameObject.layer == DamagingLayer) {
            DoDamage(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer == DamagingLayer) {
            //StartCoroutine(DoDamage(collision.gameObject));
        }
    }

    void DoDamage(GameObject gO)
    {
        if (_locked || _hasState.GetCurrentState() == MovementState.Dashing) {
            return;
        }
       // yield return new WaitForEndOfFrame();
        _locked = true;

        var health = gO.GetComponent<Health>();
        if (health != null)
            health.CalculateHealthChange(Damage);

        var mS = gO.GetComponent<MovementForceBased>();
        if (mS != null) {
            var dir = (gO.transform.position - transform.position).normalized;
            mS.KnockBack(dir, KnockbackForce, Duration);
        }
    }
}
