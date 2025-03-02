using UnityEngine;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button resumeButton;
    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        else
            Debug.LogError("PauseMenuUI not assigned in Inspector!");

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        else
            Debug.LogError("Quit Button not assigned in Inspector!");

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        else
            Debug.LogError("Resume Button not assigned in Inspector!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);

        if (isPaused)
        {
            UnlockCursor();     
        }
        else
        {
            LockCursor();
        }

        Debug.Log("Pause Menu Toggled: " + isPaused);
    }

    void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        LockCursor();
        Debug.Log("Game Resumed");
    }

    void QuitGame()
    {
        if (!isPaused) return;

        Debug.Log("Quit button clicked!");

        if (GameManager.instance != null)
        {
            GameManager.instance.QuitGame();
        }
        else
        {
            Debug.LogError("GameManager instance is null! Ensure GameManager is in the scene.");
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}