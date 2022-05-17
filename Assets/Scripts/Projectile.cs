using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask HitableLayers;
    public LayerMask IgnoreLayers;
    public float Damage = 1.0f;
    public float Speed = 5.0f;
    public float LifeTime = 10f;

    private Vector2 _direction;
    private float _lifetime;

    private enum State
    {
        Disabled,
        Fired,
    }

    private State state;

    // Start is called before the first frame update
    void Awake()
    {
        state = State.Disabled;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) {
            case State.Disabled:
                break;
            case State.Fired:
                transform.position += (Vector3)(_direction * Speed * Time.deltaTime);
                _lifetime += Time.deltaTime;

                if (_lifetime >= LifeTime) {
                    Destroy(gameObject);
                }
                break;
        }
    }

    public void Fire(Vector2 direction)
    {
        state = State.Fired;
        _direction = direction.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) {
            return;
        }

        if (1 << collision.gameObject.layer == HitableLayers && collision.transform.TryGetComponent<Health>(out Health health)) {
            health.CalculateHealthChange(1f);
        }

        if (1 << collision.gameObject.layer != IgnoreLayers) {
            Destroy(gameObject);
        }
    }
}
