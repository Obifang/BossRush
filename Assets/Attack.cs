using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public LayerMask HitableLayers;
    public Transform AttackPoint;
    public float AttackRange;
    public SpriteRenderer AttackerRenderer;

    // Start is called before the first frame update
    void Start()
    {
        DashUI.instance._movementScript.Fliped += FlipAttackPoint;
    }

    void FlipAttackPoint(bool value)
    {
        AttackPoint.localPosition = new Vector2(AttackPoint.localPosition.x * -1f, AttackPoint.localPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Use());
        }
    }

    private IEnumerator Use()
    {
        Debug.Log("Attack!");
        yield return new WaitForEndOfFrame();
        Collider2D [] hitObjects = Physics2D.OverlapCircleAll(AttackPoint.position, 1.0f, HitableLayers);

        foreach(Collider2D hitObject in hitObjects)
        {
            var health =  hitObject.GetComponent<Health>();
            if (health != null) {
                health.CalculateHealthChange(1f);
            }
            Debug.Log("Hit Object");
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
