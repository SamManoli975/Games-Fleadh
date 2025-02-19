using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WaitingScreen : MonoBehaviour
{
    [SerializeField] GameObject waitingScreenPanel;
    [SerializeField] Button exitButton;

    void Start()
    {
        if (!GameManager.instance.IsGameRunning())
        {
            Show();
            GameManager.instance.onGameStarted += Hide;
            exitButton.onClick.AddListener(HandleExitButtonClicked);
        }
        else
        {
            Hide();
        }
    }

    void Show()
    {
        waitingScreenPanel.SetActive(true);
    }

    void Hide()
    {
        waitingScreenPanel.SetActive(false);
    }

    void HandleExitButtonClicked()
    {
        GameManager.instance.QuitGame();
    }
}
