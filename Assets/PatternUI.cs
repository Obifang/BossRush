using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatternUI : MonoBehaviour
{
    public PatternHandler ObjectWithPatternHandler;
    public Text TextBox;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        TextBox.text = "Current Pattern Index: " + ObjectWithPatternHandler.GetCurrentPatternCount;
        TextBox.text += "\nCurrent Action: " + ObjectWithPatternHandler.GetCurrentActionName;
    }
}
