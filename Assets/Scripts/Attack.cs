using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IActionable
{
    public LayerMask HitableLayers;
    public Transform AttackPoint;
    public float AttackRange;
    public SpriteRenderer AttackerRenderer;
    public float Cooldown = 1f;
    private IFlippable Flippable;

    public int ID;
    public string Name;

    public int GetID { get => ID;}

    public string GetName { get => Name;}

    public bool IsActive { get; private set; }

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
        if (IsActive) {
            return;
        }

        StartCoroutine(StartCooldown(Cooldown));
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

    private IEnumerator StartCooldown(float value)
    {
        IsActive = true;
        yield return new WaitForSecondsRealtime(value);
        IsActive = false;
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
