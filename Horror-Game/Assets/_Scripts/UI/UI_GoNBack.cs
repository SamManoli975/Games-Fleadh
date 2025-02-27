using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class UI_GoNBack : MonoBehaviour
{
    public GameObject joinScreen; 
    public Button clientBtn; 
    public Button backButton; 

    void Start()
    {
       
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
