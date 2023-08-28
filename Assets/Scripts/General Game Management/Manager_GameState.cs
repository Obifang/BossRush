using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class Manager_GameState : MonoBehaviour
{
    public enum GameState
    {
        Start,
        Playing,
        Paused,
        Gameover,
        Win
    }

    public Controller_Player Player;
    public string MusicToPlayOnWin;
    public string MusicToPlayOnGameover;

    public static Manager_GameState Instance { get; private set; }

    public delegate void GamestateEvent();
    public event GamestateEvent WinCalled;
    public event GamestateEvent GameoverCalled;
    public event GamestateEvent PauseCalled;
    public event GamestateEvent ResumeCalled;
    public event GamestateEvent OnStartCalled;
    public event GamestateEvent GameStateChanged;

    public GameState GetCurrentGameState {
        get => _currentGamestate;
    }

    private GameState _currentGamestate;
    private Health _playerHealth;
    private Controller_Movement _playerMovementController;

    public void Start()
    {
        SceneManager.sceneLoaded += SetupLevel;
        GetPlayerData();
        if (Player != null) {
            ChangeGameState(GameState.Playing);
        }
    }

    public void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void SetupLevel(Scene scene, LoadSceneMode sceneMode)
    {
        ChangeGameState(GameState.Start);
        
    }

    private void GetPlayerData()
    {
        if (Player == null) {
            Player = FindObjectOfType<Controller_Player>();
        }

        if (Player == null) {
            return;
        }

        _playerMovementController = Player.GetComponent<Controller_Movement>();
        _playerHealth = Player.GetComponent<Health>();
        _playerHealth.DeathEvent += OnPlayerDeath;
    }

    public void ChangeGameState(GameState state)
    {
        _currentGamestate = state;
        switch (state) {
            case GameState.Start:
                OnStart();
                GetPlayerData();
                break;
            case GameState.Playing:
                break;
            case GameState.Paused:
                if (_currentGamestate != GameState.Paused) {
                    PauseGame();
                } else {
                    ResumeGame();
                }
                break;
            case GameState.Gameover:
                Gameover();
                break;
            case GameState.Win:
                Win();
                break;
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnStart()
    {
        if (OnStartCalled != null) {
            OnStartCalled.Invoke();
        }

        ChangeGameState(GameState.Playing);
    }

    public void PauseGame()
    {
        _currentGamestate = GameState.Paused;
        if (PauseCalled != null) {
            PauseCalled.Invoke();
        }
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        _currentGamestate = GameState.Playing;
        if (ResumeCalled != null) {
            ResumeCalled.Invoke();
        }
        Time.timeScale = 1;
    }

    public void Gameover()
    {
        _currentGamestate = GameState.Gameover;
        if (GameoverCalled != null) {
            GameoverCalled.Invoke();
        }

        Player.gameObject.SetActive(false);
        Manager_Audio.Instance.StopMusic();
        Manager_Audio.Instance.PlaySoundEffectThenMusic(MusicToPlayOnGameover, MusicToPlayOnGameover);
    }

    private void Win()
    {
        _currentGamestate = GameState.Win;
        if (WinCalled != null) {
            WinCalled.Invoke();
        }

        Player.SetActive(false);
        _playerMovementController.UpdateState(MovementState.Locked);
        Manager_Audio.Instance.StopMusic();
        Manager_Audio.Instance.PlaySoundEffectThenMusic(MusicToPlayOnWin, MusicToPlayOnWin);
    }

    private void OnPlayerDeath()
    {
        ChangeGameState(GameState.Gameover);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log(_currentGamestate);
            if (_currentGamestate == GameState.Paused) {
                ResumeGame();
            } else if (_currentGamestate == GameState.Playing || Player != null) {
                PauseGame();
            }
        }
    }
}
