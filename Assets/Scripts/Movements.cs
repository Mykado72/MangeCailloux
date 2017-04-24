using UnityEngine;
using System.Collections;

/// <summary>
/// Classe qui regroupe toutes les fonctions concernant les mouvements.
/// </summary>
public static class Movements
{
    public static float scale;
    public static GameObject[,,] tabLevel3d;
    public static GameObject[][][] jaggedtabLevel3d;
    public static GameObject[] listeItems;
    public static GameObject player;

    public static void InitializeDatas(GameObject[,,] tLvl3D, GameObject[] li, GameObject p) {
        tabLevel3d = tLvl3D; // GameObject.Find("Level3D").GetComponent<LoadLevel>().tabLevel3d;
        listeItems = li; // GameObject.Find("Level3D").GetComponent<LoadLevel>().listeItems;
        player = p; // GameObject.FindGameObjectWithTag("Player");
        scale = 2;
    }

    public static void InitializeDatasjagged(GameObject[][][] tLvl3D, int X, int Z)
    {
        jaggedtabLevel3d = tLvl3D; // GameObject.Find("Level3D").GetComponent<LoadLevel>().tabLevel3d;
    }

    public static bool TestPossibleToWalk(GameObject go, GameObject goItem) {
        if (goItem != null) {
            if (go == goItem) {// c'est l'objet qui teste s'il peut marcher sur sa propre position            
                return true;
            } else {
                if (goItem.GetComponent<Caracteristics>().b_CanBeWalkOn == true) {

                    if (go != player) {// c'est un objet qui fait le test
                        if (goItem.CompareTag("Door")) { // les objets ne peuvent passer la porte
                            //Debug.Log("les objets ne peuvent passer la porte");
                            return false;
                        } else { // ce n'est pas une porte.
                            return true;
                        }
                    } else { // c'est le joueur qui test.
                        return true;
                    }
                } else { // ce n'est pas un objet sur lequel on peut marcher dessus
                    return false;
                }
            }
        } else {   // pas d'item du tout
            return true;
        }
    }

    public static bool TestPossibleToMove(GameObject goItem)
    {
        if ((goItem != null) && !goItem.CompareTag("Player"))
        {
            if (goItem.GetComponent<Caracteristics>().b_CanBePush == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public static bool TestPossibleToPush(GameObject goItem, Vector3 vDirection)
    {
        if ((goItem != null) && !goItem.CompareTag("Player"))
        {
            if (goItem.GetComponent<Caracteristics>().b_CanBePush == true)
            {
                Vector3 behindObject = OurUtilitysFonctions.RoundIntVector(goItem.transform.position + vDirection * scale, scale);
                behindObject = OurUtilitysFonctions.ClampPosition(behindObject);
                if (OurUtilitysFonctions.useJaggedTab == true)
                {
                    if (TestPossibleToWalk(goItem, (GameObject)jaggedtabLevel3d[(int)behindObject.x][(int)behindObject.y][(int)behindObject.z]) == true)                     
                        return true;
                    else
                    {
                        // Debug.Log("Peut pas Push "+ goItem+ " a cause de "+ (GameObject)tabLevel3d[(int)behindObject.x, (int)behindObject.y, (int)behindObject.z]+" à "+ behindObject* scale);
                        return false;
                    }
                }
                else
                {
                    if (TestPossibleToWalk(goItem, (GameObject)tabLevel3d[(int)behindObject.x, (int)behindObject.y, (int)behindObject.z]) == true)
                        return true;
                    else
                    {
                        // Debug.Log("Peut pas Push "+ goItem+ " a cause de "+ (GameObject)tabLevel3d[(int)behindObject.x, (int)behindObject.y, (int)behindObject.z]+" à "+ behindObject* scale);
                        return false;
                    }
                }
            }
            else
            {
                return false; // On ne peut pas pousser cet Item
            }
        }
        else
        {   // pas d'item du tout, alors on peux avancer
            return false;
        }
    }

    public static void PushObject(GameObject goItem, Vector3 vDirection)
    {
        if (goItem != null)
        {
            if (!goItem.CompareTag("Player"))
            {
                Caracteristics caracteristics = goItem.GetComponent<Caracteristics>(); // Ne faire que un getComponent au lieu de 4.
                caracteristics.SetToMove(true);
                caracteristics.SetvDirection(vDirection);
                caracteristics.SetvDestination(OurUtilitysFonctions.RoundIntVector(goItem.transform.position + vDirection * scale, scale) * scale);
                caracteristics.b_must_Slide = false;
                // Debug.Log(goItem + " is pushed");
            }
        }
    }

    public static bool ShootObject(GameObject goItem, Vector3 vDirection)
    {
        if (goItem != null) 
        {
            if (!goItem.CompareTag("Player") && (goItem.GetComponent<Caracteristics>().b_must_Move == false)) // empeche de shooter un objet déjà en mouvement
            {
                if (TestPossibleToPush(goItem, vDirection))
                {
                    Caracteristics caracteristics = goItem.GetComponent<Caracteristics>(); // Ne faire que un getComponent au lieu de 4.
                    if (OurUtilitysFonctions.useJaggedTab == true) // A quoi sert cette condition, les deux traitement sont identiques ?
                    {                        
                        caracteristics.SetToMove(true);
                        caracteristics.SetvDirection(vDirection);
                        caracteristics.SetvDestination(OurUtilitysFonctions.RoundIntVector(goItem.transform.position + vDirection * scale, scale) * scale);
                        caracteristics.b_must_Slide = true;
                        return true;
                    }
                    else
                    {
                        caracteristics.SetToMove(true);
                        caracteristics.SetvDirection(vDirection);
                        caracteristics.SetvDestination(OurUtilitysFonctions.RoundIntVector(goItem.transform.position + vDirection * scale, scale) * scale);
                        caracteristics.b_must_Slide = true;
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public static GameObject GetNextObject(GameObject goNextItem, Vector3 vDirection)
    {
        if (goNextItem != null) // &&(goItem.tag !="Player"))
        {
            if (OurUtilitysFonctions.useJaggedTab == true)
            {
                Vector3 nextObject = OurUtilitysFonctions.RoundIntVector(goNextItem.transform.position + vDirection * scale, scale);
                //  nextObject.x = Mathf.Clamp(nextObject.x, 0, jaggedtabLevel3d[0].GetLength(0) - 1);
                //  nextObject.y = Mathf.Clamp(nextObject.y, 0, jaggedtabLevel3d[1].GetLength(1) - 1);
                //  nextObject.z = Mathf.Clamp(nextObject.z, 0, jaggedtabLevel3d[2].GetLength(2) - 1);
                return (GameObject)jaggedtabLevel3d[(int)nextObject.x][(int)nextObject.y][(int)nextObject.z];
            }
            else
            { 
                var nextObject = OurUtilitysFonctions.RoundIntVector(goNextItem.transform.position+ vDirection*scale, scale);
                nextObject.x = Mathf.Clamp(nextObject.x, 0, tabLevel3d.GetLength(0) - 1);
                nextObject.y = Mathf.Clamp(nextObject.y, 0, tabLevel3d.GetLength(1) - 1);
                nextObject.z = Mathf.Clamp(nextObject.z, 0, tabLevel3d.GetLength(2) - 1);
                return (GameObject)tabLevel3d[(int)nextObject.x, (int)nextObject.y, (int)nextObject.z];
            }
        }
        else
            return null;
    }

    public static GameObject GetPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }
}