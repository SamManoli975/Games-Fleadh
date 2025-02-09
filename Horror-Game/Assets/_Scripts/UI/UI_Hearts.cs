using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hearts : MonoBehaviour
{
    [SerializeField] Sprite fullHeartImg;
    [SerializeField] Sprite emptyHeartImg;

    [SerializeField] List<Image> heartImages;

    public void HandleHealthUpdated(int newHealth)
    {
        int fullHeartsLeft = newHealth;
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (fullHeartsLeft > 0)
            {
                heartImages[i].sprite = fullHeartImg;
                fullHeartsLeft--;
            }
            else
            {
                heartImages[i].sprite = emptyHeartImg;
            }
        }
    }
}
