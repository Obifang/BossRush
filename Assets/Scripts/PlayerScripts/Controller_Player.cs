using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Controller_Player : BaseController
{
    Controller_Movement _movement;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _movement = GetComponent<Controller_Movement>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Manager_GameState.Instance.GetCurrentGameState != Manager_GameState.GameState.Playing) {
            return;
        }
        HandleInput();
        base.Update();
    }

    private void HandleInput()
    {
        _vertical = Input.GetAxisRaw("Vertical");
        var currentState = _movement.GetCurrentState();
        if (currentState == MovementState.WallSlide || currentState == MovementState.WallJump) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                _horizontal = -_facingDirection.x;
                _movement.Move(_horizontal, _vertical);
                _movement.Jump();
            }
        } else {
            _horizontal = Input.GetAxisRaw("Horizontal");
            _movement.Move(_horizontal, _vertical);
        }

        if (!_movement.Grounded) {
            if (Input.GetKeyDown(KeyCode.Mouse0) && _movement.GetCurrentState() == MovementState.Falling) {
                _movement.UpdateState(MovementState.Falling);
                _actionHandler.ActivateAction(_facingDirection, 3);
            }

            if (!_movement.Sliding && Input.GetKeyDown(KeyCode.Space) && _movement.CanDoubleJump) {
                _movement.DoubleJump();
            }

            return;
        }
            

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            _movement.UpdateState(MovementState.PerformingAction);
            _actionHandler.ActivateAction(_facingDirection, 0);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            IsFlipable = false;
            _movement.UpdateState(MovementState.Strafe);
            _actionHandler.ActivateAction(_facingDirection, 1);
        } else if (Input.GetKeyUp(KeyCode.Mouse1)) {
            IsFlipable = true;
            _actionHandler.DeactivateAction(_facingDirection, 1);
            _movement.UpdateState(MovementState.Moving);
        }

        if (!_movement.Sliding && Input.GetKeyDown(KeyCode.Space)) {
            _movement.Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            _movement.Dash(_facingDirection.x);
        }
    }

    public override void SetActive(bool value)
    {
        this.enabled = value;
    }
}
