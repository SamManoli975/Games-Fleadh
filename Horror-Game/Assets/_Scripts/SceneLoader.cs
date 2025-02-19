using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        UI_LoadingScreen.instance.Show();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Prevents the scene from auto-loading

        // Wait until the scene is fully loaded
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                // Scene is ready to load, but we wait a bit for smooth transition
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true; // Now activate the scene
            }

            yield return null; // Wait for next frame
        }

        // Hide the loading screen after scene is loaded
        UI_LoadingScreen.instance.Hide();
    }
}
