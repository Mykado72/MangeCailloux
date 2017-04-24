using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour {

    [Header("Panels")]
    [SerializeField] private GameObject panelAccueil;
    [SerializeField] private GameObject panelMenuGame;
    [SerializeField] private GameObject panelSelectLevel;
    [SerializeField] private GameObject panelMyLevel;
    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject panelAccount;
    [SerializeField] private GameObject panelCommunityLevel;
    [SerializeField] private GameObject panelHowTo;
    /*[SerializeField] private GameObject panelHowToPlay;
    [SerializeField] private GameObject panelHowToUseUserID;
    [SerializeField] private GameObject panelHowToToCreatLevel;*/

    [Header("Variables niveaux customs")]
    [SerializeField] private GameObject prefabCustomLevel;
    [SerializeField] private GameObject prefabDownloadedLevel;
    [SerializeField] private Transform niveauxCustomViewportContent;
    [SerializeField] private GameObject panelConfirmationSuppressionLevelCustom;
    private string levelToDelete; // Si un niveau custom veut être supprimer contient le nom.
    private GameObject goToDelete; // Si un niveau custom veut être supprimer contient le gameObject de la grille à supprimer.

    [Space(20)]
    public Text textUsername; // Le texte du panel "MenuGame"
    public InputField if_Username; // L'inputField du panel "account"
    public InputField if_Password; // L'inputField du panel "account"
    public GameObject goError;
    public GameObject goInfo;
    public Text textError;
    public Text textInfo;
    public Slider sliderMusicsound;
    public Slider sliderFXsound;
    [SerializeField] private Toggle togglePad;
    [SerializeField] private Toggle toggleVibrations;
    [SerializeField] private Texture2D cursorIcon;
    [SerializeField] private Button[] qualityButtons;

    private GameObject currentPanelDisplay;
    private bool hasCreatedCustomLevelList;
    private bool hasUpdateLevelProgression;

    [SerializeField] private SoundManager soundManager;
    [SerializeField] private AudioClip cancelSound;
    [SerializeField] private AudioClip errorSound;

    [SerializeField] private DownloadLevelList downloadLevelList;
    public Text textVersion;
    /*private Animator animHowTo;
    [SerializeField]  private Animator animHowToPlay;
    [SerializeField]  private Animator animHowToUseUserID;
    [SerializeField]  private Animator animHowToCreatLevel;*/
    
    private bool displayHowToSomthing;    
    private bool howToManager_isInvoked;

    void Start() {
        textVersion.text = "Version : " + Application.version;
#if !UNITY_ANDROID
        Cursor.SetCursor(cursorIcon, Vector2.zero, CursorMode.ForceSoftware);
#endif
        if (OptionManager.optionsInstance.username == "" && PlayerPrefs.GetString("username", "") != "" && PlayerPrefs.GetString("password", "") != "") { // Le joueur n'est pas connecté et dispose d'un compte sur l'appareil.
            StartCoroutine(CoLoginAccount(PlayerPrefs.GetString("username"), OurUtilitysFonctions.DecryptText( PlayerPrefs.GetString("password"), "ChibisIsOK"), false));
        } else if (OptionManager.optionsInstance.username != "") { // Le joueur est déjà connecté.
            textUsername.text = "Welcome " + OptionManager.optionsInstance.username + "!";
        }
        OptionManager.optionsInstance.CalculLevelsFinishedNumber();

        switch (OptionManager.optionsInstance.levelType) {
            case 0:
                ShowAccueil();
                break;
            case 1:
                ShowSelectLevel();
                break;
            case 2:
                ShowMyLevel();
                break;
            case 3:
                ShowCommunityLevel();
                break;
            case 4:
                ShowMenuGame();
                break;
            default:
                ShowAccueil();
                break;
        }


        if (!howToManager_isInvoked) { // Temporaire, car si on ne la charge pas au lancement il se peut que lors d'un clique sur 
                                       // un bouton "HowTo" ne réponde pas du tout.
            SceneManager.LoadSceneAsync("HowTo-Additiv", LoadSceneMode.Additive);
            howToManager_isInvoked = true;
        }
    }

    void Update() {

        if (!displayHowToSomthing) {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                soundManager.PlayFXSound(cancelSound);
                if (currentPanelDisplay == panelAccueil) {
                    QuitApplication();
                } else if (currentPanelDisplay == panelMenuGame) {
                    ShowAccueil();
                } else {
                    if (currentPanelDisplay == panelOptions) {
                        SaveOptions();
                    }
                    ShowMenuGame();
                }
            }
        } else {

            if (Input.GetKeyUp(KeyCode.Escape)) {
                ShowHowTo();
                HowToManager.main.ShowHowToPanel(HowToManager.HowToPanel.None, 0); // Cache le panel avec les images HowTo
                displayHowToSomthing = false;
            } else if (Input.anyKeyDown) {
                //animHowTo.SetTrigger("Next");
                HowToManager.main.IncreaseImageToDisplay();
                if (HowToManager.main.currentPanelDisplayed == HowToManager.HowToPanel.None) { // On était à la dernière image avant le clique.
                    displayHowToSomthing = false;
                }
            }

        }

    }

    #region Gestion des panels
    public void ShowAccueil() {
        currentPanelDisplay = panelAccueil;
        panelAccueil.SetActive(true);
        panelMenuGame.SetActive(false);
        panelSelectLevel.SetActive(false);
        panelMyLevel.SetActive(false);
        panelCommunityLevel.SetActive(false);
        panelOptions.SetActive(false);
        panelAccount.SetActive(false);
        panelHowTo.SetActive(false);
        /*panelHowToPlay.SetActive(false); 
        panelHowToUseUserID.SetActive(false); 
        panelHowToToCreatLevel.SetActive(false); */
}

    public void ShowMenuGame() {
        currentPanelDisplay = panelMenuGame;
        panelAccueil.SetActive(false);
        panelMenuGame.SetActive(true);
        panelSelectLevel.SetActive(false);
        panelMyLevel.SetActive(false);
        panelCommunityLevel.SetActive(false);
        panelOptions.SetActive(false);
        panelAccount.SetActive(false);
        panelHowTo.SetActive(false);
        /*panelHowToPlay.SetActive(false);
        panelHowToUseUserID.SetActive(false);
        panelHowToToCreatLevel.SetActive(false);*/
    }

    public void ShowSelectLevel() {

        currentPanelDisplay = panelSelectLevel;
        panelAccueil.SetActive(false);
        panelMenuGame.SetActive(false);
        panelSelectLevel.SetActive(true);
        panelMyLevel.SetActive(false);
        panelCommunityLevel.SetActive(false);
        panelOptions.SetActive(false);
        panelAccount.SetActive(false);
        panelHowTo.SetActive(false);
        /*panelHowToPlay.SetActive(false);
        panelHowToUseUserID.SetActive(false);
        panelHowToToCreatLevel.SetActive(false);*/
        OptionManager.optionsInstance.levelType = 1;
        if (!hasUpdateLevelProgression) {
            OptionManager.optionsInstance.CalculLevelsFinishedNumber();
            panelSelectLevel.GetComponent<LevelProgression>().TempSetupList();
            hasUpdateLevelProgression = true;
        }
    }

    public void ShowMyLevel() {
        InitializeMyLevelList();
        currentPanelDisplay = panelMyLevel;
        panelAccueil.SetActive(false);
        panelMenuGame.SetActive(false);
        panelSelectLevel.SetActive(false);
        panelMyLevel.SetActive(true);
        panelCommunityLevel.SetActive(false);
        panelOptions.SetActive(false);
        panelAccount.SetActive(false);
        panelHowTo.SetActive(false);
        /*panelHowToPlay.SetActive(false);
        panelHowToUseUserID.SetActive(false);
        panelHowToToCreatLevel.SetActive(false);*/
        OptionManager.optionsInstance.levelType = 2;
    }

    public void ShowCommunityLevel() {
        currentPanelDisplay = panelCommunityLevel;
        panelAccueil.SetActive(false);
        panelMenuGame.SetActive(false);
        panelSelectLevel.SetActive(false);
        panelMyLevel.SetActive(false);
        panelCommunityLevel.SetActive(true);
        panelOptions.SetActive(false);
        panelAccount.SetActive(false);
        panelHowTo.SetActive(false);
        /*panelHowToPlay.SetActive(false);
        panelHowToUseUserID.SetActive(false);
        panelHowToToCreatLevel.SetActive(false);*/
        OptionManager.optionsInstance.levelType = 3;
        downloadLevelList.UpdateList();
    }

    public void ShowOptions() {
        InitSoundSliders();
        InitQualitySettingButtons();
        InitToggles();
        currentPanelDisplay = panelOptions;
        panelAccueil.SetActive(false);
        panelMenuGame.SetActive(true);
        panelSelectLevel.SetActive(false);
        panelMyLevel.SetActive(false);
        panelCommunityLevel.SetActive(false);
        panelOptions.SetActive(true);
        panelAccount.SetActive(false);
        panelHowTo.SetActive(false);
        /*panelHowToPlay.SetActive(false);
        panelHowToUseUserID.SetActive(false);
        panelHowToToCreatLevel.SetActive(false);*/
    }

    public void ShowAccount() {
        if_Username.text = OptionManager.optionsInstance.username;
        currentPanelDisplay = panelAccount;
        panelAccueil.SetActive(false);
        panelMenuGame.SetActive(true);
        panelSelectLevel.SetActive(false);
        panelMyLevel.SetActive(false);
        panelCommunityLevel.SetActive(false);
        panelOptions.SetActive(false);
        panelAccount.SetActive(true);
        panelHowTo.SetActive(false);
        /*panelHowToPlay.SetActive(false);
        panelHowToUseUserID.SetActive(false);
        panelHowToToCreatLevel.SetActive(false);*/
    }

    public void ShowHowTo()
    {
        panelHowTo.SetActive(true);
        currentPanelDisplay = panelHowTo;
        panelAccueil.SetActive(false);
        panelMenuGame.SetActive(true);
        panelSelectLevel.SetActive(false);
        panelMyLevel.SetActive(false);
        panelCommunityLevel.SetActive(false);
        panelOptions.SetActive(false);
        panelAccount.SetActive(false);
        if (!howToManager_isInvoked) {
            SceneManager.LoadSceneAsync("HowTo-Additiv", LoadSceneMode.Additive);
            howToManager_isInvoked = true;
        }
        /*panelHowToPlay.SetActive(false);
        panelHowToUseUserID.SetActive(false);
        panelHowToToCreatLevel.SetActive(false);*/
    }

    public void StartHowToPlay() // Pour les bouttons, sinon impossible d'appeler "StartHowTo()"
    {
        /*panelHowTo.SetActive(true);
        panelHowToPlay.SetActive(true);
        inHowToPlay = true;
        animHowTo = animHowToPlay;*/
        StartHowTo(HowToManager.HowToPanel.Play);
    }

    public void StartHowToUseUserID()
    {
        /*panelHowTo.SetActive(true);
        panelHowToUseUserID.SetActive(true);
        inHowToUseUserID = true;
        animHowTo = animHowToUseUserID;*/
        StartHowTo(HowToManager.HowToPanel.UseUserId);
    }
    
    public void StartHowToCreatLevel()
    {
        /*panelHowTo.SetActive(true);
        panelHowToToCreatLevel.SetActive(true);
        inHowToCreatLevel = true;
        animHowTo = animHowToCreatLevel;*/
        StartHowTo(HowToManager.HowToPanel.CreateLevel);
    }

    public void StartHowTo(HowToManager.HowToPanel panelToStart) {
        if (HowToManager.main != null) { // TODO : au lieu de ne rien faire, enregistrer le clic pour lancer dès que l'élément est chargé.
            displayHowToSomthing = true;
            HowToManager.main.ShowHowToPanel(panelToStart, 0);
        }
    }

    public void ShowOnlineLevels() {
        OptionManager.optionsInstance.levelType = 5;
        SceneManager.LoadScene("ListLevel");
    }
    #endregion

    #region Fonctions pour le panel "Options"
    public void InitSoundSliders() {
        if (sliderMusicsound != null)
            sliderMusicsound.value = OptionManager.optionsInstance.volMusicsound;
        if (sliderFXsound != null)
            sliderFXsound.value = OptionManager.optionsInstance.volFXsound;
    }

    void InitToggles() {
        if (PlayerPrefs.GetInt("usePad", 1) == 1) {
            togglePad.isOn = true;
        } else {
            togglePad.isOn = false;
        }

        if (PlayerPrefs.GetInt("useVibration", 1) == 0) {
            toggleVibrations.isOn = false;
        } else {
            toggleVibrations.isOn = true;
        }

    }

    private void InitQualitySettingButtons() {
        SpriteState standardColor = qualityButtons[0].spriteState;
        for (int i = 0; i < qualityButtons.Length; i++) {
            if (i == QualitySettings.GetQualityLevel()) {
                qualityButtons[i].GetComponent<Image>().sprite = standardColor.highlightedSprite;
            } else {
                qualityButtons[i].GetComponent<Image>().sprite = standardColor.disabledSprite;
            }
        }
    }
    
    public void SetQualitySetting(int nb) {
        QualitySettings.SetQualityLevel(nb, true);
        InitQualitySettingButtons();
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetFloat("MusicVol", OptionManager.optionsInstance.volMusicsound);
        PlayerPrefs.SetFloat("FXVol", OptionManager.optionsInstance.volFXsound);
        if (togglePad.isOn) {
            PlayerPrefs.SetInt("usePad", 1);
            OptionManager.optionsInstance.usePad = true;
        } else {
            PlayerPrefs.SetInt("usePad", 0);
            OptionManager.optionsInstance.usePad = false;
        }
        if (toggleVibrations.isOn) {
            PlayerPrefs.SetInt("useVibration", 1);
            OptionManager.optionsInstance.vibrate = true;
        } else {
            PlayerPrefs.SetInt("useVibration", 0);
            OptionManager.optionsInstance.vibrate = false;
        }
        PlayerPrefs.Save();
    }
#endregion

    public void GoToEditor() {
        OptionManager.optionsInstance.levelType = 4;
        SceneManager.LoadScene("LvlEditor");
    }
    
    public void QuitApplication() {
        Application.Quit();
    }

    /// <summary>
    /// Appelée depuis les boutons d'UI, sert à charger un niveau présent dans le dossier ressource, donc créé avant un build.
    /// </summary>
    public void LoadLevel (string levelName) {
        if (PlayerPrefs.GetInt("HowToPlayWasRead", 0) == 0 ) // On n'a jamais vu le tuto en entier
        {
            if (!howToManager_isInvoked) {
                SceneManager.LoadSceneAsync("HowTo-Additiv", LoadSceneMode.Additive);
                howToManager_isInvoked = true;
            }
            StartHowTo(HowToManager.HowToPanel.Play);
        }
        else {
            OptionManager.optionsInstance.PlayStandardLevel(levelName);
            SceneManager.LoadScene("Loading");
        }
	}

    /// <summary>
    /// Appelée depuis les boutons d'UI, sert à charger un niveau créé par l'utilisateur de cet appareil.
    /// </summary>
    public void LoadLevelCustom(string levelName) {
        OptionManager.optionsInstance.PlayCustomLevel(levelName);
        SceneManager.LoadScene("Loading");
    }

    /// <summary>
    /// Appelée depuis les boutons d'UI, sert à charger un niveau créé par l'utilisateur de cet appareil.
    /// </summary>
    public void LoadDownloadedLevel(string levelName)
    {
        OptionManager.optionsInstance.PlayDownloadedLevel(levelName);
        SceneManager.LoadScene("Loading");
    }

    public void NeedRefreachMyLevelList() {
        hasCreatedCustomLevelList = false;
    }

    /// <summary>
    /// Créé la liste des niveau fait par l'utilisateur.
    /// </summary>
    void InitializeMyLevelList() {
        if (hasCreatedCustomLevelList) {
            return;
        }
        int t = niveauxCustomViewportContent.childCount;
        for (int i = 0; i< t; i++) { // Destruction de tous les vieux enfants (optimisable)
            Destroy(niveauxCustomViewportContent.GetChild(i).gameObject);
        }


        List<string> listAll = new List<string>();
        string[] list = Directory.GetFiles(Application.persistentDataPath);
        for (int i = 0; i < list.Length; i++) {
            if (list[i].EndsWith(".lvl")) {
                listAll.Add(list[i]);
            }
        }
        if (Directory.Exists(Application.persistentDataPath + @"/Downloaded")) {
            list = Directory.GetFiles(Application.persistentDataPath + @"/Downloaded");
            for (int i = 0; i < list.Length; i++) {
                if (list[i].EndsWith(".lvl")) {
                    listAll.Add(list[i]);
                }
            }
        }

        for (int i = 0; i < listAll.Count; i++) {
            listAll[i] = listAll[i].Replace(Application.persistentDataPath, "").Replace("Downloaded", "").Replace("/", "").Replace(@"\", "").Replace(".lvl", "");
        }
        listAll.Sort((x, y) => string.Compare(x, y)); // Ordre alphabetique.

        for (int i = 0; i < listAll.Count; i++) {

            string levelName = listAll[i];
            if (File.Exists(Application.persistentDataPath + @"/" + levelName + ".lvl")) { // TODO : Géré le cas où un niveau téléchargé porte le même nom qu'un niveau custom.
                GameObject button = Instantiate(prefabCustomLevel, niveauxCustomViewportContent) as GameObject;
                button.GetComponent<Button>().onClick.AddListener(() => { this.LoadLevelCustom(levelName); }); // Fonctionne bien même si ne s'affiche pas dans l'inspector.
                button.GetComponentInChildren<Text>().text = levelName;
                button.transform.FindChild("ButtonErase").GetComponent<Button>().onClick.AddListener(() => { this.EraseLevel(levelName, button); }); // Fonctionne bien même si ne s'affiche pas dans l'inspector.
                button.transform.localScale = Vector3.one;
                if (File.Exists(Application.persistentDataPath + @"/" + levelName + ".png")) {
                    byte[] data = File.ReadAllBytes(Application.persistentDataPath + @"/" + levelName + ".png");
                    Texture2D texture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
                    texture.LoadImage(data);
                    button.transform.FindChild("Vignette").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
            } else if (File.Exists(Application.persistentDataPath + @"/Downloaded/" + levelName + ".lvl")) {
                GameObject button = Instantiate(prefabDownloadedLevel, niveauxCustomViewportContent) as GameObject;
                button.GetComponent<Button>().onClick.AddListener(() => { this.LoadDownloadedLevel(levelName); }); // Fonctionne bien même si ne s'affiche pas dans l'inspector.
                string creator = levelName.Split('#')[0];
                button.transform.FindChild("Creator").GetComponent<Text>().text = creator;
                if (creator == "Pitou" || creator == "Mykado" || creator == "MonsieurRaton") { // On a bien le droit de se faire remarquer ^^ (pour montrer les niveaux fait par les dev' )
                    button.transform.FindChild("Creator").GetComponent<Text>().color = Color.green;
                }
                button.transform.FindChild("LevelName").GetComponent<Text>().text = levelName.Split('#')[1];
                button.transform.FindChild("ButtonErase").GetComponent<Button>().onClick.AddListener(() => { this.EraseLevel(@"Downloaded/" +levelName, button); downloadLevelList.NeedUpdateList(); }); // Fonctionne bien même si ne s'affiche pas dans l'inspector.
                button.transform.localScale = Vector3.one;
                if (File.Exists(Application.persistentDataPath + @"/Downloaded/" + levelName + ".png")) {
                    byte[] data = File.ReadAllBytes(Application.persistentDataPath + @"/Downloaded/" + levelName + ".png");
                    Texture2D texture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
                    texture.LoadImage(data);
                    button.transform.FindChild("Vignette").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
            }

        }
        hasCreatedCustomLevelList = true;
    }

    public void EraseLevel(string levelToDeleteName, GameObject go) {
        panelConfirmationSuppressionLevelCustom.SetActive(true);
        levelToDelete = levelToDeleteName;
        goToDelete = go;
    }

    public void ConfirmErase() {
        File.Delete(Application.persistentDataPath + "//" + levelToDelete + ".png");
        File.Delete(Application.persistentDataPath + "//" + levelToDelete + ".lvl");
        Destroy(goToDelete);
        //Debug.Log("Erase : " + Application.persistentDataPath + "\\" + levelToDelete);
        panelConfirmationSuppressionLevelCustom.SetActive(false);
    }
    
    public void CreateAccount() {

        if (if_Password.text.Contains("/") || if_Password.text.Contains("\\")
         || if_Password.text.Contains("|") || if_Password.text.Contains("#")
         || if_Password.text.Contains(" ") || if_Password.text.Contains("?")) { //Le nom n'est pas valide
            DisplayError("Your password contains invalid caraters!");
        } else if(if_Password.text.Length < 4) {
            DisplayError("Your password is too short! It needs at least 4 character.");
        } else if (if_Password.text.Length > 16) {
            DisplayError("Your password is too long! You can't put more than 16 characters!");
        }else if (if_Username.text.Length < 3) {
            DisplayError("Your username is too short! It needs at least 3 character.");
        } else if (if_Username.text.Length > 32) {
            DisplayError("Your username is too long! You can't put more than 32 characters!");
        } else {
            StartCoroutine(CoCreateAccount(if_Username.text, if_Password.text));
        }

    }

    public IEnumerator CoCreateAccount(string m_user, string m_password)
    {
        string m_randomSalt = OurUtilitysFonctions.RandomSalt(32);
        m_password = m_password + m_randomSalt;
        m_password = OurUtilitysFonctions.HashMD5(m_password);
        WWWForm postForm = new WWWForm();
        postForm.AddField("username", m_user);
        postForm.AddField("password", m_password);
        postForm.AddField("salt", m_randomSalt);
        // bIsUploaded = false;
        WWW upload = new WWW(Internet.adress + "/CreateAccount.php", postForm);
        yield return upload;
        if (upload.error == null)
        {
            Debug.Log(upload.text); // affiche le retour de la page php
            if (upload.text.Contains("ChibisIsOK"))  // contient le mot ChibisIsOK
            {
                int debut = upload.text.IndexOf("ChibisIsOK");
                string stringToDisplay = upload.text.Remove(0, debut + "ChibisIsOK".Length); // on supprime les espaces et le mot ChibisIsOK
                DisplayInfo(stringToDisplay);
                PlayerPrefs.SetString("username", m_user);
                PlayerPrefs.SetString("password", OurUtilitysFonctions.EncryptText(m_password, "ChibisIsOK"));
                OptionManager.optionsInstance.username = m_user;
                textUsername.text = "Welcome " + OptionManager.optionsInstance.username + "!";
                ShowMenuGame();
            }
            else
            {
                DisplayError(upload.text);
            }
        }
        else
        {
            Debug.Log("Error during upload: " + upload.error);
            DisplayError(upload.text + System.Environment.NewLine + upload.error);
        }
    }

    public void LoginAccount()
    {
        StartCoroutine(CoLoginAccount(if_Username.text, if_Password.text, true));
    }

    public IEnumerator CoLoginAccount(string m_user, string m_password, bool showLogs)
    {
        WWWForm postForm = new WWWForm();
        postForm.AddField("username", m_user);
        postForm.AddField("password", m_password);
        WWW upload = new WWW(Internet.adress + "/LoginAccount.php", postForm);
        yield return upload;
        if (upload.error == null)
        {
            if (upload.text.Contains("ChibisIsOK"))  // contient le mot ChibisIsOK
            {
                string uploadtext = upload.text;
                int debut = uploadtext.IndexOf("ChibisIsOK");
                uploadtext.Remove(0, debut + "ChibisIsOK".Length); // on supprime les espaces et le mot ChibisIsOK
                int indexPassword = uploadtext.IndexOf("<password>") + "<password>".Length;
                int indexSalt = uploadtext.IndexOf("<salt>");
                int indexEnd = uploadtext.IndexOf("<end>");
                string password = uploadtext.Substring(indexPassword, indexSalt - indexPassword);
                string salt = uploadtext.Substring(indexSalt + "<salt>".Length, indexEnd - (indexSalt + "<salt>".Length));
                if (OurUtilitysFonctions.HashMD5(m_password + salt) == password) {

                    if (showLogs) {
                        DisplayInfo("Welcome " + m_user + "!");
                        ShowMenuGame();
                        PlayerPrefs.SetString("username", m_user);
                        PlayerPrefs.SetString("password", OurUtilitysFonctions.EncryptText(m_password, "ChibisIsOK"));
                    }
                    if_Password.text = "";
                    textUsername.text = "Welcome " + m_user + "!";
                    OptionManager.optionsInstance.username = m_user;

                } else if (showLogs) {

                    DisplayError("Wrong password");

                }
            } else if (showLogs) {

                DisplayError(upload.text);

            }
        }
        else if(showLogs)
        {
            Debug.Log("Error during upload: " + upload.error);
            DisplayError(upload.text + System.Environment.NewLine + upload.error);
        }
    }

    public void ErrorClear()
    {
        textError.text = null;
        goError.SetActive(false);
    }

    public void DisplayError(string error)
    {
        goError.SetActive(true);
        textError.text = error;
        soundManager.PlayFXSound(errorSound);
    }

    public void InfoClear()
    {
        textInfo.text = null;
        goInfo.SetActive(false);
    }

    public void DisplayInfo(string info)
    {
        goInfo.SetActive(true);
        textInfo.text = info;
    }
}
