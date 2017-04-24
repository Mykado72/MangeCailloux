using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Equivalent à "ListLevel.cs" mais adapté pour le nouveau menu.
/// </summary>
public class DownloadLevelList : MonoBehaviour {

    private string downloadedFolderPath;
    private Dictionary<string, string> dictLvlCreator;
    private List<string>[] levels; // Tableau de listes : index 0 => nom créateur, index 1 => Nom level, index 2 => nom fichiers sur serveur.

    [SerializeField]
    private MenuManager menuManager;
    [SerializeField]
    private GameObject downloadingPanel;
    [SerializeField]
    private GameObject prefabOnlineLevel;
    [SerializeField]
    private Transform niveauxOnlineViewportContent;

    private bool needToUpdateList;

    void Awake() {
        downloadedFolderPath = Application.persistentDataPath + "/Downloaded/";
        if (!Directory.Exists(downloadedFolderPath)) { // Vérification que le dossier existe bien.
            Directory.CreateDirectory(downloadedFolderPath);
        }
        levels = new List<string>[3];
        for (int i = 0; i<levels.Length; i++) {
            levels[i] = new List<string>();
        }
        needToUpdateList = true;
    }
    
    public void NeedUpdateList() {
        needToUpdateList = true;
    }

    public void UpdateList() {
        if (needToUpdateList) {
            StartCoroutine(CoGetAllList());
        }
    }

    public void RefreshList()
    {
        StartCoroutine(CoGetAllList());
    }

    private IEnumerator CoGetAllList() {
        int t = niveauxOnlineViewportContent.childCount;
        for (int i = 0; i < t; i++) { // Destruction de tous les vieux enfants (optimisable)
            Destroy(niveauxOnlineViewportContent.GetChild(i).gameObject);
        }

        downloadingPanel.SetActive(true);
        WWW www = new WWW("http://sokobancoon.esy.es/GetLvlsList.php");
        yield return www;
        //Debug.Log(www.text);
        string[] levelsData = www.text.Remove(www.text.Length-1, 1).Replace(" ", "").Split('#');
        for (int i = 0; i< levelsData.Length; i++) {
            //Debug.Log(levelsData[i]);
            string[] levelData = levelsData[i].Split('|');
            for (int j = 0; j<levelData.Length; j++) {
                //Debug.Log(levelData[j]);
                levels[j].Add(levelData[j]);
            }

            if (!File.Exists(downloadedFolderPath + levelData[0] + "#" + levelData[1] + ".lvl")) {
                CreateButton(levelData[0], levelData[1], levelData[2]);
            }
        }

        downloadingPanel.SetActive(false);
        needToUpdateList = false;
        //print(test[0][1]); // affiche le nom du créateur (0) du seconde level des list (1).
        
    }

    void CreateButton(string creatorName, string levelName, string filesPath) {

        GameObject button = Instantiate(prefabOnlineLevel, niveauxOnlineViewportContent) as GameObject;
        Image vignette = button.transform.FindChild("Vignette").GetComponent<Image>();
        GameObject buttonPlay = button.transform.FindChild("Play").gameObject;
        //button.GetComponent<Button>().onClick.AddListener(() => { this.LoadDownloadedLevel(levelName); }); // Fonctionne bien même si ne s'affiche pas dans l'inspector.
        button.transform.FindChild("Download").GetComponent<Button>().onClick.AddListener(() => { DownLoadLvlFiles(creatorName, levelName, filesPath, vignette, buttonPlay); }); // Fonctionne bien même si ne s'affiche pas dans l'inspector.
        button.transform.FindChild("Download").GetComponent<Button>().onClick.AddListener(() => { DesactivateButton(button.transform.FindChild("Download").gameObject); }); // Fonctionne bien même si ne s'affiche pas dans l'inspector.
        button.transform.FindChild("Play").GetComponent<Button>().onClick.AddListener(() => { menuManager.LoadDownloadedLevel(creatorName+"#"+levelName); }); // Fonctionne bien même si ne s'affiche pas dans l'inspector.
        button.transform.FindChild("Creator").GetComponent<Text>().text = creatorName;
        if (creatorName == "Pitou" || creatorName == "Mykado" || creatorName == "MonsieurRaton") { // On a bien le droit de se faire remarquer ^^ (pour montrer les niveaux fait par les dev' )
            button.transform.FindChild("Creator").GetComponent<Text>().color = Color.green;
        }
        button.transform.FindChild("LevelName").GetComponent<Text>().text = levelName;
        button.transform.localScale = Vector3.one;
        StartCoroutine(CoDownloadImageLvl(creatorName, levelName, filesPath, vignette));
    }



    void DownLoadLvlFiles(string creatorName, string levelName, string serveurLevelName, Image vignette, GameObject buttonPlay) {
        StartCoroutine(CoDownloadLvl(creatorName, levelName, serveurLevelName, vignette, buttonPlay));
    }

    void DesactivateButton(GameObject thisGameObject)
    {
        thisGameObject.SetActive(false);
    }

    private IEnumerator CoDownloadLvl(string creatorName, string levelName, string serveurLevelName, Image vignette, GameObject buttonPlay) {
        if (File.Exists(downloadedFolderPath + creatorName + "#" + levelName + ".lvl")) {
            yield return null;
        } else {
            WWW www = new WWW("http://sokobancoon.esy.es/uploads/LVLS/" +serveurLevelName + ".lvl");
            yield return www;
            if (www.error == null) {
                File.WriteAllBytes(downloadedFolderPath + creatorName + "#" + levelName + ".lvl", www.bytes);
                if (!File.Exists(downloadedFolderPath + creatorName + "#" + levelName + ".png")) {
                    StartCoroutine(CoDownloadImageLvl(creatorName, levelName, serveurLevelName, vignette));
                }
                buttonPlay.SetActive(true);
                // menuManager.NeedRefreachMyLevelList();
            } else {
                Debug.Log("WWW Error: " + www.error);
            }
            www.Dispose();
        }
    }

    private IEnumerator CoDownloadImageLvl(string creatorName, string levelName, string serveurName, Image vignette) {
        if (File.Exists(downloadedFolderPath + creatorName + "#" + levelName + ".png")) {
            byte[] data = File.ReadAllBytes(downloadedFolderPath + creatorName + "#" + levelName + ".png");
            Texture2D texture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
            texture.LoadImage(data);
            vignette.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 128f);
            
            yield return null;
        } else {
            WWW www = new WWW("http://sokobancoon.esy.es/uploads/images/" + serveurName + ".png");
            yield return www;
            if (www.error == null) {
                File.WriteAllBytes(downloadedFolderPath + creatorName + "#" + levelName + ".png", www.bytes);
                vignette.sprite = Sprite.Create(www.texture, new Rect(0f, 0f, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 128f);
            } else {
                Debug.Log("WWW Error: " + www.error);
            }
            www.Dispose();
        }
    }
}
