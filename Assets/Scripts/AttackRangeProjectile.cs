using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeProjectile : MonoBehaviour, IActionable
{
    public Projectile projectileType;
    
    public Transform AttackPoint;

    public int ID;
    public string Name;
    public float Cooldown = 0.5f;

    public int GetID { get => ID; }

    public string GetName { get => Name; }
    public bool IsActive { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Activate(Vector2 direction)
    {
        StartCoroutine(StartCooldown(Cooldown));
        StartCoroutine(Use(direction));
    }

    private IEnumerator Use(Vector2 direction)
    {
        yield return new WaitForEndOfFrame();

        var proj = Instantiate(projectileType, AttackPoint.position, Quaternion.identity);
        proj.Fire(direction);
    }

    private IEnumerator StartCooldown(float value)
    {
        IsActive = true;
        yield return new WaitForSecondsRealtime(value);
        IsActive = false;
    }
}
