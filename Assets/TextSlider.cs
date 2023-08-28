using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextSlider : MonoBehaviour
{
    public TextMeshProUGUI Numbertext;

    public void SetNumberText(float value)
    {
        Numbertext.text = value.ToString();
    }
}
