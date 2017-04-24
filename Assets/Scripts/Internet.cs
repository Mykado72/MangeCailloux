using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class Internet : MonoBehaviour {

    public static Internet internetInstance; // Singelton.
    public static string adress = "http://sokobancoon.esy.es/";
    public bool bTestLocal;
    public bool bFinished;
    public bool bIsUploading;
    public bool bError ;
    public bool bUseLastVersion;
    public Text lvlname;
    public Text defaultnbpush;
    public Text lvlstring;
    public Text lvlhash;
    public GameObject goError;
    public GameObject goInfo;
    public GameObject goUpdateMessage;
    public Text textError;
    public Text textInfo;
    public Text textVersionUpdate;
    public Text textUpdateMessage;
    private OptionManager options;
    private LogicManager logic;
    public GameObject panelUpload;
    public GameObject panelUpdate;
    private string path;

    void Awake()
    {
        if (internetInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            internetInstance = this;
            DontDestroyOnLoad(gameObject);
            TestIfIsLastVersion(); // Test de la version au lancement de l'application
        }
        
    }

    // Use this for initialization
    void Start () {
        options = OptionManager.optionsInstance;
    }

    void Update()
    {
        if (bTestLocal == true)
        {
            adress = "http://localhost/sokobancoon/";
        }
        else
        {
            adress = "http://sokobancoon.esy.es/";
        }
    }

    public void UploadLevel(string m_localfile, string m_uploadURL, string m_name, int m_pushLeft)
    {  
        StartCoroutine(CoUploadLevel(m_localfile, m_uploadURL, m_name, m_pushLeft));
    }

    public void UploadFinished()
    {
//        bFinished = true;
        StopAllCoroutines();
    }

    public IEnumerator CoUploadLevel(string m_localfile, string m_uploadURL, string m_lvl_name, int m_pushLeft)
    {
        bIsUploading = true;
        if ((System.IO.File.Exists(m_localfile + ".png")) && (System.IO.File.Exists(m_localfile + ".lvl")))
        {
            WWW localImageFileToWWW = new WWW("file:///"+m_localfile + ".png");
            WWW localLVLFileToWWW = new WWW("file:///" + m_localfile + ".lvl");
            if (localImageFileToWWW.size == 0 || localLVLFileToWWW.size == 0)
            {
                if (localImageFileToWWW.size == 0)
                {
                    DisplayError("Le fichier " + m_localfile + ".png est vide \n");
                }                    
                if (localLVLFileToWWW.size == 0)
                {
                    DisplayError("Le fichier " + m_localfile + ".lvl est vide \n");
                }            
            }
            else
            {
                WWWForm postForm = new WWWForm();
                Debug.Log("Préparation du formulaire");
                // postForm.AddField("action", "Upload Image");
                postForm.AddBinaryData("image", localImageFileToWWW.bytes, m_localfile + ".png", "image/png");
                postForm.AddBinaryData("lvl", localLVLFileToWWW.bytes, m_localfile + ".lvl", "text/plain");
                postForm.AddField("lvl_name", m_lvl_name);
                postForm.AddField("creator", options.username);
                postForm.AddField("defaultnbpush", m_pushLeft);
                // string m_randomSalt = OurUtilitysFonctions.RandomSalt(32);
                string m_lvlstring = OurUtilitysFonctions.EncryptText(localLVLFileToWWW.text, "ChibIStheBEst"); // + m_randomSalt                
                postForm.AddField("lvl_string", localLVLFileToWWW.text);
                string m_hash = OurUtilitysFonctions.HashMD5(m_lvlstring);              
                postForm.AddField("hash", m_hash);
                postForm.AddField("appVersion", Application.version);
                // postForm.AddBinaryData("file", levelData, fileName, "text/xml");
                // bIsUploaded = false;
                WWW upload = new WWW(m_uploadURL, postForm);
                Debug.Log("Attente réponse de "+ m_uploadURL);
                yield return upload;
                if (upload.error == null)
                {
                    if (upload.text.Contains("BAD")||(!upload.text.Contains("ChibisIsOK")))
                    {
                        bError = true;
                        Debug.Log(upload.text);
                        goError.SetActive(false); // cache la fenetre d'erreur dans le cas d'un "Retry"
                        DisplayInfo(upload.text);
                        panelUpload.SetActive(false);
                    }
                    else
                    {
                        bIsUploading = false;
                        bFinished = true;
                        Debug.Log("Upload Finished");
                        Debug.Log(upload.text);
                        goError.SetActive(false); // cache la fenetre d'erreur si c'est un "Retry"
                        yield return new WaitForSeconds(2);
                        DisplayInfo("Level Uploaded");
                        panelUpload.SetActive(false);
                    }
                }
                else
                {
                    bError = true;
                    bIsUploading = false;
                    bFinished = false;
                    panelUpload.SetActive(false);
                    Debug.Log("Error during upload: " + upload.error);
                    DisplayError(upload.text);
                    DisplayError(upload.error);
                }
            }
        }
        else
        {
            DisplayError(m_localfile + " ou .lvl n'existe(nt) pas");
            Debug.Log(m_localfile+" ou .lvl n'existe(nt) pas");
            // bFinished = false;
        }
        
    } 
       
    /*private IEnumerator LoadURL(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error==null) {
            yield return true;
        }
        else
        {
            yield return false;
        }
    }*/

    public void ErrorClear()
    {
        textError.text = null;
        goError.SetActive(false);
        bError = false;
    }
    
    public void DisplayError(string error)
    {
        goError.SetActive(true);
        textError.text = error;
    }

    public void DisplayUpdateMessage(string version, string message)
    {
        goUpdateMessage.SetActive(true);
        textVersionUpdate.text = "New version : " + version;
        textUpdateMessage.text = message;
    }


    public void InfoClear()
    {
        textInfo.text = null;
        goInfo.SetActive(false);
        bError = false;
    }

    public void DisplayInfo(string info)
    {
        goInfo.SetActive(true);
        textInfo.text = info;
    }

    public void SendLevel()
    {
        Debug.Log("Send Level");
        bIsUploading = true;
        string path = Application.persistentDataPath + "/" + options.nameLevelToLoad;
        UploadLevel(path, adress + "upload.php", options.nameLevelToLoad, options.pushLeft);
    }

    public void AfterQuitOrOK()
    {
        if (options.levelIsCustom == false)
        { // C'est un niveau fait par nous.
            SceneManager.LoadScene("MainMenu");
        }
        else
        { // C'est un niveau fait par le joueur. Il est sans doute en train de le tester.
            SceneManager.LoadScene("LvlEditor");
        }
    }

    public void TestIfIsLastVersion() {
        StartCoroutine(CoTestIfIsLastVersion("GetActualVersion.php"));
    }

    /// <summary>
    /// Permet de savoir si l'application du client est à la dernière version.
    /// Initialise la variable bUseLastVersion.
    /// </summary>
    private IEnumerator CoTestIfIsLastVersion(string m_phpPage)
    {
        WWW www = new WWW(adress + m_phpPage);
        yield return www;
        //Debug.Log("wait... for actual version...");
        if (www.error == null)
        {
            if (www.text.Contains("|"))  // contient les caractère |
            {
                string[] stringList = www.text.Split('|');
                stringList[0] = stringList[0].Replace(" ", "");
                Debug.Log(stringList[0]);
                Debug.Log(stringList[1]);
                float lastversion = 0;
                float appversion = 0;
                if (float.TryParse(stringList[0], out lastversion))
                {
                    //Debug.Log("Derniere version " + lastversion);
                    if (float.TryParse(Application.version, out appversion))
                    {
                        //Debug.Log("version de cette application : " + appversion);
                        if (lastversion > appversion)
                        {
                            {
                                DisplayUpdateMessage(stringList[0], stringList[1]);                               
                            }
                        }
                        else
                        {
                            //Debug.Log("A jour");
                            bUseLastVersion = true;
                        }
                    }
                }            
            }
            else
            {
                //DisplayError(www.text + Environment.NewLine + www.error);
                Debug.Log("Error can not test version: " + www.error);
                bError = true;
            }
        }
        else
        {
            //DisplayError(www.text + Environment.NewLine + www.error);
            Debug.Log("Error can not test version: " + www.error);
            bError = true;
        }
    }

    public void OpenLinkToNewVersion()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.Djingarey.Lehay.Pitou.SokobanCoon");
        }
        else if ((Application.platform == RuntimePlatform.WindowsEditor)||(Application.platform == RuntimePlatform.WindowsPlayer))
        {
            Application.OpenURL("https://djingarey.itch.io/sokobancoon");
        }
    }

    public void CancelUpdate()
    {
        OpenLinkToNewVersion();
        Application.Quit();
        //SceneManager.LoadSceneAsync("MainMenu");
    }

}