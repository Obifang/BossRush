using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Manager_Input : MonoBehaviour
{
    public static Manager_Input Instance { get; private set; }
    public InputActionAsset inputAsset;
    [HideInInspector]
    public PlayerInputActions PlayerControls { get; private set; }
    [HideInInspector]
    public InputAction _move;
    [HideInInspector]
    public InputAction _attack;
    [HideInInspector]
    public InputAction _jump;
    [HideInInspector]
    public InputAction _block;
    [HideInInspector]
    public InputAction _dash;
    [HideInInspector]
    public InputAction _pause;
    public PlayerInput _playerInput { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
            UpdateKeybinds();
            DontDestroyOnLoad(this);
        }
    }

    public void AddPerformedCallback(string actionName , Action<CallbackContext> action)
    {
        _playerInput.actions[actionName].performed += action;
    }

    public void AddCanceledCallback(string actionName, Action<CallbackContext> action)
    {
        _playerInput.actions[actionName].canceled += action;
    }

    private void OnEnable()
    {
        UpdateKeybinds();

        _playerInput.actions["Attack"].Enable();
        _playerInput.actions["Move"].Enable();
        _playerInput.actions["Dash"].Enable();
        _playerInput.actions["Jump"].Enable();
        _playerInput.actions["Block"].Enable();

        _playerInput.actions["Pause"].Enable();


        /*_move = PlayerControls.Player.Move;
        _move.Enable();
        _attack = PlayerControls.Player.Attack;
        _attack.Enable();
        _jump = PlayerControls.Player.Jump;
        _jump.Enable();
        _block = PlayerControls.Player.Block;
        _block.Enable();
        _dash = PlayerControls.Player.Dash;
        _dash.Enable();

        _pause = PlayerControls.UI.Pause;
        _pause.Enable();*/
    }

    public void UpdateKeybinds()
    {
        //string rebinds = PlayerPrefs.GetString("Player");
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.currentActionMap.Enable();

        //InputActionRebindingExtensions.LoadBindingOverridesFromJson(PlayerControls, rebinds);
    }

    private void OnDisable()
    {
        _playerInput.actions["Attack"].Disable();
        _playerInput.actions["Move"].Disable();
        _playerInput.actions["Dash"].Disable();
        _playerInput.actions["Jump"].Disable();
        _playerInput.actions["Block"].Disable();

        _playerInput.actions["Pause"].Disable();

        /*_move.Disable();
        _attack.Disable();
        _jump.Disable();
        _block.Disable();
        _dash.Disable();
        _pause.Disable();*/
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
