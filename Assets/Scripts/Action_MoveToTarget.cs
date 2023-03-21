using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Action_MoveToTarget : MonoBehaviour, IActionable
{
    public int ID;
    public string Name;

    public int GetID { get => ID; }
    public string GetName { get => Name; }
    public bool IsActive { get; private set; }

    public float RangeToCheckCompletion;

    private Controller_Movement _movementController;
    private Vector2 _oldPos;
    private Vector2 _newPos;

    public void Activate(Vector2 position)
    {
        _oldPos = position;
        var newPos = (position - (Vector2)transform.position).normalized;
        newPos = new Vector2(newPos.x, 0);
        _movementController.Move(newPos.x, newPos.y);
        _movementController.UpdateState(MovementState.Moving);
        _newPos = newPos;
        IsActive = true;
    }

    public bool CanActivate(Vector2 direction)
    {
        return (_movementController.Grounded && !TargetWithinRange(direction) && _movementController.CheckForGroundAtPosition(direction));
    }

    public void Deactivate(Vector2 direction)
    {
        IsActive = false;
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
            _movementController.Move(_newPos.x, _newPos.y);
            if (TargetWithinRange(_oldPos)) {
                Debug.Log("Completed");
                Deactivate(_oldPos);
            }
            
        }
    }
}
