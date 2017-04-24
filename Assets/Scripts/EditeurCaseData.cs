using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Contient les données de la case présente sur la grille et quelques fonctions utiles.
/// </summary>
public class EditeurCaseData : MonoBehaviour {

    /// <summary>
    /// La position en X de la case. X=0 pour le plus à gauche.
    /// </summary>
    public ushort positionX;// { get; private set; }
    /// <summary>
    /// La position en Y de la case. Y=0 pour le plus en haut.
    /// </summary>
    public ushort positionY;// { get; private set; }
    /// <summary>
    /// La case est en bordure.
    /// </summary>
    public bool isBorder; // { get; private set; }
    /// <summary>
    /// Le type d'objet présent sur cette case.
    /// </summary>
    public char type;
    /// <summary>
    /// Le type d'objet présent sur cette case.
    /// </summary>
    public Image myImage;

    /// <summary>
    /// Met à jour les données de la position de la case.
    /// </summary>
    /// <param name="newXpos">La nouvelle position en X</param>
    /// <param name="newYpos">La nouvelle position en Y</param>
    public void SetPostion(ushort newXpos, ushort newYpos) {
        positionX = newXpos;
        positionY = newYpos;
        /*if (positionX == 0 || positionX == LvlEditor.mainLvlEditor.gridSizeX-1) {
            transform.eulerAngles = Vector3.forward * 90;
        } else {
            transform.eulerAngles = Vector3.zero;
        }*/
        transform.eulerAngles = Vector3.zero;
        Active(true); // On active la case car elle doit s'afficher.
    }

    public void SetType(char newType, bool border) {
        if ((type == 'A' || type == 'B' || type == 'C' || type == 'D' || type == 'E') && (newType == '5' || newType == '6' || newType == '7' || newType == '8' || newType == '9' || newType == 'Y')) {

            switch (newType) {
                case '5': // Clic avec une caisse marron.
                    if (type == 'A') {// Sur inter marron
                        newType = 'a';
                    } else if (type == 'B') {// Sur inter bleu
                        newType = 'b';
                    } else if (type == 'C') {// Sur inter orange
                        newType = 'c';
                    } else if (type == 'D') {// Sur inter rouge
                        newType = 'd';
                    } else {// Sur inter vert
                        newType = 'e';
                    }
                    break;
                case '6': // Clic avec une caisse rouge.
                    if (type == 'A') {// Sur inter marron
                        newType = 'f';
                    } else if (type == 'B') {// Sur inter bleu
                        newType = 'g';
                    } else if (type == 'C') {// Sur inter orange
                        newType = 'h';
                    } else if (type == 'D') {// Sur inter rouge
                        newType = 'i';
                    } else {// Sur inter vert
                        newType = 'j';
                    }
                    break;
                case '7': // Clic avec une caisse orange.
                    if (type == 'A') {// Sur inter marron
                        newType = 'k';
                    } else if (type == 'B') {// Sur inter bleu
                        newType = 'l';
                    } else if (type == 'C') {// Sur inter orange
                        newType = 'm';
                    } else if (type == 'D') {// Sur inter rouge
                        newType = 'n';
                    } else {// Sur inter vert
                        newType = 'o';
                    }
                    break;
                case '8': // Clic avec une caisse verte.
                    if (type == 'A') {// Sur inter marron
                        newType = 'p';
                    } else if (type == 'B') {// Sur inter bleu
                        newType = 'q';
                    } else if (type == 'C') {// Sur inter orange
                        newType = 'r';
                    } else if (type == 'D') {// Sur inter rouge
                        newType = 's';
                    } else {// Sur inter vert
                        newType = 't';
                    }
                    break;
                case '9': // Clic avec une caisse bleue.
                    if (type == 'A') {// Sur inter marron
                        newType = 'u';
                    } else if (type == 'B') {// Sur inter bleu
                        newType = 'v';
                    } else if (type == 'C') {// Sur inter orange
                        newType = 'w';
                    } else if (type == 'D') {// Sur inter rouge
                        newType = 'x';
                    } else {// Sur inter vert
                        newType = 'y';
                    }
                    break;
                case 'Y': // Clic avec une caisse non colorée.
                    if (type == 'A') {// Sur inter marron
                        newType = '%';
                    } else if (type == 'B') {// Sur inter bleu
                        newType = '&';
                    } else if (type == 'C') {// Sur inter orange
                        newType = '#';
                    } else if (type == 'D') {// Sur inter rouge
                        newType = '$';
                    } else {// Sur inter vert
                        newType = '!';
                    }
                    break;
                default:
                    break;
            }
        }
        type = newType;
        isBorder = border;
        myImage.sprite = LvlEditor.mainLvlEditor.sprites[OurUtilitysFonctions.CharToInt(type)];
        //print("Type " + type + " Bordure " + isBorder);
    }

    /// <summary>
    /// Appelée lors du clic sur l'image.
    /// </summary>
	public void Click () {
        LvlEditor.mainLvlEditor.ImageGridClick(this);
    }

    public void Active(bool active) {
        if (active) {
            gameObject.SetActive(true);
        } else {
            gameObject.SetActive(false);
        }
    }

}
