using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_SeekEnemy : MonoBehaviour, IActionable
{
    public int ID;
    public string Name;

    public int GetID { get => ID; }
    public string GetName { get => Name; }
    public bool IsActive { get; private set; }

    public float RangeToCheckCompletion;
    public float HeightDifferenceCheck = 2.5f;
    public Transform TransformToSeek;

    private Controller_Movement _movementController;
    private Vector2 _oldPos;
    private Vector2 _newPos;

    public void Activate(Vector2 position)
    {
        _movementController.UpdateState(MovementState.Moving);
        IsActive = true;
    }

    public bool CanActivate(Vector2 direction)
    {
        return (_movementController.Grounded && TargetAtCorrectHeight() && _movementController.CheckForGroundAtPosition(transform.position));
    }

    public void Deactivate(Vector2 direction)
    {
        IsActive = false;
    }

    bool TargetAtCorrectHeight()
    {
        return Mathf.Abs(transform.position.y - TransformToSeek.position.y) <= HeightDifferenceCheck;
    }

    bool TargetWithinRange(Vector2 newPos)
    {
        return Mathf.Abs(transform.position.x - newPos.x) <= RangeToCheckCompletion;
    }

    // Start is called before the first frame update
    void Start()
    {
        _movementController = GetComponent<Controller_Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive) {
            var newPos = ((Vector2)TransformToSeek.position - (Vector2)transform.position).normalized;
            newPos = new Vector2(newPos.x, 0);
            _movementController.Move(newPos.x, newPos.y);
            if (TargetWithinRange(TransformToSeek.position)) {
                Deactivate(newPos);
            }

        }
    }
}
