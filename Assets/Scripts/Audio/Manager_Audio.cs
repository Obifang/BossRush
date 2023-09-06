using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using static AudioOnAction;
using static Manager_Audio;

public class Manager_Audio : MonoBehaviour
{
    [Serializable]
    public struct MusicInfo
    {
        public string NameOfMusic;
        public AudioClip MusicClip;
        public bool Loop;
        public bool PlayOnSceneStart;
        public string SceneNameToStart;
    }

    [Serializable]
    public struct AudioInfo
    {
        public string NameOfEffect;
        public AudioClip SoundEffect;
        public bool OverrideAudioSourceSettings;
        [Range(0f, 1f)]
        public float Volume;
        [Range(-3f, 3f)]
        public float Pitch;
        [Range(0f, 1.1f)]
        public float Reverb;
    }
    [Range(0f, 100f)]
    public float MasterSoundEffectVolume = 1;
    [Range(0f, 100f)]
    public float MasterMusicVolume = 1;
    public int NumberOfAudioSourcesInPool;
    [SerializeField]
    public List<AudioInfo> SoundEffects;

    [SerializeField]
    public List<MusicInfo> Music;

    private Dictionary<string, AudioClip> _sounds;
    private Dictionary<string, MusicInfo> _musicInfo;
    private Dictionary<string, string> _musicStart;

    private string _currentSoundEffect;
    private AudioSource _musicAudioSource;
    private Queue<AudioSource> _audioSourcePoolForSoundEffects;
    [Range(0f, 1f)]
    private float _masterSoundEffectVolume;
    [Range(0f, 1f)]
    private float _masterMusicVolume;

    private struct AudioSourceSettings
    {
        float _defaultVolume;
        float _defaultPitch;
        float _defaultReverb;

        public AudioSourceSettings(float Volume, float Pitch, float Reverb)
        {
            _defaultVolume = Volume;
            _defaultPitch = Pitch;
            _defaultReverb = Reverb;
        }

        public float GetVolume {
            get { return _defaultVolume; }
        }

        public float GetPitch {
            get { return _defaultPitch; }
        }

        public float GetReverb {
            get { return _defaultReverb; }
        }
    }

    private Dictionary<AudioSource, AudioSourceSettings> _tempAudioSourceSettings;

    public static Manager_Audio Instance { get; private set; }

    public void Awake()
    {
        
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        _sounds = new Dictionary<string, AudioClip>();
        _tempAudioSourceSettings = new Dictionary<AudioSource, AudioSourceSettings>();
        foreach (AudioInfo a in SoundEffects) {
            _sounds[a.NameOfEffect] = a.SoundEffect;
        }

        _audioSourcePoolForSoundEffects = new Queue<AudioSource>();

        for(int i = 0;i < NumberOfAudioSourcesInPool;i++) {
            _audioSourcePoolForSoundEffects.Enqueue(gameObject.AddComponent<AudioSource>());
        }

        _musicAudioSource = gameObject.AddComponent<AudioSource>();
        _musicStart = new Dictionary<string, string>();
        _musicInfo = new Dictionary<string, MusicInfo>();
        foreach(MusicInfo m in Music) {
            _musicInfo.Add(m.NameOfMusic, m);
            if (m.PlayOnSceneStart)
                _musicStart.Add(m.SceneNameToStart, m.NameOfMusic);
        }

        string currentScene = SceneManager.GetActiveScene().name;
        if (_musicStart.ContainsKey(currentScene))
            PlayMusic(_musicStart[currentScene]);

        AdjustMasterMusicVolume(MasterMusicVolume);
        AdjustMasterSoundEffectsVolume(MasterSoundEffectVolume);
    }

    private void StopAllSoundEffects()
    {
        foreach(AudioSource a in _audioSourcePoolForSoundEffects) {
            a.Stop();
        }
    }

    public void OnSceneStart(Scene scene, LoadSceneMode mode)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        StopAllSoundEffects();
        StopMusic();
        if (_musicStart.ContainsKey(currentScene))
            PlayMusic(_musicStart[currentScene]);
    }

    public void AdjustMasterSoundEffectsVolume(float volume)
    {
        MasterSoundEffectVolume = (volume);
        _masterSoundEffectVolume = (volume / 100f);
    }

    public void AdjustMasterMusicVolume(float volume)
    {
        MasterMusicVolume = (volume);
        _masterMusicVolume = (volume / 100f);
        _musicAudioSource.volume = _masterMusicVolume;
    }

    public void PlayMusic(string name)
    {
        
        _musicAudioSource.clip = _musicInfo[name].MusicClip;
        _musicAudioSource.loop = _musicInfo[name].Loop;
        _musicAudioSource.Play();
        _musicAudioSource.volume = _masterMusicVolume;
    }

    public void PlaySoundEffect(string NameOfEffect)
    {
        if (!_sounds.ContainsKey(NameOfEffect)) {
            return;
        }

        var source = _audioSourcePoolForSoundEffects.Dequeue();
        _audioSourcePoolForSoundEffects.Enqueue(source);

        var soundEffect = SoundEffects.Find(x => x.NameOfEffect == NameOfEffect);
        ResetAudioSourceSettings(source);
        
        _currentSoundEffect = NameOfEffect;
        SaveAudioSourceSettings(source);
        AdjustAudioSourceSettings(soundEffect, source);

        source.PlayOneShot(_sounds[NameOfEffect], _masterSoundEffectVolume * soundEffect.Volume);
    }

    public void StopMusic()
    {
        _musicAudioSource.Stop();
    }

    public void PlaySoundEffectThenMusic(string soundEffect, string music)
    {
        PlaySoundEffect(soundEffect);
        StartCoroutine(WaitForAudioClipPlayMusic(soundEffect, music));
    }

    private IEnumerator WaitForAudioClipPlayMusic(string soundEffect, string music)
    {
        yield return new WaitForSeconds(_sounds[soundEffect].length);
        if (Manager_GameState.Instance.GetCurrentGameState != Manager_GameState.GameState.Playing)
            PlayMusic(music);
    }

    /*public void PlayContinousSoundEffect(string NameOfEffect, AudioSource source)
    {
        if (_sounds.ContainsKey(NameOfEffect)) {
            SaveAudioSourceSettings(source);
            AdjustAudioSourceSettings(SoundEffects.Find(x => x.NameOfEffect == NameOfEffect), source);
            if (source.clip == null)
                source.clip = _sounds[NameOfEffect];
            source.Play();
            Debug.Log("Audio Clip Played");
        }
    }*/

    public void StopContinousSoundEffect(AudioSource source)
    {
        source.Stop();
    }

    void AdjustAudioSourceSettings(AudioInfo settings, AudioSource source)
    {
        if (!settings.OverrideAudioSourceSettings) {
            return;
        }
        source.pitch = settings.Pitch;
        source.reverbZoneMix = settings.Reverb;
        //StartCoroutine(WaitForAudioClipThenReset(settings.NameOfEffect, source));
    }

    private IEnumerator WaitForAudioClipThenReset(string NameOfEffect, AudioSource source)
    {
        yield return new WaitForSeconds(_sounds[NameOfEffect].length);
        ResetAudioSourceSettings(source);
    }

    void SaveAudioSourceSettings(AudioSource source)
    {
        if (!_tempAudioSourceSettings.ContainsKey(source)) {
            _tempAudioSourceSettings.Add(source, new AudioSourceSettings(source.volume, source.pitch, source.reverbZoneMix));
        }
    }

    void ResetAudioSourceSettings(AudioSource source)
    {
        if (!_tempAudioSourceSettings.ContainsKey(source)) {
            return;
        }
        AudioSourceSettings temp = _tempAudioSourceSettings[source];
        source.volume = temp.GetVolume;
        source.pitch = temp.GetPitch;
        source.reverbZoneMix = temp.GetReverb;
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneStart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
