using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Manager_Input : MonoBehaviour
{
    public static Manager_Input Instance { get; private set; }

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


    public void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
            PlayerControls = new PlayerInputActions();
            DontDestroyOnLoad(this);
        }
    }
    private void OnEnable()
    {
        if (PlayerControls == null) {
            PlayerControls = new PlayerInputActions();
        }

        _move = PlayerControls.Player.Move;
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
        _pause.Enable();
    }

    private void OnDisable()
    {
        _move.Disable();
        _attack.Disable();
        _jump.Disable();
        _block.Disable();
        _dash.Disable();
        _pause.Disable();
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
