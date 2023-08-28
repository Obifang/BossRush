using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeTextSlider : MonoBehaviour
{
    [Serializable]
    public enum VolumeTypes
    {
        SoundEffect,
        Music
    }

    public VolumeTypes VolumeType = VolumeTypes.SoundEffect;
    public TextMeshProUGUI numberText;

    private Slider slider;

    public void SetVolumeText()
    {
        if (slider == null) {
            slider = GetComponent<Slider>();
        }
        switch (VolumeType) {
            case VolumeTypes.SoundEffect: {
                    numberText.text = Manager_Audio.Instance.MasterSoundEffectVolume.ToString();
                    slider.value = (Manager_Audio.Instance.MasterSoundEffectVolume);
                    break;
                }
            case VolumeTypes.Music: {
                    numberText.text = Manager_Audio.Instance.MasterMusicVolume.ToString();
                    slider.value = (Manager_Audio.Instance.MasterMusicVolume);
                    Debug.Log((Manager_Audio.Instance.MasterMusicVolume));
                    break;
                }
        }
    }
}
