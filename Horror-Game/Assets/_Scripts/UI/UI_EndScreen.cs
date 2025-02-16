using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        instance = this;
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
