using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WorldCrate : MonoBehaviour {

    [SerializeField]
    private LevelCratesSprites levelCratesSprites;

    private Button myButton;
    private Image myImage;
    private string progressFilePath;
    private string displayedName;
    public int starNbToActivate;

    public void Initialize() {
        myButton = GetComponent<Button>();
        myImage = GetComponent<Image>();
        if (OptionManager.optionsInstance.nbOfLevelFinished >= starNbToActivate) {
            Activate();
        } else {
            Desactivate();
        }
    }

    /// <summary>
    /// Désactive le bouton et lui donne la bonne apparence.
    /// </summary>
    void Desactivate()
    {
        myButton.spriteState = levelCratesSprites.spriteState_Disable;
        myImage.sprite = levelCratesSprites.spriteState_Disable.disabledSprite;
        myButton.interactable = false;
    }

    /// <summary>
    /// Active le bouton (sans étoile) et lui donne la bonne apparence.
    /// </summary>
    void Activate()
    {
        myButton.interactable = true;
        switch (gameObject.tag)
        {
            case "Verte":
                myButton.spriteState = levelCratesSprites.spriteState_Green;
                myImage.sprite = levelCratesSprites.spriteState_Green.disabledSprite;
                break;
            case "Rouge":
                myButton.spriteState = levelCratesSprites.spriteState_Red;
                myImage.sprite = levelCratesSprites.spriteState_Red.disabledSprite;
                break;
            case "Jaune":
                myButton.spriteState = levelCratesSprites.spriteState_Yellow;
                myImage.sprite = levelCratesSprites.spriteState_Yellow.disabledSprite;
                break;
            case "Bleue":
                myButton.spriteState = levelCratesSprites.spriteState_Blue;
                myImage.sprite = levelCratesSprites.spriteState_Blue.disabledSprite;
                break;
            default:
                myButton.spriteState = levelCratesSprites.spriteState_Green;
                myImage.sprite = levelCratesSprites.spriteState_Green.disabledSprite;
                break;
        }
    }
    /// <summary>
    /// Active le bouton avec étoile et lui donne la bonne apparence. TODO, faire que ça fonctionnne pour le monde entier, mais nécessite une centralisation
    /// </summary>
    void MarkLevelFinished()
    {
        myButton.interactable = true;
        switch (gameObject.tag)
        {
            case "Verte":
                myButton.spriteState = levelCratesSprites.spriteState_Green_Finished;
                myImage.sprite = levelCratesSprites.spriteState_Green_Finished.disabledSprite;
                break;
            case "Rouge":
                myButton.spriteState = levelCratesSprites.spriteState_Red_Finished;
                myImage.sprite = levelCratesSprites.spriteState_Red_Finished.disabledSprite;
                break;
            case "Jaune":
                myButton.spriteState = levelCratesSprites.spriteState_Yellow_Finished;
                myImage.sprite = levelCratesSprites.spriteState_Yellow_Finished.disabledSprite;
                break;
            case "Bleue":
                myButton.spriteState = levelCratesSprites.spriteState_Blue_Finished;
                myImage.sprite = levelCratesSprites.spriteState_Blue_Finished.disabledSprite;
                break;
            default:
                myButton.spriteState = levelCratesSprites.spriteState_Green_Finished;
                myImage.sprite = levelCratesSprites.spriteState_Green_Finished.disabledSprite;
                break;
        }
    }
}
