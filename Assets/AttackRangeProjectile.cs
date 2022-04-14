using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeProjectile : MonoBehaviour
{
    public MovementScript Player;
    public Projectile projectileType;

    private bool Flipped;

    // Start is called before the first frame update
    void Start()
    {
        Player.Fliped += FlipAttackDirection;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(Use());
        }
    }

    void FlipAttackDirection(bool value)
    {
        Flipped = value;
    }

    private IEnumerator Use()
    {
        Debug.Log("Range Projectile Attack!");
        yield return new WaitForEndOfFrame();

        var proj = Instantiate(projectileType, transform.position, Quaternion.identity);

        if (Flipped) {
            proj.Fire(Vector2.left);
        } else {
            proj.Fire(Vector2.right);
        }
    }
}
