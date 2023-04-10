using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayState : MonoBehaviour
{
    public GameObject ObjectWithState;
    public IHasState<MovementState> _stateToDisplay;
    public Text TextBox;
    public Controller_Combat CombatController;
    // Start is called before the first frame update
    void Start()
    {
        _stateToDisplay = ObjectWithState.GetComponent<IHasState<MovementState>>();
    }

    // Update is called once per frame
    void Update()
    {
        TextBox.text = "Current State: " + _stateToDisplay.GetCurrentState().ToString();
        TextBox.text += "\nPrevious State: " + _stateToDisplay.GetPreviousState().ToString();
        if (CombatController == null) {
            return;
        }
        TextBox.text += "\nCurrent Damage Reduction: " + CombatController.Defense.ToString();
    }
}
