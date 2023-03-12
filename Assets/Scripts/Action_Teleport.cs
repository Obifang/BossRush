using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Action_Teleport : MonoBehaviour, IActionable
{
    public int ID;
    public string Name;
    public string AnimationName;
    public float TeleportTime;
    public float GroundCheckDistance = 2.0f;
    public float AdditionalDistanceFromTarget = 0f;
    public int GetID { get => ID; }
    public string GetName { get => Name; }
    public bool IsActive { get => _isTeleporting;}

    private bool _isTeleporting;
    private float _teleportTimer;
    private Vector2 _newPos;
    private Animator _animator;
    private bool _hasFaded;
    private Controller_Movement _movement;
    private Collider2D _collider;

    public void Start()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<Controller_Movement>();
        _collider = GetComponent<Collider2D>();
    }

    public void Activate(Vector2 direction)
    {
        if (_isTeleporting || !GroundChecks(direction)) {
            return;
        }

        _isTeleporting = true;
        _newPos = GetPositionNextToTarget(direction);

        if (_newPos == Vector2.zero) {
            _isTeleporting = false;
            return;
        }
        _animator.SetTrigger(AnimationName);
        FadeIn();
    }

    public void Deactivate(Vector2 direction)
    {
        _isTeleporting = false;
        _teleportTimer = 0f;
        _animator.SetFloat("AnimationDirection", 1f);
        _animator.speed = 1;
    }

    private Vector2 GetPositionNextToTarget(Vector2 pos)
    {
        var width = _collider.bounds.size.x + AdditionalDistanceFromTarget;
        var left = new Vector2(pos.x - width, pos.y);
        var right = new Vector2(pos.x + width, pos.y);

        if (GroundChecks(left)) {
            return left;
        } else if (GroundChecks(right)){
            return right;
        }

        return Vector2.zero;
    }

    public void TeleportToPosition(Vector2 newPos)
    {
        gameObject.transform.position = newPos;
    }

    private void FadeIn()
    {
        _animator.ResetTrigger(AnimationName);
        _animator.speed = 1 / (TeleportTime * 0.5f);
        _animator.SetTrigger(AnimationName);
        _hasFaded = false;
    }

    private void FadeOut()
    {
        _animator.ResetTrigger(AnimationName);
        _animator.SetTrigger(AnimationName);
        _hasFaded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isTeleporting) {
            _teleportTimer += Time.deltaTime;
            if (_teleportTimer >= TeleportTime * 0.5f) {
                if (!_hasFaded) {
                    FadeOut();
                    TeleportToPosition(_newPos);
                }
                if (_teleportTimer >= TeleportTime) {
                    Deactivate(_newPos);
                }
            }
        }
    }

    bool GroundChecks(Vector2 newPos)
    {
        return (_movement.Grounded && CheckForGroundAtPosition(newPos));
    }

    bool CheckForGroundAtPosition(Vector2 newPos)
    {
        var groundLayer = 1 << LayerMask.NameToLayer("Ground");
        var halfWidth = _collider.bounds.size.x * 0.5f;
        var left = new Vector2(newPos.x - halfWidth, newPos.y);
        var right = new Vector2(newPos.x + halfWidth, newPos.y);
        var centerHit = Physics2D.Raycast(newPos, Vector2.down, GroundCheckDistance, groundLayer);
        var leftHit = Physics2D.Raycast(left, Vector2.down, GroundCheckDistance, groundLayer);
        var rightHit = Physics2D.Raycast(right, Vector2.down, GroundCheckDistance, groundLayer);

        return (leftHit && rightHit && centerHit);
    }

    public bool CanActivate(Vector2 direction)
    {
        return GroundChecks(direction);
    }
}