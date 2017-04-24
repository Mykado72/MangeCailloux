using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgression : MonoBehaviour {

    [SerializeField] private LevelCratesSprites levelCratesSprites; // Premet d'avoir accès à toutes les images de caisses.
    [SerializeField] private PanelSlide[] worldsSlidesPanels;
    [SerializeField] private WorldPanel[] worldsPanels;
    [SerializeField] private Button[] worldSmallButton; // Les boutons pour switch entre les différents mondes.
    public int currentWorldDisplayed;
    public int progress; // Le nombre de niveau réussi (sans compter les niveau bonus)

    [SerializeField]
    private LevelCrate[] allLevelCreates;
    [SerializeField]
    private WorldCrate[] allWorldCreates;
    
    public void TempSetupList()  { // Initialise la liste
        
        for (int i = 0; i < allLevelCreates.Length; i++) {
            allLevelCreates[i].Initialize();
        }
        for (int i = 0; i < allWorldCreates.Length; i++) {
            allWorldCreates[i].Initialize();
        }
        if (OptionManager.optionsInstance.currentWorld == -1) { // Premier lancement du menu.
            ShowPanel(OptionManager.optionsInstance.nbOfLevelFinished / 10);
        } else {
            ShowPanel(OptionManager.optionsInstance.currentWorld);
        }


    }

    public void SetupList() {
        currentWorldDisplayed = (progress - (progress%10)) / 10;

        for (int i = 0; i < worldsPanels.Length; i++) {

            if (i < currentWorldDisplayed) {
                worldsSlidesPanels[i].Move(-Vector2.right * 1920, -Vector2.right * 1920);
                worldsPanels[i].SetLevelsActive(10, SpriteStateActiveFromWorld(i), SpriteStateFinishedFromWorld(i), levelCratesSprites.spriteState_Disable);
                worldSmallButton[i].spriteState = SpriteStateFinishedFromWorld(i);
                worldSmallButton[i].GetComponent<Image>().sprite = SpriteStateFinishedFromWorld(i).disabledSprite;
            } else if (i == currentWorldDisplayed) {
                worldsSlidesPanels[i].Move(Vector2.zero, Vector2.zero);
                worldsPanels[i].SetLevelsActive((progress % 10), SpriteStateActiveFromWorld(i), SpriteStateFinishedFromWorld(i), levelCratesSprites.spriteState_Disable);
                worldSmallButton[i].spriteState = SpriteStateActiveFromWorld(i);
                worldSmallButton[i].GetComponent<Image>().sprite = SpriteStateActiveFromWorld(i).disabledSprite;
            } else {
                worldsSlidesPanels[i].Move(Vector2.right * 1920, Vector2.right * 1920);
                worldsPanels[i].SetLevelsActive(-1, SpriteStateActiveFromWorld(i), SpriteStateFinishedFromWorld(i), levelCratesSprites.spriteState_Disable);
                worldSmallButton[i].spriteState = levelCratesSprites.spriteState_Disable;
                worldSmallButton[i].GetComponent<Image>().sprite = levelCratesSprites.spriteState_Disable.disabledSprite;
            }

        }
        

    }

    public void ShowPanel(int indexToShow) {
        OptionManager.optionsInstance.currentWorld = indexToShow; // Met en mémoire le dernier panel affiché.
        if (indexToShow == currentWorldDisplayed) {
            return;
        }
        if (indexToShow > currentWorldDisplayed) {
            worldsSlidesPanels[indexToShow].Move(Vector2.right * 1920, Vector2.zero);
            worldsSlidesPanels[currentWorldDisplayed].Move( Vector2.zero, -Vector2.right * 1920);
        } else {
            worldsSlidesPanels[indexToShow].Move(-Vector2.right * 1920, Vector2.zero);
            worldsSlidesPanels[currentWorldDisplayed].Move(Vector2.zero, Vector2.right * 1920);
        }
        currentWorldDisplayed = indexToShow;
    }


    private SpriteState SpriteStateActiveFromWorld(int worldID) {
        switch (worldID) {
            case 0:
                return levelCratesSprites.spriteState_Green;
            case 1:
                return levelCratesSprites.spriteState_Blue;
            case 2:
                return levelCratesSprites.spriteState_Red;
            default:
                return levelCratesSprites.spriteState_Green;
        }
    }
    private SpriteState SpriteStateFinishedFromWorld(int worldID) {
        switch (worldID) {
            case 0:
                return levelCratesSprites.spriteState_Green_Finished;
            case 1:
                return levelCratesSprites.spriteState_Blue_Finished;
            case 2:
                return levelCratesSprites.spriteState_Red_Finished;
            default:
                return levelCratesSprites.spriteState_Green_Finished;
        }
    }

}
