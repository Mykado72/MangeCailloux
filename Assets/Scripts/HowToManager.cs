using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToManager : MonoBehaviour {
    
    public enum HowToPanel { None = -1, Play, UseUserId, CreateLevel}
    public static HowToManager main; // Singleton.

    [SerializeField]
    private Image imagesPanel;
    private int currentImageID;
    public HowToPanel currentPanelDisplayed { get; private set; }


    [SerializeField]
    private Sprite[] spritesPlay;
    [SerializeField]
    private Sprite[] spritesUseUserId;
    [SerializeField]
    private Sprite[] spritesCreateLevel;

    void Start () {
        ShowHowToPanel(HowToPanel.None, 0); // Cache le canvas.
        if (main == null) {
            main = this;
        } else {
            Debug.LogWarning("Attention, il y a deux canvas HowTo dans la scène, ça ne deverait pas se prosuire !");
        }
	}

    public void IncreaseImageToDisplay() {
        ShowHowToPanel(currentPanelDisplayed, ++currentImageID);
    }
    public void DecreaseImageToDisplay() {
        ShowHowToPanel(currentPanelDisplayed, --currentImageID);
    }
    public void ShowHowToPanel(HowToPanel panel, int imageID) {
        currentPanelDisplayed = panel;
        currentImageID = imageID;
        if (panel == HowToPanel.None || imageID < 0) {
            gameObject.SetActive(false); // Cache le canvas.
        } else if (panel == HowToPanel.Play) {

            if (imageID >= spritesPlay.Length) {
                PlayerPrefs.SetInt("HowToPlayWasRead", 1);
                gameObject.SetActive(false);
            } else {
                imagesPanel.sprite = spritesPlay[imageID];
                gameObject.SetActive(true);
            }

        } else if (panel == HowToPanel.UseUserId) {

            if (imageID >= spritesUseUserId.Length) {
                PlayerPrefs.SetInt("HowUseUserIDWasRead", 1);
                gameObject.SetActive(false);
            } else {
                imagesPanel.sprite = spritesUseUserId[imageID];
                gameObject.SetActive(true);
            }

        } else if (panel == HowToPanel.CreateLevel) {

            if (imageID >= spritesCreateLevel.Length) {
                PlayerPrefs.SetInt("HowHowToCreatLevelWasRead", 1);
                gameObject.SetActive(false);
            } else {
                imagesPanel.sprite = spritesCreateLevel[imageID];
                gameObject.SetActive(true);
            }

        }


    }

}
