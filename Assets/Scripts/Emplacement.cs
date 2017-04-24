using UnityEngine;
using System.Collections;


public class Emplacement : MonoBehaviour {

    private AudioSource soundPlayer;
    private bool b_stateOK;
    private Control_Player player;
    private LogicManager logicManager;
    private Animator animator;
    private bool b_IsCollected;
    private Caracteristics myCaracteristics;

    void Awake() {
        animator = GetComponent<Animator>();
        myCaracteristics = GetComponent<Caracteristics>();
        b_stateOK = false;
        b_IsCollected = false;

        logicManager = GameObject.Find("Managers").GetComponent<LogicManager>();
    }

    void Start() {
        soundPlayer = GameObject.Find("SpecialSound").GetComponent<AudioSource>(); 
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Control_Player>();
    }
	
    void OnTriggerEnter(Collider col)
    {
        TestEmplacement(col);
    }

    /*void OnTriggerStay(Collider col)
    {
        TestEmplacement(col);
    }*/

    void OnTriggerExit(Collider col) {      
        Caracteristics.types colType = col.GetComponent<Caracteristics>().whatIsIt;
        if (myCaracteristics.whatIsIt == Caracteristics.types.Inter && colType == Caracteristics.types.Caisse) {
            col.GetComponent<Caracteristics>().SetIsWellPlaced(false);
            myCaracteristics.SetIsWellPlaced(false);
            logicManager.CkeckIfAllButtonsActivated();
            if (b_stateOK == true) {
                if (soundPlayer != null) {
                    soundPlayer.PlayOneShot(myCaracteristics.pushSound);
                    b_stateOK = false;                    
                }
            }
        }        
    }

    void TestEmplacement(Collider col)
    {
        if (b_IsCollected == false)
        {
            switch (myCaracteristics.whatIsIt)
            {
                case Caracteristics.types.Inter:
                    if (col.tag == gameObject.tag) // la caisse est de la même couleur que l'inter
                    {
                        //print("HAAAAAAAAAAAAAAAAAAA un GetComponent ! ;-) ");
                        col.GetComponent<Caracteristics>().SetIsWellPlaced(true);
                        myCaracteristics.SetIsWellPlaced(true);
                        logicManager.CkeckIfAllButtonsActivated();
                        if (b_stateOK == false)
                        {
                            if (soundPlayer != null)
                            {
                                soundPlayer.PlayOneShot(myCaracteristics.pushSound);
#if UNITY_ANDROID
                                if (OptionManager.optionsInstance.vibrate) {
                                    Handheld.Vibrate();
                                }
#endif
                            }
                        }
                        b_stateOK = true;
                    }
                    break;
                case Caracteristics.types.Piece:
                    soundPlayer.PlayOneShot(myCaracteristics.pushSound);
                    logicManager.IncPushLeft(5);
                    animator.SetBool("Pick", true);
                    //Debug.Log("Piece " + animator.GetCurrentAnimatorClipInfo(0).Length);
                    Destroy(gameObject, 1);
                    b_IsCollected = true;
                    break;
                case Caracteristics.types.Sac:
                    soundPlayer.PlayOneShot(myCaracteristics.pushSound);
                    logicManager.IncPushLeft(10);
                    animator.SetBool("Pick", true);
                    //Debug.Log("Sac " + animator.GetCurrentAnimatorClipInfo(0).Length);
                    Destroy(gameObject, 1);
                    b_IsCollected = true;
                    break;
                case Caracteristics.types.Marteau:
                    soundPlayer.PlayOneShot(myCaracteristics.pushSound);
                    // logicManager.IncPushLeft(10);
                    animator.SetBool("Pick", true);
                    //Debug.Log("Marteau " + animator.GetCurrentAnimatorClipInfo(0).Length);
                    Destroy(gameObject, 1);
                    b_IsCollected = true;
                    break;
                default:
                    break;
            }
        }
    }

}