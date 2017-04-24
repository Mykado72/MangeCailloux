using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelCrate : MonoBehaviour {

    [SerializeField] private LevelCratesSprites levelCratesSprites;
    
    private Button myButton;
	private Image myImage;
    private string progressFilePath;
    private string displayedName;
    public string realfileName;
    public int starNbToActivate;
    
    public void Initialize () {
        progressFilePath = Application.persistentDataPath + "/Levelprogress";
        displayedName = transform.FindChild("Text").GetComponent<Text>().text;
        myButton = GetComponent<Button> ();
        myImage = GetComponent<Image> ();
        LoadLvlListFile();
    }
    
    // Il faudra centraliser cette fonction
    void LoadLvlListFile() {
        if (File.Exists(progressFilePath)) {
            string[] lvlList = File.ReadAllLines(progressFilePath);
            bool b_find = false;
            string[] infosList = new string[] { realfileName, "O", "0" };
            int line;
            for (line = 0; line < lvlList.Length; line++) {
                string stLine = lvlList[line];
                if (stLine.Contains(realfileName)) {
                    b_find = true;
                    infosList = stLine.Split('|');
                    if (infosList[1].Contains("F")) {
                        //Debug.Log(lvlList[line] +" was finished");
                        MarkLevelFinished();
                    }
                    break;
                }
            }
            if (b_find == false) // le level n'avais jamais été fini
            {
                //Debug.Log(displayedName + " was never finished");
                if (OptionManager.optionsInstance.nbOfLevelFinished >= starNbToActivate) {
                    //Debug.Log("Level "+ displayedName + " is unlocked");
                    Activate();
                } else {
                    Desactivate();
                }
            }
        } else  // le fichier LevelPlayed n'existe pas encore
          {
            if (OptionManager.optionsInstance.nbOfLevelFinished >= starNbToActivate)
                Activate();
            else
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
    /// Active le bouton avec étoile et lui donne la bonne apparence.
    /// </summary>
    void MarkLevelFinished()
    {
        myButton.interactable = true;
        switch (gameObject.tag) {
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
