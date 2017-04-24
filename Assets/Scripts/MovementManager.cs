using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementManager : MonoBehaviour
{
    public static GameObject[] listeItems;
    private AudioSource soundPlayer;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private GameObject goInitial;
    public List<Caracteristics> listMoveableObjectCaracts = new List<Caracteristics>();

    void Start()
    {
        listeItems = GameObject.Find("Level3D").GetComponent<LoadLevel>().listeItems;
        soundPlayer = GameObject.Find("FXPlayer").GetComponent<AudioSource>();
        
    }

    void Update()
    {
        float slideSpeed = 1.0f;
        GameObject goObject;
        for (int i = 0; i < listMoveableObjectCaracts.Count; i++)
        {
            startPosition = OurUtilitysFonctions.RoundIntVector(listMoveableObjectCaracts[i].transform.position, Movements.scale);
            if (listMoveableObjectCaracts[i].b_must_Move == true)
            {
                if ((listMoveableObjectCaracts[i].vDestination.x > listMoveableObjectCaracts[i].transform.position.x) && (listMoveableObjectCaracts[i].vDirection.x < 0) || 
                    (listMoveableObjectCaracts[i].transform.position.x > listMoveableObjectCaracts[i].vDestination.x) && (listMoveableObjectCaracts[i].vDirection.x > 0) ||
                    ((listMoveableObjectCaracts[i].vDestination.z > listMoveableObjectCaracts[i].transform.position.z) && (listMoveableObjectCaracts[i].vDirection.z < 0) || 
                    (listMoveableObjectCaracts[i].transform.position.z > listMoveableObjectCaracts[i].vDestination.z) && (listMoveableObjectCaracts[i].vDirection.z > 0)))
                {   // on dépasse la destination
                    listMoveableObjectCaracts[i].SetToMove(false);
                    if (listMoveableObjectCaracts[i].b_must_Slide == true) // l'objet glissait
                    {
                        listMoveableObjectCaracts[i].b_must_Slide = false;
                        soundPlayer.clip = listMoveableObjectCaracts[i].shockSound;
                        soundPlayer.Play();
                        listMoveableObjectCaracts[i].transform.position = OurUtilitysFonctions.RoundIntVector(listMoveableObjectCaracts[i].vDestination, Movements.scale) * Movements.scale;
                        if (listMoveableObjectCaracts[i].p_Impact.isPlaying != true)
                        {
                            //soundPlayer.Stop();
                            listMoveableObjectCaracts[i].p_Impact.Play();
                            listMoveableObjectCaracts[i].p_Shoote.Stop();
                            listMoveableObjectCaracts[i].p_Pousse.Stop();
                        }                            
                    }
                    else // c'était un push
                    {
                        if (listMoveableObjectCaracts[i].p_Pousse.isPlaying == true)
                        {
                            //soundPlayer.Stop();
                            listMoveableObjectCaracts[i].p_Impact.Stop();
                            listMoveableObjectCaracts[i].p_Shoote.Stop();
                            // listMoveableObjectCaracts[i].p_Pousse.Stop();
                        }
                    }
                }
                else // pas de dépassement de destination
                {
                    goObject = listMoveableObjectCaracts[i].gameObject;
                    if (listMoveableObjectCaracts[i].b_CanSlide || listMoveableObjectCaracts[i].b_must_Slide)
                    {   // l'object peux ou doit glisser ?  
                        slideSpeed = listMoveableObjectCaracts[i].moveSpeed * 5;
                        if (listMoveableObjectCaracts[i] != null)
                        {
                            var NextObject = Movements.GetNextObject(goObject, listMoveableObjectCaracts[i].vDirection);
                            if (Movements.TestPossibleToWalk(goObject, NextObject))
                            {   // on peut continuer alors on défini la nouvelle destination
                                if (NextObject != null)
                                {
                                    listMoveableObjectCaracts[i].vDestination = OurUtilitysFonctions.RoundIntVector(NextObject.transform.position + listMoveableObjectCaracts[i].vDirection * Movements.scale, Movements.scale) * Movements.scale;
                                    soundPlayer.clip = listMoveableObjectCaracts[i].slideSound;
                                    if (soundPlayer.isPlaying != true)
                                    {
                                        listMoveableObjectCaracts[i].p_Shoote.Play();
                                        soundPlayer.Play();
                                        #if UNITY_ANDROID
                                        if (OptionManager.optionsInstance.vibrate) {
                                            Handheld.Vibrate();
                                        }
                                        #endif
                                    }
                                }
                            }
                            else // on n'avancera pas plus loin que la position exacte de la case dans laquel on entre.
                            {
                               // Debug.Log(goObject + " ne peut plus avancer à cause de " + NextObject);
                                listMoveableObjectCaracts[i].vDestination = OurUtilitysFonctions.RoundIntVector(goObject.transform.position, Movements.scale) * Movements.scale;
                            }
                        }  // fin (objectCaract!=null)
                    }      // fin object qui doit ou peux glisser
                    else  // c'est juste un Push donc on avance de vDirection
                    {
                        // Gestion des particules                             

                     
                        slideSpeed = listMoveableObjectCaracts[i].moveSpeed;
                        soundPlayer.clip = listMoveableObjectCaracts[i].pushSound;
                        if (soundPlayer.isPlaying != true)
                        {
                            listMoveableObjectCaracts[i].p_Pousse.Play();
                            soundPlayer.Play();
                            // Handheld.Vibrate();
                        }                            
                    }
                    goObject.transform.Translate(listMoveableObjectCaracts[i].vDirection * slideSpeed * Time.deltaTime);
                }   // fin pas de depassement
                Vector3 actualPosition = (OurUtilitysFonctions.RoundIntVector(listMoveableObjectCaracts[i].transform.position, Movements.scale));
                if ((actualPosition - startPosition) != Vector3.zero) // changement de case ?
                {
                    actualPosition= OurUtilitysFonctions.ClampPosition(actualPosition);
                    startPosition= OurUtilitysFonctions.ClampPosition(startPosition);
                    listMoveableObjectCaracts[i].goBeforeThisObject = listMoveableObjectCaracts[i].goWhereObjectIs;
                    listMoveableObjectCaracts[i].goWhereObjectIs = Movements.tabLevel3d[(int)actualPosition.x, (int)actualPosition.y, (int)actualPosition.z]; // on sauve l'objet sur lequel le joueur va
                    Movements.tabLevel3d[(int)actualPosition.x, (int)actualPosition.y, (int)actualPosition.z] = listMoveableObjectCaracts[i].gameObject;   // on met l'objet à cet emplacement
                    Movements.tabLevel3d[(int)startPosition.x, (int)startPosition.y, (int)startPosition.z] = listMoveableObjectCaracts[i].goBeforeThisObject; // on remet l'ancien object à la place d'avant.                   
                }
            }
            else  // l'objet ne doit plus être en mouvement
            {
                if (listMoveableObjectCaracts[i].p_Pousse.isPlaying==true)
                {
                    listMoveableObjectCaracts[i].p_Pousse.Stop();
                    listMoveableObjectCaracts[i].p_Shoote.Stop();
                }
                    
            }
        }
    }
}