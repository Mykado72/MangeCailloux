using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;

/// <summary>
/// Classe qui permet de sauvegarder et initialiser les options dans la scène MainMenu.
/// </summary>
public class OptionManager : MonoBehaviour {

    public static OptionManager optionsInstance; // Singelton.

    // Son
    public float volMusicsound;
    public float volFXsound;
    public bool usePad;
    public bool vibrate;
    public string nameLevelToLoad;
    /// <summary>
    /// Le niveau est un niveau fait par l'utilisateur de cet appareil.
    /// </summary>
    public bool levelIsCustom;
    /// <summary>
    /// Le niveau est lancé depuis l'éditeur et est en test pour savoir s'il est réalisable et peut-être mis en ligne.
    /// /// </summary>
    public bool levelInTestBeforeUpload;
    public bool levelInTestMinimalPush;
    /// <summary>
    /// Le niveau est un niveau fait par un autre utilisateur.
    /// </summary>
    public bool levelIsCustomDownloaded;
    /// <summary>
    /// Le niveau vient du panel "PanelSelectLevel" => 1
    /// Le niveau vient du panel "PanelMyLevel" => 2
    /// Le niveau vient du panel "PanelCommunityLevel" => 3
    /// Le niveau est un niveau lancé depuis l'éditeur => 4
    /// </summary>
    public int levelType;
    /// <summary>
    /// Le nom de l'utilisateur.
    /// </summary>
    public string username;
    public int pushLeft;
    public int nbOfLevelFinished;
    public int currentWorld;

    void Awake ()
    {
        if (optionsInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            optionsInstance = this;
            DontDestroyOnLoad(gameObject);
            LoadOptions();
        }
    }
    
    public void PlayStandardLevel(string levelName) {
        nameLevelToLoad = levelName;
        levelInTestBeforeUpload = false;
        levelIsCustom = false;
        levelIsCustomDownloaded = false;
    }

    public void PlayCustomLevel(string levelName) {
        nameLevelToLoad = levelName;
        levelInTestBeforeUpload = false;
        levelIsCustom = true;
        levelIsCustomDownloaded = false;
    }

    public void PlayDownloadedLevel(string levelName)
    {
        nameLevelToLoad = levelName;
        levelInTestBeforeUpload = false;
        levelIsCustom = true;
        levelIsCustomDownloaded = true;
    }

    public void TestCustomLevel(string levelName) {
        nameLevelToLoad = levelName;
        levelInTestBeforeUpload = true;
        levelIsCustom = true;
        levelIsCustomDownloaded = false;
    }

    public void LoadOptions()
    {
        // nameLevelToLoad = null; // force ouverture du MainMenu           
        levelInTestBeforeUpload = false;
        volMusicsound = PlayerPrefs.GetFloat("MusicVol", 0.8f);
        volFXsound = PlayerPrefs.GetFloat("FXVol", 0.8f);
        if (PlayerPrefs.GetInt("usePad", 1) == 1) {
            usePad = true;
        } else {
            usePad = false;
        }
        if (PlayerPrefs.GetInt("useVibration", 1) == 1) {
            vibrate = true;
        } else {
            vibrate = false;
        }
    }

    public void CalculLevelsFinishedNumber() {
        if (File.Exists(Application.persistentDataPath + "/Levelprogress")) {

            string[] lvlList = File.ReadAllLines(Application.persistentDataPath + "/Levelprogress");
            nbOfLevelFinished = 0;
            for (int line = 0; line < lvlList.Length; line++) {
                if (!lvlList[line].Contains("NbFinished") && lvlList[line][0] == 'W' //C'est un niveau d'un monde (W0-000)
                    && !lvlList[line].Contains("B") /* ce n'est pas un niveau bonus */ && lvlList[line].Split('|')[1] == "F" /* le niveau est fini*/) {                    
                    nbOfLevelFinished++;
                }
            }
        }
        else // le fichier "LevelPlayed" n'exite pas encore        
        {
            File.AppendAllText(Application.persistentDataPath + "/Levelprogress", ""); //On créé le fichier
            nbOfLevelFinished = 0;
        }
    }
}
