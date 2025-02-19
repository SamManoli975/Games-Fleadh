using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EndScreenType
{
    none,
    win,
    lose
}

public class UI_EndScreen : MonoBehaviour
{
    public static UI_EndScreen instance;

    [SerializeField] GameObject loseScreen;
    [SerializeField] GameObject winScreen;

    [SerializeField] Button loseExitButton;
    [SerializeField] Button winExitButton;

    void Awake()
    {
        if (instance == null)
            instance = this;


        loseExitButton.onClick.AddListener(ExitCurGame);
        winExitButton.onClick.AddListener(ExitCurGame);

        HideEndScreen();
    }

    void ExitCurGame()
    {
        GameManager.instance.QuitGame();
        HideEndScreen();
    }

    void HideEndScreen()
    {
        loseScreen.SetActive(false);
        winScreen.SetActive(false);
    }

    public void ShowEndScreen(EndScreenType endScreenType)
    {
        loseScreen.SetActive(false);
        winScreen.SetActive(false);

        if (endScreenType == EndScreenType.win)
        {
            winScreen.SetActive(true);
        }
        else if (endScreenType == EndScreenType.lose)
        {
            loseScreen.SetActive(true);
        }
    }
}
