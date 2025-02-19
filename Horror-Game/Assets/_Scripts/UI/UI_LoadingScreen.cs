using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LoadingScreen : MonoBehaviour
{
    public static UI_LoadingScreen instance;

    [SerializeField] GameObject loadingScreen;

    void Awake()
    {
        if (instance == null)
            instance = this;

        Hide();
    }

    public void Show()
    {
        loadingScreen.SetActive(true);
    }

    public void Hide()
    {
        loadingScreen.SetActive(false);
    }
}
