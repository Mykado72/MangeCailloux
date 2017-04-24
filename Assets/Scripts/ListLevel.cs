using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ListLevel : MonoBehaviour
{

    public static ListLevel listLevelInstance; // Singelton.
    private static Internet internetInstance; // Singelton.
    private Dictionary<string, string> dictLvlName;
    private Dictionary<string, string> dictLvlCreator;
    private OptionManager options;
    private string[] lvlNameToDownload;
    private string[] lvlRealnameToDownload;
    private string[] lvlCreatorToDownload;
    public List<string> listLvlNameToDownload;
    public List<GameObject> buttons;
    public GameObject panelListeNiveaux;
    public GameObject canvasDownload;
    private Transform niveauxViewportGrid;
    public GameObject buttonLevelPrefab;
    //    public GameObject sprites;
    public bool wwwLvlNameIsDone;
    public bool wwwLvlRealNameIsDone;
    public bool wwwLvlCreatorIsDone;
    public bool wwwLvlNameIsStarted;
    public bool wwwLvlRealNameIsStarted;
    public bool wwwLvlCreatorIsStarted;
    public bool wwwLvlNameError;
    public bool wwwLvlRealNameError;
    public bool wwwLvlCreatorError;
    public bool listsAreFill;
    public int nbOfImageDownloaded;
    public int buttonnumber;
    private string path; 
    // Use this for initialization
    void Awake()
    {
        path = Application.persistentDataPath + "/Downloaded/";
        if (listLevelInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            listLevelInstance = this;
        }
        internetInstance = Internet.internetInstance;
        canvasDownload.SetActive(true);
        panelListeNiveaux.SetActive(false);        
    }

    void Start()
    {
        options = OptionManager.optionsInstance;        
        wwwLvlNameIsDone = false;
        wwwLvlRealNameIsDone = false;
        wwwLvlCreatorIsDone = false;
        wwwLvlNameError = false;
        wwwLvlRealNameError = false;
        wwwLvlCreatorError = false;
        wwwLvlNameIsStarted = false;
        wwwLvlRealNameIsStarted = false;
        wwwLvlCreatorIsStarted = false;
        listsAreFill = false;
        buttonnumber = 0;
        nbOfImageDownloaded = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (listsAreFill == true)
        {
            if ((buttonnumber < lvlNameToDownload.Length) && (nbOfImageDownloaded== buttonnumber))
            {
                AddImageButton(buttonnumber);
                buttonnumber++;                
            }
            else // tout est téléchargé
            {
                RectTransform contentRect = panelListeNiveaux.transform.FindChild("Scroll View/Viewport/Content").GetComponent<RectTransform>();
                contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, -90);
            }
        }
        else
        {
            WaitDownloadLevelList();
            WaitOpenLevelList();
        }
    }

    private void WaitDownloadLevelList()
    {
        if ((wwwLvlNameIsDone == false) && (wwwLvlNameError == false) && (wwwLvlNameIsStarted == false))
            FillTabToDownload("UploadLvlList", "Name");
        if ((wwwLvlRealNameIsDone == false) && (wwwLvlRealNameError == false) && (wwwLvlRealNameIsStarted == false))
            FillTabToDownload("UploadLvlRealNamelList", "RealName");
        if ((wwwLvlCreatorIsDone == false) && (wwwLvlCreatorError == false) && (wwwLvlCreatorIsStarted == false))
            FillTabToDownload("UploadLvlCreatorList", "Creator");
    }

    public void DownloadLvl(string name)
    {
        string realname = null;
        dictLvlName.TryGetValue(name, out realname);
        Debug.Log("Downloaded " + name + ".lvl");
        StartCoroutine(CoDownloadLvl(Internet.adress + "uploads/LVLS/" + realname + ".lvl", name));
    }

    private void DownloadImageLvl(bool local, string name, int num)
    {
        string realname = null;
        dictLvlName.TryGetValue(name, out realname);
        if (local==true)
        {
            StartCoroutine(CoDownloadImageLvl("file:///" + path + name + ".png", name, num));
        }
        else
        {
            StartCoroutine(CoDownloadImageLvl(Internet.adress + "uploads/images/" + realname + ".png", name, num));
        }
    }

    private IEnumerator CoDownloadImageLvl(string url, string name, int num)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null)
        {
            System.IO.Directory.CreateDirectory(path);
            File.WriteAllBytes(path + name + ".png", www.bytes);
            buttons[num].GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0f, 0f, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 128f);
            www.Dispose();
            www = null;
            nbOfImageDownloaded++;
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }


    private IEnumerator CoDownloadLvl(string url, string name)
    {
        if (File.Exists(path + name + ".lvl"))
        {
            yield return null;
        }
        else
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.error == null)
            {
                System.IO.Directory.CreateDirectory(path);
                File.WriteAllBytes(path + name + ".lvl", www.bytes);
                www.Dispose();
                www = null;
                MarkAsAlreadyDownloaded(name);
            }
            else
            {
                Debug.Log("WWW Error: " + www.error);
            }
        }
    }

    private void MarkAsAlreadyDownloaded(string name)
    {
        int i = listLvlNameToDownload.IndexOf(name);
        buttons[i].transform.FindChild("Download").gameObject.SetActive(false);
        // buttons[i].transform.FindChild("Downloaded").gameObject.SetActive(true);
        buttons[i].transform.FindChild("ButtonPlay").gameObject.SetActive(true);
        buttons[i].transform.FindChild("ButtonErase").gameObject.SetActive(true);
    }

    private void MarkAsNotDownloaded(string name)
    {
        int i = listLvlNameToDownload.IndexOf(name);
        buttons[i].transform.FindChild("Download").gameObject.SetActive(true);
        // buttons[i].transform.FindChild("Downloaded").gameObject.SetActive(false);
        buttons[i].transform.FindChild("ButtonPlay").gameObject.SetActive(false);
        buttons[i].transform.FindChild("ButtonErase").gameObject.SetActive(false);
    }

    private bool AlreadyDownloaded(string name)
    {
        if (File.Exists(path + name + ".lvl"))
            return true;
        else
            return false;
    }


    private void FillTabToDownload(string m_phpPage, string m_what)
    {
        StartCoroutine(CoFillTabToDownload(m_phpPage, m_what));
    }

    private IEnumerator CoFillTabToDownload(string m_phpPage, string m_what)
    {
        WWWForm postForm = new WWWForm();
        postForm.AddField("List", "list");
        // Debug.Log(Internet.adress + m_phpPage + ".php");
        WWW www = new WWW(Internet.adress + m_phpPage + ".php", postForm);
        switch (m_what)
        {
            case "Name":
                wwwLvlNameIsStarted = true;
                break;
            case "RealName":
                wwwLvlRealNameIsStarted = true;
                break;
            case "Creator":
                wwwLvlCreatorIsStarted = true;
                break;
            default:
                break;
        }
        yield return www;
        Debug.Log("wait...");
        if (www.error == null)
        {
            int debut = www.text.IndexOf('|');
            if (debut != -1)  // contient les caractères |
            {
                string stringList = www.text.Remove(0, debut + 1);  // on supprime les espaces et le premier |
                if (stringList.Length >= 2)
                {
                    stringList = stringList.Remove(stringList.Length - 1, 1); // et le dernier |
                    switch (m_what)
                    {
                        case "Name":
                            lvlNameToDownload = stringList.Split('|');
                            wwwLvlNameIsDone = true;
                            break;
                        case "RealName":
                            lvlRealnameToDownload = stringList.Split('|');
                            wwwLvlRealNameIsDone = true;
                            break;
                        case "Creator":
                            lvlCreatorToDownload = stringList.Split('|');
                            wwwLvlCreatorIsDone = true;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    internetInstance.DisplayError("No List For "+ m_what);
                    Debug.Log("Liste vide pour "+ m_what);
                    switch (m_what)
                    {
                        case "Name":
                            wwwLvlNameError = true;
                            break;
                        case "RealName":
                            wwwLvlRealNameError = true;
                            break;
                        case "Creator":
                            wwwLvlCreatorError = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                internetInstance.DisplayError(www.text);
                internetInstance.DisplayError(www.error);
                Debug.Log(www.text);
                Debug.Log("Error during upload: " + www.error);
                switch (m_what)
                {
                    case "Name":
                        wwwLvlNameError = true;
                        break;
                    case "RealName":
                        wwwLvlRealNameError = true;
                        break;
                    case "Creator":
                        wwwLvlCreatorError = true;
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            internetInstance.DisplayError(www.text);
            internetInstance.DisplayError(www.error);
            Debug.Log("Error during upload: " + www.error);
            switch (m_what)
            {
                case "Name":
                    wwwLvlNameError = true;
                    break;
                case "RealName":
                    wwwLvlRealNameError = true;
                    break;
                case "Creator":
                    wwwLvlCreatorError = true;
                    break;
                default:
                    break;
            }
        }
    }

    private void WaitOpenLevelList()
    {
        if ((wwwLvlNameIsDone == true) && (wwwLvlRealNameIsDone == true) && (wwwLvlCreatorIsDone == true))
            PrepLevelList();
    }

    private void PrepLevelList()
    {
        listLvlNameToDownload = new List<string>(lvlNameToDownload);
        dictLvlName = new Dictionary<string, string>();
        dictLvlName.Clear();
        for (int i = 0; i < lvlRealnameToDownload.Length; i++)
        {
            dictLvlName.Add(lvlNameToDownload[i], lvlRealnameToDownload[i]);
        }
        dictLvlCreator = new Dictionary<string, string>();
        dictLvlCreator.Clear();
        for (int i = 0; i < lvlCreatorToDownload.Length; i++)
        {
            dictLvlCreator.Add(lvlNameToDownload[i], lvlCreatorToDownload[i]);
        }
        listsAreFill = true;
        canvasDownload.SetActive(false);
        panelListeNiveaux.SetActive(true);        
    }

    private void AddImageButton(int i)
    {
        niveauxViewportGrid = panelListeNiveaux.transform.FindChild("Scroll View/Viewport/Content");
        GameObject button = Instantiate(buttonLevelPrefab, niveauxViewportGrid) as GameObject;
        button.transform.FindChild("Name").GetComponent<Text>().text = lvlNameToDownload[i];
        button.transform.FindChild("Creator").GetComponent<Text>().text = lvlCreatorToDownload[i];
        button.transform.position = new Vector2(i * 300, -150);
        buttons.Add(button);

        if (File.Exists(path + "/" + lvlNameToDownload[i] + ".png"))
        {
            DownloadImageLvl(true, lvlNameToDownload[i], i);   // ajoute la vignette
        }
        else
        {
            DownloadImageLvl(false, lvlNameToDownload[i], i);   // ajoute la vignette
        }
        if (File.Exists(path + "/" + lvlNameToDownload[i] + ".lvl"))
        {
            button.SetActive(false);
        }
        else
        {                              
            button.transform.FindChild("Download").gameObject.SetActive(true);
            // button.transform.FindChild("Downloaded").gameObject.SetActive(false);
            button.transform.FindChild("ButtonPlay").gameObject.SetActive(false);
            button.transform.FindChild("ButtonErase").gameObject.SetActive(false);
            button.transform.FindChild("Download").GetComponent<Button>().onClick.AddListener(() => { DownloadLvl(lvlNameToDownload[i]); });
            button.transform.FindChild("ButtonPlay").GetComponent<Button>().onClick.AddListener(() => { LoadLevel(lvlNameToDownload[i]); });
            button.transform.FindChild("ButtonErase").GetComponent<Button>().onClick.AddListener(() => { EraseLevel(lvlNameToDownload[i], i); });
        }
    }

    public void ShowAccueil()
    {
        options.levelType = 4;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevel(string levelName)
    {
        Debug.Log("Play " + levelName);
        options.nameLevelToLoad = levelName;
        options.levelIsCustom = true;
        options.levelInTestBeforeUpload = false;
        options.levelIsCustomDownloaded = true;
        SceneManager.LoadScene("Loading");
    }

    public void EraseLevel(string levelName, int buttonNb)
    {
        MarkAsNotDownloaded(levelName);
        File.Delete(path + levelName+".png");
        File.Delete(path + levelName + ".lvl");
        Debug.Log("Erase "+path + levelName);
    }

}