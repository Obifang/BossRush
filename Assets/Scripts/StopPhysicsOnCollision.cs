using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic Script used to lock Rigidbody constraints when colliding with an object of a certain layer.
/// Used to address issues where a player and enemy are able to move eachother when colliding with one another.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class StopPhysicsOnCollision : MonoBehaviour
{
    public LayerMask LayersToStopWhenColliding;
    
    private Rigidbody2D _rb;
    private RigidbodyConstraints2D _startingConstraints;


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _startingConstraints = _rb.constraints;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (1 << collision.gameObject.layer == LayersToStopWhenColliding) {
            Debug.Log("Stopped Physics");
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (1 << collision.gameObject.layer == LayersToStopWhenColliding) {
            Debug.Log("Began Physics");
            _rb.constraints = _startingConstraints;
        }
    }
}
