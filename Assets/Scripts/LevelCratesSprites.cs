using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class LevelCratesSprites : ScriptableObject {

    public Sprite greenSpriteNormal;
    public Sprite greenSpriteHover;
    public Sprite greenSpritePressed;
    public Sprite greenSpriteOKNormal;
    public Sprite greenSpriteOKHover;
    public Sprite greenSpriteOKPressed;
    public Sprite noActiveSpriteNormal;
    public Sprite noActiveSpriteHover;
    public Sprite redSpriteNormal;
    public Sprite redSpriteHover;
    public Sprite redSpritePressed;
    public Sprite redSpriteOKNormal;
    public Sprite redSpriteOKHover;
    public Sprite redSpriteOKPressed;
    public Sprite blueSpriteNormal;
    public Sprite blueSpriteHover;
    public Sprite blueSpritePressed;
    public Sprite blueSpriteOKNormal;
    public Sprite blueSpriteOKHover;
    public Sprite blueSpriteOKPressed;
    public Sprite purpleSpriteNormal;
    public Sprite purpleSpriteHover;
    public Sprite purpleSpritePressed;
    public Sprite purpleSpriteOKNormal;
    public Sprite purpleSpriteOKHover;
    public Sprite purpleSpriteOKPressed;
    public Sprite yellowSpriteNormal;
    public Sprite yellowSpriteHover;
    public Sprite yellowSpritePressed;
    public Sprite yellowSpriteOKNormal;
    public Sprite yellowSpriteOKHover;
    public Sprite yellowSpriteOKPressed;
    
    [HideInInspector] public SpriteState spriteState_Disable;
    [HideInInspector] public SpriteState spriteState_Green;
    [HideInInspector] public SpriteState spriteState_Green_Finished;
    [HideInInspector] public SpriteState spriteState_Red;
    [HideInInspector] public SpriteState spriteState_Red_Finished;
    [HideInInspector] public SpriteState spriteState_Yellow;
    [HideInInspector] public SpriteState spriteState_Yellow_Finished;
    [HideInInspector] public SpriteState spriteState_Blue;
    [HideInInspector] public SpriteState spriteState_Blue_Finished;

    private void OnEnable() {

        spriteState_Disable.highlightedSprite = noActiveSpriteHover;
        spriteState_Disable.pressedSprite = noActiveSpriteNormal;
        spriteState_Disable.disabledSprite = noActiveSpriteNormal;

        // J'utilise le SpriteState.disabledSprite pour contenir l'image du bouton en mode Normal.
        spriteState_Green.disabledSprite = greenSpriteNormal;
        spriteState_Green.highlightedSprite = greenSpriteHover;
        spriteState_Green.pressedSprite = greenSpritePressed;

        spriteState_Green_Finished.disabledSprite = greenSpriteOKNormal;
        spriteState_Green_Finished.highlightedSprite = greenSpriteOKHover;
        spriteState_Green_Finished.pressedSprite = greenSpriteOKPressed;

        spriteState_Red.disabledSprite = redSpriteNormal;
        spriteState_Red.highlightedSprite = redSpriteHover;
        spriteState_Red.pressedSprite = redSpritePressed;

        spriteState_Red_Finished.disabledSprite = redSpriteOKNormal;
        spriteState_Red_Finished.highlightedSprite = redSpriteOKHover;
        spriteState_Red_Finished.pressedSprite = redSpriteOKPressed;

        spriteState_Yellow.disabledSprite = yellowSpriteNormal;
        spriteState_Yellow.highlightedSprite = yellowSpriteHover;
        spriteState_Yellow.pressedSprite = yellowSpritePressed;

        spriteState_Yellow_Finished.disabledSprite = yellowSpriteOKNormal;
        spriteState_Yellow_Finished.highlightedSprite = yellowSpriteOKHover;
        spriteState_Yellow_Finished.pressedSprite = yellowSpriteOKPressed;

        spriteState_Blue.disabledSprite = blueSpriteNormal;
        spriteState_Blue.highlightedSprite = blueSpriteHover;
        spriteState_Blue.pressedSprite = blueSpritePressed;

        spriteState_Blue_Finished.disabledSprite = blueSpriteOKNormal;
        spriteState_Blue_Finished.highlightedSprite = blueSpriteOKHover;
        spriteState_Blue_Finished.pressedSprite = blueSpriteOKPressed;


    }


}
