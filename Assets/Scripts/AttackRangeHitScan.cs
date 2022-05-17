using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeHitScan : MonoBehaviour, IActionable
{
    public LayerMask HitableLayers;
    public float Distance;
    public float Damage = 1.0f;

    public int ID;
    public string Name;

    public int GetID { get => ID; }

    public string GetName { get => Name; }
    public bool IsActive { get; private set; }

    private bool Flipped;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Activate(Vector2 direction)
    {
        StartCoroutine(Use(direction));
    }

    private IEnumerator Use(Vector2 direction)
    {
        Debug.Log("Range Attack!");
        yield return new WaitForEndOfFrame();
        RaycastHit2D hit;

        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        hit = Physics2D.Raycast(pos, direction, Distance, HitableLayers);

        if (hit && hit.transform.gameObject.TryGetComponent<Health>(out Health health)) {
            Debug.DrawLine(pos, hit.point, Color.red);
            health.CalculateHealthChange(Damage);
        }
    }
}
