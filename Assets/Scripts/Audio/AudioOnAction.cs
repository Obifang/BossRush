using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOnAction : MonoBehaviour
{

    [Serializable]
    public struct Audio{
        public string ActionID;
        public string ActionName;
        public AudioClip SoundEffect;
    }
    [SerializeField]
    public List<Audio> SoundEffects;

    public ActionMonitorer ActionMonitorer;
    private Dictionary<string, AudioClip> _sounds;
    private AudioSource _audioSource;
    private void Awake()
    {
        _sounds = new Dictionary<string, AudioClip>();
        foreach (Audio a in SoundEffects) {
            _sounds[a.ActionName] = a.SoundEffect;
            _sounds[a.ActionID] = a.SoundEffect;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ActionMonitorer.Subscribe(PlaySoundByAction, gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    void PlaySoundByAction(GameObject subscriber, string ActionName)
    {
        Debug.Log(ActionName);
        if (_sounds.ContainsKey(ActionName)) {
            _audioSource.PlayOneShot(_sounds[ActionName]);
            Debug.Log("Audio Clip Played");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
