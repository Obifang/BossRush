using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class Controller_Player : BaseController
{
    Controller_Movement _movement;
    private PlayerInputActions PlayerControls;
    private Vector2 _moveDirection;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _movement = GetComponent<Controller_Movement>();
        PlayerControls = Manager_Input.Instance.PlayerControls;

        

        Manager_Input.Instance.AddPerformedCallback("Attack", Attack);
        Manager_Input.Instance.AddPerformedCallback("Jump", Jump);
        Manager_Input.Instance.AddPerformedCallback("Dash", Dash);
        Manager_Input.Instance.AddPerformedCallback("Block", _ => Block(true));
        Manager_Input.Instance.AddCanceledCallback("Block", _ => Block(false));

       /* Manager_Input.Instance._attack.performed += Attack;
        Manager_Input.Instance._jump.performed += Jump;
        Manager_Input.Instance._dash.performed += Dash;
        Manager_Input.Instance._block.performed += _ => Block(true);
        Manager_Input.Instance._block.canceled += _ => Block(false);*/
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
        /*_moveDirection =  Manager_Input.Instance._move.ReadValue<Vector2>();*/
        _moveDirection = Manager_Input.Instance._playerInput.actions["Move"].ReadValue<Vector2>();
        _vertical = _moveDirection.y;
        var currentState = _movement.GetCurrentState();
        if (currentState != MovementState.WallSlide && currentState != MovementState.WallJump) {
            _horizontal = _moveDirection.x;
            _movement.Move(_horizontal, _vertical);
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        var currentState = _movement.GetCurrentState();
        if (currentState == MovementState.WallSlide || currentState == MovementState.WallJump) {
            _horizontal = -_facingDirection.x;
            _movement.Move(_horizontal, _vertical);
            _movement.Jump();
        }

        if (!_movement.Grounded) {
            if (!_movement.Sliding && _movement.CanDoubleJump) {
                _movement.DoubleJump();
            }

            return;
        }

        if (!_movement.Sliding) {
            _movement.Jump();
        }
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (!_movement.Grounded)
            return;

        _movement.Dash(_facingDirection.x);
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (!_movement.Grounded)
            return;

        _movement.UpdateState(MovementState.PerformingAction);
        _actionHandler.ActivateAction(_facingDirection, 0);
    }

    private void Block(bool value)
    {
        if (!_movement.Grounded)
            return;

        if (value) {
            IsFlipable = false;
            _movement.UpdateState(MovementState.Strafe);
            _actionHandler.ActivateAction(_facingDirection, 1);
        } else if (!value) {
            IsFlipable = true;
            _actionHandler.DeactivateAction(_facingDirection, 1);
            _movement.UpdateState(MovementState.Moving);
        }
    }

    public override void SetActive(bool value)
    {
        this.enabled = value;
    }

    private void OnDestroy()
    {
        Manager_Input.Instance.RemovePerformedCallback("Attack", Attack);
        Manager_Input.Instance.RemovePerformedCallback("Jump", Jump);
        Manager_Input.Instance.RemovePerformedCallback("Dash", Dash);
        Manager_Input.Instance.RemovePerformedCallback("Block", _ => Block(true));
        Manager_Input.Instance.RemoveCanceledCallback("Block", _ => Block(false));
    }
}
