using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IActionable
{
    public LayerMask HitableLayers;
    public Transform AttackPoint;
    public float AttackRange;
    public SpriteRenderer AttackerRenderer;

    private IFlippable Flippable;
    // Start is called before the first frame update
    void Start()
    {
        Flippable = GetComponent<IFlippable>();
        Flippable.Fliped += FlipAttackPoint;
    }

    void FlipAttackPoint(bool value)
    {
        AttackPoint.localPosition = new Vector2(AttackPoint.localPosition.x * -1f, AttackPoint.localPosition.y);
    }

    public void Activate(Vector2 direction)
    {
        StartCoroutine(Use());
    }

    private IEnumerator Use()
    {
        yield return new WaitForEndOfFrame();
        Collider2D [] hitObjects = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange, HitableLayers);

        foreach(Collider2D hitObject in hitObjects) {
            if (hitObject.TryGetComponent<Health>(out Health health)) {
                health.CalculateHealthChange(1f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (AttackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    }
}
