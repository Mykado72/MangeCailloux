using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]

public class Control_Player : MonoBehaviour
{
    private float scale;
    public float m_Vitesse_Dep;
    private int m_nDirection;
    private Vector3 startPosition;      // position dans le tableau avant mouvement
    private Vector3 realplayerPosition; // position réél (unité Unity) du player
    private Vector3 endPosition;        // position de destination dans le tableau
    private Vector3 vDirection;        // vecteur direction
    private Vector3 oldvDirection;
    private GameObject goWherePlayerWhere;
    private GameObject goWherePlayerIs;
    public GameObject goPlayerIsMoving;
    private GameObject goIsBlockingPlayer;
    private GameObject gobjectToShoot;
    public GameObject gobjectToMove;
    public GameObject gobjectBehind;
    private GameObject goDestination;
    public bool b_IsInAction;
    public bool b_MoveCrate;
    public bool b_Alignment;
    public bool b_IsWalking;
    public bool b_IsHolding;
    public bool b_IsPushing;
    public bool b_IsPulling;
    public bool b_IsShootingIsStart;
    public bool b_IsShootingIsFinished;
    public bool b_IsShooting;
    public float timerShooting;
    private const float shootingDelay = 0.18f;
    private const float shootingAnimTime = 0.35f;
    private Caracteristics myCaracteristics;
    private AudioSource soundPlayer;
    private ParticleSystem myParticuleSystem;
    private bool particulesEmitting; // Le système de particule joue-t-il ?

    int[] m_tabToucheDir= { 0, 0, 0, 0 };
    int[] m_tabToucheAction = { 0, 0 };
    private bool useTactilesControles;
    private ControlesTactiles controlesTactiles;
    private LogicManager logicManager;
//    private OptionManager options;
    public Animator anPlayer;

    void Awake()
    {
        // options = GameObject.Find("Options").GetComponent<OptionManager>();
        anPlayer = GetComponent<Animator>();
        soundPlayer = GameObject.Find("ChibiSound").GetComponent<AudioSource>();
    }

    void Start ()
    {
        scale = 2f;
        vDirection= Vector3.back; // vers le bas
        transform.localEulerAngles = new Vector3(0, 180, 0);
        oldvDirection =vDirection;
        m_nDirection = 2;
        myCaracteristics = GetComponent<Caracteristics>();
        goWherePlayerWhere = myCaracteristics.goBeforeThisObject;
        goWherePlayerIs = myCaracteristics.goBeforeThisObject;
        b_MoveCrate = false;
        b_IsPushing = false;
        b_IsPulling = false;
        b_IsShooting = false;
        b_IsShootingIsStart = false;
        myParticuleSystem = transform.FindChild("P-Chibis-Marche").GetComponent<ParticleSystem>();
        timerShooting = 0;
        if (Application.platform == RuntimePlatform.Android) {
            useTactilesControles = true;
            controlesTactiles = GameObject.Find("Canvas/ControlesTactiles").GetComponent<ControlesTactiles>();
        }
        logicManager = GameObject.Find("Managers").GetComponent<LogicManager>();

    }

    // Update is called once per frame
    void Update()
    {
        //logicManager.b_IsDead = caracteristic.b_IsDead;
        startPosition = OurUtilitysFonctions.RoundIntVector(transform.position, scale);
        if (!logicManager.b_IsDead && !logicManager.b_LevelFinished && !logicManager.b_Pause && Time.timeSinceLevelLoad > 2)
        {
            if (!useTactilesControles) { // La plateforme de jeu n'est pas Android
                KeyUp();
                KeyDown();
            } else { // On est sous android
                m_tabToucheDir = controlesTactiles.GetTabToucheDir();
                m_tabToucheAction = controlesTactiles.GetTabToucheAction();                
            }
            if (b_IsShootingIsStart == true)
            {
                TimeToShoot(); // est-ce que le pied est sufissament en avant pour faire l'action Shoot()
            }
            else
            {
                if (b_MoveCrate == false)
                {
                    b_IsHolding = false;
                    if (TestTouchesAction() == false)
                    {   // si pas d'action de demandée                        
                        if (b_IsShootingIsStart == false) // le joueur n'est pas en train de shooter
                        {
                            if (b_Alignment==false)
                            {
                                TestDirection(true); // on tourne le joueur en fonction de la direction demandée
                                if (vDirection != Vector3.zero)
                                {   // déplacement est demandé
                                    Move();
                                    EmetParticule();
                                    oldvDirection = vDirection;
                                }
                                else
                                {   // pas de déplacement 
                                    b_IsWalking = false;
                                    StopParticule();
                                }
                            }
                        } // le joueur n'est pas en train de shooter
                    }
                }
            }
            TestDirection(false);  // Prend en compte les déplacements demandé sans tourner le joueur
            endPosition = startPosition + oldvDirection;
            TestChangementCase(startPosition, OurUtilitysFonctions.RoundIntVector(transform.position, scale));
            ActionDeplacementCaisse(); 
        }
        else // Le Perso IsDead
        {
            b_IsWalking= false;
            b_IsPushing = false;
            b_IsPulling = false;
            b_IsShooting = false;
            if (logicManager.b_IsDead == true) {
                /*transform.FindChild("Raton").*/gameObject.SetActive(false); // désactive l'affichage du Mesh
            }
        }
        if (logicManager.b_LevelFinished == true)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            anPlayer.SetBool("bGagne", true);
            logicManager.Win();
        }
        TestAnim();
    }

    /// <summary>
    /// Retourn True si une touche action (Main ou Shoot) à été préssée
    /// </summary>
    private bool TestTouchesAction()
    {
        if (m_tabToucheAction[0] != 0)   // Touche Main ?
        {
            b_IsWalking = false;            
            ActionToucheMain();
            return true;
        }  
        else
        {
            b_IsPushing = false;
            b_IsPulling = false;
            b_Alignment = false;
        } 
        if (m_tabToucheAction[1] != 0)  // Touche Shoot ?
        {
            b_IsWalking = false;
            b_IsInAction = true;
            b_IsShootingIsStart = true;
            ActionToucheShoot();
            return true;
        }
        else
        {
            b_IsShooting = false;
            return false;
        }
    }

    void TimeToShoot()
    {
        timerShooting = timerShooting + Time.deltaTime;                  
        if (timerShooting>= shootingDelay)
        {
            if (Movements.ShootObject(gobjectToShoot, oldvDirection)==true)
            {
                soundPlayer.clip = myCaracteristics.shockSound;
                soundPlayer.Play();
                logicManager.DecPushLeft();
                b_IsShooting = true;
            }
            gobjectToShoot = null;
            b_IsShooting = false;
        }
        if (timerShooting >= shootingAnimTime)
        {
            timerShooting = 0;
            b_IsShootingIsFinished = true;
            b_IsShootingIsStart = false;
        }           
        else
        {
            b_IsShootingIsFinished = false;
        }
    }

    void ActionToucheShoot()
    {
        if (OurUtilitysFonctions.useJaggedTab == true)
        {
            gobjectToShoot = (GameObject)Movements.jaggedtabLevel3d[(int)endPosition.x][(int)endPosition.y][(int)endPosition.z];
        }
        else
        {
            gobjectToShoot = (GameObject)Movements.tabLevel3d[(int)endPosition.x, (int)endPosition.y, (int)endPosition.z];
        }         
        if ((Movements.TestPossibleToPush(gobjectToShoot, vDirection) == true)) // peut-on le Shooter/pousser ? 
        {
            Shoot();
        }
        else // rien à shooter
        {
            Shoot();
            b_IsInAction = false;
        }
    }

    void ActionToucheMain()
    {
        if (OurUtilitysFonctions.useJaggedTab == true)
        {
            gobjectToMove = (GameObject)Movements.jaggedtabLevel3d[(int)endPosition.x][(int)endPosition.y][(int)endPosition.z];
        }
        else
        {
            gobjectToMove = (GameObject)Movements.tabLevel3d[(int)endPosition.x, (int)endPosition.y, (int)endPosition.z];
        }
        float distance = Vector3.Magnitude(transform.position - (gobjectToMove.transform.position - oldvDirection * 1.5f));
        if (gobjectToMove != null)
        {
            if (vDirection != Vector3.zero)
            {
                if (vDirection == oldvDirection) // on pousse ?
                {
                    if (distance > 0.1f) // trop loin pour vraiment tenir
                    {
                        b_IsHolding = false;
                        Alignment(gobjectToMove);
                    }
                    else
                    {
                        if ((Movements.TestPossibleToPush(gobjectToMove, vDirection) == true)) // c'est un objet que l'on peut pousser ? 
                        {
                            if (soundPlayer.clip != myCaracteristics.pushSound)
                            {
                                soundPlayer.clip = myCaracteristics.pushSound;
                                soundPlayer.Play();
                            }
                            b_IsHolding = true;
                            Movements.PushObject(gobjectToMove, vDirection);
                            logicManager.DecPushLeft();
                            myCaracteristics.b_must_Move = true;
                            PlayerPush(gobjectToMove, vDirection);
                        }
                        else // l'objet ne peux pas être poussé
                        {
                            b_IsHolding = false;
                            myCaracteristics.b_must_Move = false;
                            if (gobjectToMove.CompareTag("Mur") || (gobjectToMove.GetComponent<Caracteristics>().whatIsIt == Caracteristics.types.Caisse))
                                b_IsPushing = true;
                            else
                            {
                                b_IsPulling = false;
                            }
                        }
                    }
                }
                else // on ne pousse pas 
                {
                    b_IsPushing = false;
                }

                if (vDirection == -oldvDirection)  // on tire ?
                { // on tire                    
                    if (distance >= 0.1f) // trop loin pour vraiment être tenu ou tirer
                    {
                        b_IsHolding = false;
                        Alignment(gobjectToMove);
                    }
                    else // assez pret
                    {
                        if (Movements.TestPossibleToMove(gobjectToMove) == true)
                        {
                            gobjectBehind = Movements.GetNextObject(gameObject, vDirection);
                            if ((Movements.TestPossibleToWalk(gameObject, gobjectBehind) == true)) // de la place derrière nous?
                            {
                                if (soundPlayer.clip != myCaracteristics.slideSound)
                                {
                                    soundPlayer.clip = myCaracteristics.slideSound;
                                    soundPlayer.Play();
                                }
                                b_IsHolding = true;
                                b_Alignment = false;
                                Movements.PushObject(gobjectToMove, vDirection);
                                logicManager.DecPushLeft();
                                myCaracteristics.b_must_Move = true;
                                PlayerPull(gobjectToMove, vDirection);
                            }
                            else  // un obstacle derriere
                            {
                                b_IsHolding = true;
                                myCaracteristics.b_must_Move = false;
                            }
                        } // l'objet n'est pas déplaçable
                        else
                        {
                            b_IsHolding = false;
                        }
                    }
                }
                else // on ne tire pas
                {
                    b_IsPulling = false;
                }
            }
            else // pas de déplacement
            {                
                if ((Movements.TestPossibleToMove(gobjectToMove) == true)) // || (Movements.TestPossibleToPush(gobjectToPush, -oldvDirection) == true))
                {   // une caisse pouvant être déplaçable se trouve devant
                    if (distance >= 0.1f) // un Alignement est nécessaire (
                    {
                        b_IsHolding = true;
                        Alignment(gobjectToMove);
                    }
                    else   // on tient la caisse sans bouger
                    {
                        b_Alignment = false;
                        b_IsHolding = true;
                        b_IsPushing = false;
                        b_IsPulling = false;
                    }
                }
            }
        }
    }

    void Alignment(GameObject go)
    {
        b_Alignment = true;
        // b_IsWalking = true;
        transform.position = Vector3.Lerp(transform.position, (go.transform.position - oldvDirection * (scale / 2 + scale / 4)), Time.deltaTime * m_Vitesse_Dep);
    }

    void PlayerPush(GameObject goPushObject, Vector3 vDirection)
    {
        if (goPushObject != null)
        {
            goPlayerIsMoving = goPushObject;
            transform.parent = goPushObject.transform;
            b_IsInAction = true;
            myCaracteristics.vDestination = (Vector3)OurUtilitysFonctions.RoundIntVector(goPushObject.transform.position, Movements.scale) * Movements.scale;
            transform.position = goPushObject.transform.position - vDirection * (scale / 2 + scale / 4);
            b_IsPushing = true;
            b_IsPulling = false;
        }
    }

    void PlayerPull(GameObject goPullObject, Vector3 vDirection)
    {
        if (goPullObject != null)
        {
            goPlayerIsMoving = goPullObject;
            transform.parent = goPullObject.transform;
            b_IsInAction = true;
            myCaracteristics.vDestination = OurUtilitysFonctions.RoundIntVector(goPullObject.transform.position + vDirection * Movements.scale * 2, Movements.scale) * Movements.scale;
            transform.position = goPullObject.transform.position + vDirection * (scale / 2 + scale / 4);
            b_IsPulling = true;
            b_IsPushing = false;
        }
    }

    void Move()
    {
        // pour éviter les overflow dans le tableau
        endPosition=OurUtilitysFonctions.ClampPosition(endPosition);
        if (OurUtilitysFonctions.useJaggedTab == true)
        {
            goDestination = Movements.jaggedtabLevel3d[(int)endPosition.x][(int)endPosition.y][(int)endPosition.z];
        }
        else
        {
            goDestination = (GameObject)Movements.tabLevel3d[(int)endPosition.x, (int)endPosition.y, (int)endPosition.z];
        }        
        if (Movements.TestPossibleToWalk(gameObject,goDestination) == true)
        {
            transform.position = Vector3.Slerp(transform.position, endPosition * scale, Time.deltaTime);
            transform.Translate(vDirection * Time.deltaTime * m_Vitesse_Dep, Space.World);
            b_IsWalking=true;
        }
        else // un obstacle gène la progression
        {   // on ne peux pas marcher sur le prochain item
            goIsBlockingPlayer = Movements.GetNextObject(gameObject, vDirection);
            if (goIsBlockingPlayer != null)
            {
                transform.position = Vector3.Lerp(transform.position, (goIsBlockingPlayer.transform.position - vDirection * 1.75f), Time.deltaTime * m_Vitesse_Dep);
                b_IsWalking = true;
            }
        }

    }

    private void TestChangementCase(Vector3 beforePosition, Vector3 actualPosition)
    {
        if ((actualPosition - beforePosition) != Vector3.zero) // changement de case
        {
            goWherePlayerWhere = goWherePlayerIs;
            goWherePlayerIs = Movements.tabLevel3d[(int)actualPosition.x, 1, (int)actualPosition.z]; // on sauve l'objet sur lequel le joueur va
            if (OurUtilitysFonctions.useJaggedTab == true)
            {
                Movements.jaggedtabLevel3d[(int)actualPosition.x][1][(int)actualPosition.z] = gameObject;   // on met le joueur à son nouvel emplacement dans le tableau
                Movements.jaggedtabLevel3d[(int)beforePosition.x][1][(int)beforePosition.z] = goWherePlayerWhere; // on remet l'objet qui était à la place du joueur  
            }
            else
            {
                Movements.tabLevel3d[(int)actualPosition.x, 1, (int)actualPosition.z] = gameObject;   // on met le joueur à son nouvel emplacement dans le tableau
                Movements.tabLevel3d[(int)beforePosition.x, 1, (int)beforePosition.z] = goWherePlayerWhere; // on remet l'objet qui était à la place du joueur  
            }
            myCaracteristics.goWhereObjectIs = goWherePlayerIs;
            myCaracteristics.goBeforeThisObject = goWherePlayerWhere;
        } 
        else
        {
            // caracteristic.goBeforeThisObject = Movements.tabLevel3d[(int)beforePosition.x, (int)beforePosition.y, (int)beforePosition.z];
        }          
    }

    private void ActionDeplacementCaisse()
    {
        if (b_IsHolding == true)
        { 
            if (goPlayerIsMoving != null)
            {
                if (goPlayerIsMoving.GetComponent<Caracteristics>().b_must_Move == true)
                {   // l'object que le joueur essai de déplacer peut encore se déplacer.
                    b_IsInAction = true;
                    b_MoveCrate = true;
                    myCaracteristics.b_must_Move = true;
                }
                else // l'object est arrivé à destination donc le joueur ne doit plus être enfant.
                {
                    b_IsInAction = false;
                    b_MoveCrate = false;
                    transform.parent = null;
                    goPlayerIsMoving = null;
                }
            }
            else
            {
                transform.parent = null;
                goPlayerIsMoving = null;
                myCaracteristics.b_must_Move = false;
                b_IsInAction = false;
                b_MoveCrate = false;
            }
        }
    }

    int Recup_Ind_Touche()
    {

        int nInd = 1;
        foreach (int nTouche in m_tabToucheDir)
        {
            if (nTouche != 0 && nInd <= nTouche)
                nInd = nTouche + 1;
        }
        return nInd;
    }

    int Recup_Direction()
    {
        int nInd = 0;
        int nDir = -1;
        int i = 1;
        foreach (int nTouche in m_tabToucheDir)
        {
            if (nTouche > nInd && nTouche != 0)
            {
                nInd = nTouche;
                nDir = i;
            }
            i++;
        }
        return nDir;
    }

    private void TestDirection(bool b_Rotate)
    {
        m_nDirection = Recup_Direction();
        switch (m_nDirection)
        {  // applique les rotations en fonction de la direction
            case 1:   // vers le haut
                vDirection = new Vector3(0, 0, 1);
                if (b_Rotate == true)
                    transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case 2:   // vers le bas
                vDirection = new Vector3(0, 0, -1);
                if (b_Rotate == true)
                    transform.eulerAngles = new Vector3(0, 180, 0);
                break;
            case 3:   // vers la gauche
                vDirection = new Vector3(-1, 0, 0);
                if (b_Rotate == true)
                    transform.eulerAngles = new Vector3(0, 270, 0);
                break;
            case 4:   // vers la droiteAnimPush
                vDirection = new Vector3(1, 0, 0);
                if (b_Rotate == true)
                    transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            default:
                vDirection = new Vector3(0, 0, 0);
                break;
        }   // fin switch
    }

    void KeyUp()
    {
        //Touches Relachées
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            m_tabToucheDir[0] = 0;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            m_tabToucheDir[1] = 0;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            m_tabToucheDir[2] = 0;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            m_tabToucheDir[3] = 0;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            m_tabToucheAction[0] = 0;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            m_tabToucheAction[1] = 0;
        }
    }

    void KeyDown()
    {
        //Touches Enfoncées
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_tabToucheDir[0] = Recup_Ind_Touche();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_tabToucheDir[1] = Recup_Ind_Touche();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            m_tabToucheDir[2] = Recup_Ind_Touche();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_tabToucheDir[3] = Recup_Ind_Touche();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_tabToucheAction[0] = 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            m_tabToucheAction[1] = 1;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Door")
        {
            Gagne(false);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Door")
        {
            transform.eulerAngles = (new Vector3(0, 180, 0));
            Gagne(true);
            if (!logicManager.b_LevelFinished)
            {
                logicManager.b_LevelFinished = true;
            }
        }
    }

    void Gagne(bool bOnOff)
    {
        anPlayer.SetBool("bGagne", bOnOff);
    }

    void Shoot()
    {
        anPlayer.SetBool("bShoot", true);
        myCaracteristics.b_must_Move = false;
        b_IsShooting = true;
        b_IsInAction = true;
    }

    void AnimFin_Shoot()
    {
        anPlayer.SetBool("bShoot", false);
        myCaracteristics.b_must_Move = false;
        b_IsShooting = false;
        b_IsInAction = false;
    }

    void TestAnim()
    {
        if (b_IsShootingIsStart==false)
        {
            anPlayer.SetBool("bShoot", b_IsShootingIsStart);
            anPlayer.SetBool("bWalk", b_IsWalking);
 //           anPlayer.SetBool("bAlignment", b_Alignment);
            anPlayer.SetBool("bHOLD", b_IsHolding);
            anPlayer.SetBool("bPush", b_IsPushing);
            anPlayer.SetBool("bPull", b_IsPulling);
        }
        else
        {
            anPlayer.SetBool("bShoot", b_IsShootingIsStart);
        }
    }

    void EmetParticule()
    {
        // Gestion des particules de la marche de Chibis.
            if (!particulesEmitting)
            {
                myParticuleSystem.Play();
                particulesEmitting = true;
            }
    }

    void StopParticule()
    {
        if (particulesEmitting)
        {
            myParticuleSystem.Stop();
            particulesEmitting = false;
        }
    }
}