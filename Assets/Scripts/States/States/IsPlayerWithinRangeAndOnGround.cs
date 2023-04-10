using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerWithinRangeAndOnGround : SMDecision
{
    public float RangeCheck;
    public float GroundCheckDistance;

    private Controller_Movement _movement;
    private Collider _collider;

    public override bool Decide(StateMachine stateMachine)
    {
        if (_movement == null) {
            _movement = stateMachine.GetComponent<Controller_Movement>();
        }

        if (_collider == null) {
            _collider = stateMachine.GetComponent<Collider>();
        }

        return true;
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

}
