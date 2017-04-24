using UnityEngine;
using System.Collections;

public class Caracteristics : MonoBehaviour {

    public AudioClip pushSound;
    public AudioClip slideSound;
    public AudioClip shockSound;
    public float moveSpeed = 10f;
    public bool b_CanBeWalkOn = true; 
    public bool b_CanBePush = false;
    public bool b_CanSlide = false;
    public bool b_must_Move = false;
    public bool b_must_Slide = false;
    //public bool b_IsPlayer = false;
    //public bool b_Isinter = false;
    public enum types { Sol, Player, Inter, Caisse, Mur, Piece, Door, Sac, Marteau, Other, CaisseSurInterMarron, CaisseSurInterRouge, CaisseSurInterJaune, CaisseSurInterVert, CaisseSurInterBleu };
    public types whatIsIt;
    //public bool b_IsDead = false;
    [HideInInspector] public bool b_IsWellPlaced;
    [HideInInspector] public Vector3 vDestination;
    private Vector3 startPosition;
    private Vector3 endPosition;
    [HideInInspector] public Vector3 vDirection;
    public ParticleSystem p_Impact;
    public ParticleSystem p_Pousse;
    public ParticleSystem p_Shoote;
    public GameObject goBeforeThisObject;
    public GameObject goWhereObjectIs;
    public Mesh meshOn;
    public Mesh meshOff;

    /*void Start()
    {
      // Une fonction Start(), même vide sera appelée et prendra quelques nanosecondes de chargmenet supplérmentaire ;-)
    }*/

    public void SetToMove (bool state)
    {
        b_must_Move = state;
    }

    public void SetvDestination(Vector3 destination)
    {
        vDestination = destination;
    }
    public void SetvDirection(Vector3 direction)
    {
        vDirection = direction;
    }
    /*public void SetvIsDead(bool dead)
    {
        b_IsDead = dead;
    }*/

    /// <summary>
    /// Pour les caisses et les interrupteurs, permet de signaler que 
    /// </summary>
    /// <param name="state">L'objet doit mettre le mesh "allumer"</param>
    public void SetIsWellPlaced(bool state)
    {
        b_IsWellPlaced = state;
        if (whatIsIt == types.Caisse || whatIsIt == types.Inter) {
            if (state) {
                GetComponent<MeshFilter>().mesh = meshOn;
            } else {
                GetComponent<MeshFilter>().mesh = meshOff;
            }
        }

    }
}