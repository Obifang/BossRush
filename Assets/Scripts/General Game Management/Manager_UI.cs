using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Manager_GameState;

public class Manager_UI : MonoBehaviour
{
    [Header("General UI")]
    public GameObject LevelCompletionUI;
    public GameObject DeathUI;
    public GameObject WinUI;
    public GameObject PauseMenu;
    public GameObject InGameUI;
    public GameObject OptionsUI;
    [Header("In Game UI")]
    public HealthUI PlayerHealthUI;
    public HealthUI EnemyHealthUI;
    public StaminaUI PlayerStaminaUI;
    public DashUI PlayerDashUI;

    public static Manager_UI Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        DeathUI.SetActive(false);
        WinUI.SetActive(false);
        PauseMenu.SetActive(false);
        InGameUI.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Manager_GameState.Instance != null) {
            Manager_GameState.Instance.GameoverCalled += EnableGameoverUI;
            Manager_GameState.Instance.WinCalled += EnableWinUI;
            Manager_GameState.Instance.PauseCalled += EnablePauseMenu;
            Manager_GameState.Instance.ResumeCalled += DisablePauseMenu;
            Manager_GameState.Instance.ResumeCalled += DisableOptionsMenu;
            Manager_GameState.Instance.OnStartCalled += OnSceneChange;
            
        }

        if (Manager_GameState.Instance.Player != null) {
            EnableInGameUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResumeGame()
    {
        Manager_GameState.Instance.ResumeGame();    
    }

    private void SetupInGameUIReferences()
    {
        PlayerHealthUI.HealthObject = Manager_GameState.Instance.Player.GetComponent<Health>();
        EnemyHealthUI.HealthObject = FindObjectOfType<Controller_AI_Boss>().GetComponent<Health>();
        PlayerStaminaUI.StaminaObject = Manager_GameState.Instance.Player.GetComponent<Stamina>();

        PlayerHealthUI.Setup();
        EnemyHealthUI.Setup();
        PlayerStaminaUI.Setup();
    }

    private void OnSceneChange()
    {
        var player = Manager_GameState.Instance.Player;
        
        if (player != null && player.isActiveAndEnabled) {
            EnableInGameUI();
            DeathUI.SetActive(false);
            WinUI.SetActive(false);
            PauseMenu.SetActive(false);
        } else {
            InGameUI.SetActive(false);
        }
    }

    private void EnableInGameUI()
    {
        InGameUI.SetActive(true);
        SetupInGameUIReferences();
    }

    private void EnablePauseMenu()
    {
        PauseMenu.SetActive(true);
    }

    private void DisablePauseMenu()
    {
        PauseMenu.SetActive(false);
    }

    private void DisableOptionsMenu()
    {
        OptionsUI.SetActive(false);
    }

    private void EnableGameoverUI()
    {
        EnableLevelCompletionUI();
        DeathUI.SetActive(true);
    }

    private void EnableWinUI()
    {
        EnableLevelCompletionUI();
        WinUI.SetActive(true);
    }

    private void EnableLevelCompletionUI()
    {
        LevelCompletionUI.SetActive(true);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeScene(int sceneNo)
    {
        SceneManager.LoadScene(sceneNo);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
