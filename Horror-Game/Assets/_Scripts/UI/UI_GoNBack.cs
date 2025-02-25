using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class UI_GoNBack : MonoBehaviour
{
    public GameObject joinScreen; // Reference to the JoinScreen UI panel
    public Button clientBtn; // Reference to the Client Button
    public Button backButton; // Reference to the Back Button

    void Start()
    {
        // Ensure the buttons are properly assigned before adding listeners
        if (clientBtn != null)
            clientBtn.onClick.AddListener(ShowJoinScreen);

        if (backButton != null)
            backButton.onClick.AddListener(HideJoinScreen);
    }

    void ShowJoinScreen()
    {
        if (joinScreen != null)
            joinScreen.SetActive(true);
    }

    void HideJoinScreen()
    {
        if (joinScreen != null)
            joinScreen.SetActive(false);
    }
}
