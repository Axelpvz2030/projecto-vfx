using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject startMenu;
    public GameObject gameOverMenu;
    public GameObject controlsMenu;
    public GameObject winMenu;
    public GameObject hudPanel;

    private static bool autoStartGame = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (autoStartGame)
        {
            autoStartGame = false; 
            StartGame(); 
        }
        else
        {
            ShowMainMenu(); 
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }

    public void ShowMainMenu()
    {
        Time.timeScale = 0f;
        startMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        controlsMenu.SetActive(false);
        winMenu.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(false);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        startMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        controlsMenu.SetActive(false);
        winMenu.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
    }

    public void ShowControls()
    {
        startMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void HideControls() 
    {
        controlsMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void PlayerDied()
    {
        Time.timeScale = 0f;
        if (hudPanel != null) hudPanel.SetActive(false);
        gameOverMenu.SetActive(true);
    }

    public void BossDied()
    {
        Time.timeScale = 0f;
        if (hudPanel != null) hudPanel.SetActive(false);
        winMenu.SetActive(true);
    }

    public void RestartGame()
    {
        autoStartGame = true; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        autoStartGame = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}