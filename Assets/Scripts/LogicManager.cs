using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System;

[RequireComponent(typeof(Text))]

public class LogicManager : MonoBehaviour {

    public List<Caracteristics> listInter = new List<Caracteristics>();
    public Light ambiantelight;
    private AudioSource specialSound;
    private AudioSource musicAudio;
    private AudioSource fxAudio;
    public AudioClip victorySound;
    public AudioClip loseSound;
    private bool b_victorySound;
    private bool b_loseSound;
    private Caracteristics porteSortieCaracteristics;
    private Caracteristics playerCaracteristics;
    private Animator animPorteSortie;
    private int nbInterOK = 0;
    private bool b_DoorIsOpen;
    public bool b_LevelFinished;
    public bool b_IsDead;
    public bool b_Pause;
    public Text textPushLeftValue;
    public Text textPushLeftText;
    public GameObject panelWin;
    public GameObject panelLose;
    public GameObject ButtonSend;
//    public GameObject chibiLosePrefab;
    public GameObject chibiLoseScene;
//    public GameObject chibiWinPrefab;
    public GameObject chibiWinScene;
    private OptionManager options;
    private Internet internet;
    private SkyboxManager skyboxManager;
    public GameObject butttonrestart;
    private GameObject canvasControlesTactiles;
    public GameObject canvasExit;
    public GameObject level3D;
    private string path;

    // Use this for initialization
    void Awake()
    {
        path = Application.persistentDataPath + "//";
        internet = Internet.internetInstance;     
    }

    void Start () {
        b_Pause = false;
        b_LevelFinished = false;
        b_IsDead = false;
        canvasExit.SetActive(false);
        playerCaracteristics = GameObject.FindGameObjectWithTag("Player").GetComponent<Caracteristics>();
        options = OptionManager.optionsInstance;
        skyboxManager = GetComponent<SkyboxManager>();
        // introTime = 6.0f;
        Init();
        CkeckIfAllButtonsActivated();
    }

    // Update is called once per frame
    void Update()
    {
        if ((options.levelInTestMinimalPush == false) & (options.pushLeft < 0))
        {
            options.pushLeft = 0;
            textPushLeftValue.text = "0";
            Lose();
        }
        if (b_LevelFinished == true)
        {
            Win();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !b_IsDead && !b_LevelFinished)
        {
            Pause();
        }
        if ((b_DoorIsOpen==true) && (animPorteSortie.GetCurrentAnimatorStateInfo(0).IsName("Open")))
        {
            porteSortieCaracteristics.b_CanBeWalkOn = true; // Permet d'aller dans la porte.          
        }
    }

    public void CkeckIfAllButtonsActivated() {
        nbInterOK = 0;
        for (int i = 0; i < listInter.Count; i++) {
            if (listInter[i].b_IsWellPlaced) {
                nbInterOK++;
            }
        }
        if (nbInterOK >= listInter.Count) { // Tous les interrupteurs sont actifs
            animPorteSortie.SetBool("bOpening", true);
            if (!b_DoorIsOpen)
            { // La porte n'était pas ouverte                
                b_DoorIsOpen = true;
                specialSound.PlayOneShot(porteSortieCaracteristics.pushSound);
#if UNITY_ANDROID
                Vibrate();
#endif                
            }
        } else { // Tous les interrupteurs ne sont pas activés.
            animPorteSortie.SetBool("bOpening", false);
            if (b_DoorIsOpen) { // La porte était ouverte
                specialSound.PlayOneShot(porteSortieCaracteristics.pushSound);
                b_DoorIsOpen = false;
            }
            porteSortieCaracteristics.b_CanBeWalkOn = false;
        }
    }

    public void Pause()
    {
        b_Pause = true;
        canvasExit.SetActive(true);
    }

    public void ExitPause()
    {
        b_Pause = false;
        canvasExit.SetActive(false);
    }

    public void Exit()
    { 
        if (options.levelInTestBeforeUpload)
        { // C'est un niveau fait par le joueur, en test depuis l'éditeur
            SceneManager.LoadScene("LvlEditor");
        }
        else if (options.levelIsCustomDownloaded)
        { // C'est un niveau fait par un autre joueur
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                SceneManager.LoadSceneAsync("WebGLMainMenu");
            else
                SceneManager.LoadSceneAsync("MainMenu");
        }
        else
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                SceneManager.LoadSceneAsync("WebGLMainMenu");
            else
                SceneManager.LoadSceneAsync("MainMenu");
        }
    }

    public void DecPushLeft()
    {
        if (options.levelInTestMinimalPush == true)
            options.pushLeft++;
        else
            options.pushLeft--;
        textPushLeftValue.text = options.pushLeft.ToString();
    }

    public void IncPushLeft(int nb)
    {
        if (options.levelInTestMinimalPush == true) {
            options.pushLeft = options.pushLeft - nb;
        } else {
            options.pushLeft = options.pushLeft + nb;
        }
        textPushLeftValue.text = options.pushLeft.ToString();
    }

    public void Win()
    {
        if (canvasControlesTactiles != null) {
            canvasControlesTactiles.SetActive(false);
        }
        butttonrestart.SetActive(false);
        musicAudio.Stop();
        if (b_victorySound== false)
        {
            specialSound.clip=victorySound;
            specialSound.Play();
            b_victorySound = true;
#if UNITY_ANDROID
            Vibrate();
#endif
            // specialSound.volume = 0.7f;
            /* GameObject chibiLoseIntancied = Instantiate(chibiWinPrefab);
            chibiLoseIntancied.transform.position = GameObject.Find("Player").transform.position + new Vector3(0, 3.0f, 0); */
            level3D.SetActive(false);
            chibiWinScene.gameObject.SetActive(true);
            CameraBehaviors.mainCameraInstance.Win();
            skyboxManager.SetSkybox(1); // Met la skybox de victoire.
            ambiantelight.gameObject.SetActive(false);
            if (GameObject.FindGameObjectWithTag("Player") != null) {
                GameObject.FindGameObjectWithTag("Player").SetActive(false);
            }
            if (options.levelInTestBeforeUpload == false) { // C'est un niveau fait par nous.
                MarkLevelFinished(options.nameLevelToLoad);
            }
        }

        if ((internet.bIsUploading == false) && (internet.bFinished == false) && (internet.bError == false)) // on a gagné mais pas encore cliqué sur un bouton
        {
            if (options.levelIsCustom == false) // C'est un niveau d'origine.
                ButtonSend.SetActive(false);   // on n'affiche pas le bouton Send
            else // C'est un niveau fait par le joueur.
            {
                if ((options.levelInTestBeforeUpload == true)&&(options.username!=""))
                    ButtonSend.SetActive(true);  // on affiche le bouton Send
                else
                    ButtonSend.SetActive(false);  // on n'affiche pas le bouton Send
            }
            panelWin.SetActive(true);
        } 
        else // on est en train d'uploader, on cache le canvas Win pour faire apparaitre celui d'attente
        {
            panelWin.SetActive(false);
            if (internet.bFinished == true)
            {
            }
        }
    }

    public void Lose()
    {
        if (canvasControlesTactiles != null)
            canvasControlesTactiles.SetActive(false);
        musicAudio.Stop();
        if (b_victorySound == false)
        {
            specialSound.clip=loseSound;
            specialSound.Play();
            b_victorySound = true;
#if UNITY_ANDROID // Code qui compile que pour android
            Vibrate();
#endif
            skyboxManager.SetSkybox(2); // Met la skybox de défaite.
        }
        butttonrestart.SetActive(false);
        panelLose.SetActive(true);
        level3D.SetActive(false);
        /*GameObject chibiLoseIntancied = Instantiate(chibiLosePrefab);
        chibiLoseIntancied.transform.position = GameObject.Find("Player").transform.position+ new Vector3(0,0.05f,0); */
        CameraBehaviors.mainCameraInstance.Lose();

        chibiLoseScene.gameObject.SetActive(true);
        ambiantelight.gameObject.SetActive(false);
        b_IsDead = true;
        //playerCaracteristics.b_IsDead = true;
        if (GameObject.FindGameObjectWithTag("Player") != null)
            GameObject.FindGameObjectWithTag("Player").SetActive(false);
    }
    
    public void RetryLevel()
    {
        // SceneManager.LoadScene("Loading");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Init()
    {
        textPushLeftValue = GameObject.Find("PushLefValue").GetComponent<Text>();
        b_victorySound = false;
        b_loseSound = false;
        porteSortieCaracteristics = GameObject.FindGameObjectWithTag("Door").GetComponent<Caracteristics>();
        animPorteSortie = porteSortieCaracteristics.GetComponent<Animator>();
        animPorteSortie.SetBool("bOpening", false);
        specialSound = GameObject.Find("SpecialSound").GetComponent<AudioSource>();
        musicAudio = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
        fxAudio = GameObject.Find("FXPlayer").GetComponent<AudioSource>();
        specialSound.volume = PlayerPrefs.GetFloat("FXVol", specialSound.volume);
        musicAudio.volume = PlayerPrefs.GetFloat("MusicVol", musicAudio.volume);
        fxAudio.volume = PlayerPrefs.GetFloat("FXVol", fxAudio.volume);
        panelWin.SetActive(false);
        panelLose.SetActive(false);
        if (internet.panelUpload!= null)
            internet.panelUpload.SetActive(false);
        if (internet.textError != null)
            internet.ErrorClear();
        if (internet.textInfo != null)
            internet.InfoClear();
        internet.bIsUploading = false;
        internet.bFinished = false;
        if (options.levelInTestMinimalPush == true)
        {
            options.pushLeft = 0;
            textPushLeftText.text = "Pushes : ";
            // butttonrestart.SetActive(false);
        }
        else
        {
            // butttonrestart.SetActive(true);
        }
        textPushLeftValue.text = options.pushLeft.ToString();
        if (Application.platform == RuntimePlatform.Android)
        {
            canvasControlesTactiles = GameObject.Find("Canvas/ControlesTactiles");
        }
    }

    /// <summary>
    /// Exécuté par les boutons UI, lors de l'animation de victoire ou de défaite, pour revenir au menu ou à l'éditeur.
    /// </summary>
    public void QuitAsyncAfterPlaying()
    {
        if (options.levelInTestBeforeUpload == false)
        { // C'est un niveau fait par nous.
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                SceneManager.LoadSceneAsync("WebGLMainMenu");
            else
                SceneManager.LoadSceneAsync("MainMenu");
        }
        else
        { // C'est un niveau fait par le joueur. Il est sans doute en train de le tester.
            SceneManager.LoadSceneAsync("LvlEditor");
        }
    }

    public void SendLevel()
    {
        //Debug.Log("Send Level");
        string path = Application.persistentDataPath + "/" + options.nameLevelToLoad;
        StreamReader textFile;
        textFile = new StreamReader(path + ".lvl");
        int oldpushLeft = 0;
        if (!int.TryParse(OurUtilitysFonctions.DecryptText(textFile.ReadLine(), "ChibIStheBEst"), out oldpushLeft)) // on lit la première ligne
        {
            // erreur de lecture 
        }
        else
        {
            internet.panelUpload.SetActive(true); // TODO : POO -> Ce script ne deverai pas influer sur cet élément, à modifier plus tard. "panelUpload" deverait même être priver dans l'autre script.
            string lvlData = OurUtilitysFonctions.DecryptText(textFile.ReadToEnd(), "ChibIStheBEst");
            textFile.Close();
            lvlData = OurUtilitysFonctions.EncryptText(lvlData, "ChibIStheBEst");
            using (FileStream fs = new FileStream(path + ".lvl", FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    // writer.WriteLine(OurUtilitysFonctions.EncryptText(options.pushLeft.ToString(), "ChibIStheBEst")); //ecrit la nouvelle valeur de pushleft
                    writer.WriteLine(OurUtilitysFonctions.EncryptText(oldpushLeft.ToString(), "ChibIStheBEst")); //ecrit la nouvelle valeur de pushleft
                    writer.Write(lvlData);
                }
            }
            internet.SendLevel(); // pas de soucis ici.
        }
    }

    /// <summary>
    /// Inscrit un niveau fait par nous niveau comme fini dans le fichier "Levelprogress"
    /// </summary>
    /// <param name="name"></param>
    public void MarkLevelFinished(string name)
    {
        if (File.Exists(path + "Levelprogress"))
        {
            string[] lvlList = File.ReadAllLines(path + "Levelprogress");
            bool b_levelIsInList = false;
            string[] infosList = new string[] { "levelname", "O", "0" };
            int line;
            for (line = 0; line < lvlList.Length; line++)
            {
                string stLine = lvlList[line];
                if (stLine.Contains(name))
                {
                    // int debut = stLine.IndexOf('|');
                    // string stringInfos = stLine.Remove(0, debut + 1);  // on supprime le nom du level et le premier |
                    infosList= stLine.Split('|');    
                    b_levelIsInList = true;
                    break;
                }                
            }
            if (b_levelIsInList == false) // le level n'avais jamais été fini
            {
                //options.nbOfLevelFinished++;
                //lvlList[0]="NbFinished|" + options.nbOfLevelFinished;
                File.WriteAllLines(path + "Levelprogress", lvlList);
                File.AppendAllText(path + "Levelprogress", name + "|F|" + options.pushLeft);
            }
            else  // trouvé
            {     
                if (infosList[1] == "F") // Le level à déja été fini
                {
                    Debug.Log("Le level " + infosList[0] + " à déjà été fini avec " + infosList[2] + " coups restants.");

                    if (options.pushLeft > Int32.Parse(infosList[2])) // on a amélioré le score
                    {
                        Debug.Log("On a amélioré le score " + infosList[2] + "->" + options.pushLeft);
                        lvlList[line] = name + "|F|" + options.pushLeft; // on reconstitue la nouvelle chaine
                        File.WriteAllLines(path + "Levelprogress", lvlList);
                    }
                }
            }
        }
        else // le fichier Levelprogress n'existe pas encore. Ne peut pas arriver en dehors de l'éditeur.
        {
            File.AppendAllText(path + "Levelprogress", name + "|F|" + options.pushLeft);
        }
    }

    void Vibrate() {
#if UNITY_ANDROID
        if (options.vibrate) {
            Handheld.Vibrate();
        }
#endif
    }



}