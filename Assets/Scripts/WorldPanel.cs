using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldPanel : MonoBehaviour {
    
    [SerializeField] private Button[] niveaux;
    [SerializeField] private Button[] niveauxBonus;


    public void SetLevelsActive(int t, SpriteState active, SpriteState finished, SpriteState disable) {



        for (int i = 0; i<niveaux.Length; i++) {
            niveaux[i].interactable = i < t+1;
            if (i < t) {
                niveaux[i].GetComponent<Image>().sprite = finished.disabledSprite;
                niveaux[i].spriteState = finished;
            } else if (i == t) {
                niveaux[i].GetComponent<Image>().sprite = active.disabledSprite;
                niveaux[i].spriteState = active;
            } else {
                niveaux[i].GetComponent<Image>().sprite = disable.disabledSprite;
                niveaux[i].spriteState = disable;
            }
        }
        
        niveauxBonus[0].interactable = (t >= 5);
        niveauxBonus[1].interactable = (t >= 10);
    }


}
