using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeProjectile : MonoBehaviour, IActionable
{
    public Projectile projectileType;
    
    public Transform AttackPoint;

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
        Debug.Log("Range Projectile Attack!");
        yield return new WaitForEndOfFrame();

        var proj = Instantiate(projectileType, AttackPoint.position, Quaternion.identity);
        proj.Fire(direction);
    }
}
