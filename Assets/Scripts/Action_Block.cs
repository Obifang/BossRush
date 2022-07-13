using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Block : MonoBehaviour, IActionable
{
    public int ID;
    public string Name;
    public string AssociatedAnimationName;

    public LayerMask HitableLayers;
    public float DefenseRange;
    public float Duration = 1f;
    public int FrameCheckCount = 2;

    public int GetID { get => ID; }

    public string GetName { get => Name; }

    public bool IsActive { get; private set; }
    private Animator _animator;
    private int _frameCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (IsActive) {
            _frameCounter++;
            if (_frameCounter >= FrameCheckCount) {
                IsActive = false;
                if (_animator != null) {
                    _animator.SetBool(AssociatedAnimationName, false);
                }
            }
        }
    }

    public void Activate(Vector2 direction)
    {
        if (_animator != null) {
            _animator.SetBool(AssociatedAnimationName, true);
        }
        IsActive = true;
        _frameCounter = 0;
    }

    public void Deactivate(Vector2 direction)
    {
        if (_frameCounter <= FrameCheckCount && IsActive)
            return;

        StopAllCoroutines();
        IsActive = false;
        if (_animator != null) {
            _animator.SetBool(AssociatedAnimationName, false);
        }
    }

    private void OnDrawGizmos()
    {

    }
}
