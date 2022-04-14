using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeHitScan : MonoBehaviour
{
    public LayerMask HitableLayers;
    public MovementScript Player;
    public float Distance;
    public float Damage = 1.0f;

    private bool Flipped;
    // Start is called before the first frame update
    void Start()
    {
        Player.Fliped += FlipAttackDirection;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2)) {
            StartCoroutine(Use());
        }
    }

    void FlipAttackDirection(bool value)
    {
        Flipped = value;
    }

    private IEnumerator Use()
    {
        Debug.Log("Range Attack!");
        yield return new WaitForEndOfFrame();
        RaycastHit2D hit;

        Vector2 pos = new Vector2(transform.position.x, transform.position.y);  

        if (Flipped) {
            hit = Physics2D.Raycast(pos, Vector2.left, Distance, HitableLayers);
        } else {
            hit = Physics2D.Raycast(pos, Vector2.right, Distance, HitableLayers);
        }

        if (hit && hit.transform.gameObject.TryGetComponent<Health>(out Health health)) {
            Debug.DrawLine(pos, hit.point, Color.red);
            health.CalculateHealthChange(Damage);
        }
    }
}
