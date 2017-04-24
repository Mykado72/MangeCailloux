using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using System.Linq;

public class LvlEditor : MonoBehaviour {
    
    public static LvlEditor mainLvlEditor; // Singelton.

    private List<EditeurCaseData> cases = new List<EditeurCaseData>();
    private List<EditeurCaseData> casesPool = new List<EditeurCaseData>();

    [HideInInspector]
    public ushort gridSizeX, gridSizeY;
    public GameObject casePrefab;
    public GameObject buttonResourceLevelPrefab;
    public GameObject buttonCustomLevelPrefab;
    public Sprite[] sprites;
    public GameObject[] panelsButtons;

    [Space]
    /*Les différents panels*/
    [Header("UI Elements")]
    public GameObject panelErreurChibisExit;
    public GameObject panelErreurNumberInterrupteur;
    public GameObject panelListeNiveaux;
    public GameObject panelSaveAs;
    public InputField levelNameInputField;
    public GameObject panelSaveOver;
    public GameObject panelConfirmExit;
    //public Toggle toggleLevelType;
    
    /*Elements d'UI*/
    private GridLayoutGroup mapGrid;
    //private RectTransform mapGridRect;
    private RectTransform gridTextAdapter;
    [SerializeField] private Text textGridHeight;
    [SerializeField] private Text textGridWidth;
    [SerializeField] private Text textnbPush;
    [SerializeField] private GameObject panelError;
    private Text errorMessage;
    [SerializeField] private Text levelNameText;

    private Transform gridPool;
    private TextAsset textFile;
    private Transform niveauxViewportContent;
    private string lvlDatas;
    private bool playAfterSave;
    private Texture2D texture2Dscreenshot;
    private Camera screenShotCam;
    [SerializeField]
    private RectTransform zoneCaptureGrille;
    private int nbPushes;
    /// <summary>
    /// Le nom du level actuellement édité
    /// </summary>
    private string currentLevelName;
    /// <summary>
    /// Le type de bloc a placer si on clique dans la grille.
    /// </summary>
    private char currentBrushType;
    /// <summary>
    /// Le level en cours d'édition à été édité.
    /// </summary>
    private bool levelWasChanged;
    private string[] levelTab;

    [SerializeField]
    private Button[] buttonsCreates;
    [SerializeField]
    private Sprite[] buttonsCreatesNormalSprite;
    [SerializeField]
    private SoundManager soundManager;
    [SerializeField]
    private AudioClip soundError;
    [SerializeField]
    private AudioClip soundNavigate;
    [SerializeField]
    private AudioClip soundClickCase;
    [SerializeField]
    private AudioClip soundClickCaseChibis;

    void Awake()
    {
        if (mainLvlEditor == null) {
            mainLvlEditor = this;
        } else {
            Debug.LogError("Il y a plusieurs scripts \"LvlEditor\" sur la scène, ce n'est pas normal !");
        }
        screenShotCam = Camera.main;
        screenShotCam.enabled = false; // Comme il n'y a que de l'UI, pas besoin d'avoir de camera active sur la scène.
    }

    void Start() {
        mapGrid = GameObject.Find("GridEditable").GetComponent<GridLayoutGroup>();
        //mapGridRect = mapGrid.GetComponent<RectTransform>();
        gridPool = GameObject.Find("Canvas/GridPool").transform;
        gridTextAdapter = GameObject.Find("Canvas/GridPanel").GetComponent<RectTransform>();
        niveauxViewportContent = panelListeNiveaux.transform.FindChild("Fond/Scroll View/Viewport/Content");
        errorMessage = panelError.transform.FindChild("FondRouge/Message").GetComponent<Text>();
        currentBrushType = '1'; //1 correspond a un mur
        panelErreurChibisExit.SetActive(false);
        panelErreurNumberInterrupteur.SetActive(false);
        panelSaveAs.SetActive(false);
        panelSaveOver.SetActive(false);
        if (OptionManager.optionsInstance.nameLevelToLoad != null && OptionManager.optionsInstance.levelInTestBeforeUpload) { // Niveau chargé précedement.
            LoadLevelCustom(OptionManager.optionsInstance.nameLevelToLoad);
        } else {
            InitialiseGrid(8, 8); // Créé une grille de 8*8
            nbPushes = 10;  // Initialise le nombre de push à 10
            textnbPush.text = "Pushes : " + nbPushes.ToString();
        }
        OpenButtonPanel(-1); // Cache tous les panels des buttons de cases.
        //SwitchLevelType();
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (panelListeNiveaux.activeInHierarchy) {
                CloseLevelList();
            } else if (!panelErreurChibisExit.activeInHierarchy && !panelErreurNumberInterrupteur.activeInHierarchy
                        && !panelError.activeInHierarchy && !panelSaveAs.activeInHierarchy
                        && !panelSaveOver.activeInHierarchy) { // Aucune erreur en cours
                WantToExit();
            }
        }
        
    }

    public void WantToExit() {

        if (levelWasChanged) { // le niveau a été modifié.
            panelConfirmExit.SetActive(true);
        } else { // sinon on quitte directement.
            Exit();
        }
    }

    void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void InitialiseGrid(ushort newSizeX, ushort newSizeY) {

        SetMapSize(newSizeX, newSizeY);

        //print("Nombre de cases necessaires : " + gridSizeX * gridSizeY);
        //print("Il faut retirer " + (cases.Count - newSizeX * newSizeY) + " cases");

        int csCount = cases.Count;
        for (int i = gridSizeX * gridSizeY; i < csCount; i++) { // Si il y a plus de cases que le nombre nécessaire.
            SendToPool(cases[0]);
        }

        for (int i = cases.Count; i < gridSizeX * gridSizeY; i++) { // Si il y a moins de cases que le nombre nécessaire.
            cases.Add(TakeFromPool());
        }

        for (ushort y = 0; y < gridSizeY; y++) {
            for (ushort x = 0; x < gridSizeX; x++) {
                int caseNumber = y * gridSizeX + x;
                //cases[caseNumber].name = "Case " + x + "," + y; //Inutile dans un build
                cases[caseNumber].GetComponent<RectTransform>().localScale = Vector3.one;
                cases[caseNumber].SetPostion(x, y);

                if (x == 0 || y == 0 || x == gridSizeX - 1 || y == gridSizeY - 1) { // C'est une bordure
                    cases[caseNumber].SetType('1', true);
                } else {
                    cases[caseNumber].SetType('0', false);
                }

            }
        }
    }

    void SetDataBack() { // Remet les anciennes données dans la nouvelle grille.
        for (ushort y = 0; y < gridSizeY; y++) {
            for (ushort x = 0; x < gridSizeX; x++) {
                int caseNumber = y * gridSizeX + x;
                // TODO : "bug" des murs du côté droit et gauche qui sont remplacés par le mur de defaut.
                //if (x != 0 && y != 0 && x != gridSizeX - 1 && y != gridSizeY - 1) { // Ce n'est pas une bordure
                if (!cases[caseNumber].isBorder) { // Ce n'est pas une bordure
                    if ((x == gridSizeX - 2 || y == gridSizeY - 2) && (levelTab[y][x] == '1' || levelTab[y][x] == 'F')) { // Ancienne bordure ou porte a remplacer par du sol
                        cases[caseNumber].SetType('0', false);
                    } else {
                        cases[caseNumber].SetType(levelTab[y][x], false); // On remet ce qu'il y avait avant d'aggrandir la map
                    }
                } else if ((x == 0 || y == 0) && x != gridSizeX - 1 && y != gridSizeY - 1) { // C'est une bordure du haut ou à gauche qui n'est pas un coin
                    cases[caseNumber].SetType(levelTab[y][x], true); // On remet ce qu'il y avait avant d'aggrandir la map
                }
            }
        }
        UpdateLevelTab();
    }

    void SetMapSize(ushort newSizeX, ushort newSizeY) {
        gridSizeX = (ushort)Mathf.Clamp(newSizeX, 6, 25);
        gridSizeY = (ushort)Mathf.Clamp(newSizeY, 6, 25);
        textGridHeight.text = "height of the grid : " + gridSizeY;
        textGridWidth.text = "width of the grid : " + gridSizeX;
        float ratio = (float)gridSizeX / (float)gridSizeY;
        ((RectTransform)mapGrid.transform).sizeDelta = new Vector2(800f * ratio, 800f);
        gridTextAdapter.sizeDelta = new Vector2(800f * ratio, 800f);
        mapGrid.cellSize = new Vector2(800f * ratio / (float)gridSizeX, 800f / (float)gridSizeY);
    }

    EditeurCaseData TakeFromPool() {
        EditeurCaseData temp;
        if (casesPool.Count > 0) { // il y a des objets dans la réserve.
            temp = casesPool[0];
            casesPool.RemoveAt(0);
        } else {
            GameObject sp = Instantiate(casePrefab, mapGrid.transform) as GameObject;
            temp = sp.GetComponent<EditeurCaseData>();
        }
        temp.transform.SetParent(mapGrid.transform);
        return temp;
    }

    void SendToPool(EditeurCaseData objectToSend) {
        objectToSend.Active(false);
        objectToSend.transform.SetParent(gridPool);
        cases.Remove(objectToSend);
        casesPool.Add(objectToSend);
        //objectToSend.name = "Case dans la piscine"; //Inutile dans un build
    }

    /// <summary>
    /// Appelée lors d'un clic sur une image composant la grille de création du niveau.
    /// </summary>
    public void ImageGridClick(EditeurCaseData caseClicked) {
        //Debug.Log("Clic en X : " + caseClicked.positionX + " Y : " + caseClicked.positionY + " brosse : " + currentBrushType);
        if ((caseClicked.positionX == 0 && caseClicked.positionY == 0) ||
            (caseClicked.positionX == gridSizeX - 1 && caseClicked.positionY == 0) ||
            (caseClicked.positionX == gridSizeX - 1 && caseClicked.positionY == gridSizeX - 1) ||
            (caseClicked.positionX == 0 && caseClicked.positionY == gridSizeX - 1)) { // Si la case est un coin.

            if (currentBrushType != '1' && currentBrushType != 'G' && currentBrushType != 'X' 
             && currentBrushType != 'I' && currentBrushType != 'H' && currentBrushType != 'J'
             && currentBrushType != 'ä' && currentBrushType != 'â' && currentBrushType != 'à' && currentBrushType != '+') { // La case veut être remplie avec ni du vide ni du mur
                return; // On ignore le clique
            }

        } else if (caseClicked.isBorder && currentBrushType != 'F' && currentBrushType != '1' && currentBrushType != 'G'
             && currentBrushType != 'X' && currentBrushType != 'I' && currentBrushType != 'H' && currentBrushType != 'J'
             && currentBrushType != 'ä' && currentBrushType != 'â' && currentBrushType != 'à' && currentBrushType != '+') { // Ni du vide ni du mur
            return; // On ignore le clique
        } else if (!caseClicked.isBorder && currentBrushType == 'F') { // On veut placer une sortie ailleur que sur un bord.
            return; // On ignore le clique
        }

        if (caseClicked.isBorder && currentBrushType == 'F') {

            foreach (EditeurCaseData c in cases) {
                if (c.isBorder && c.type == 'F') {
                    c.SetType('1', true);
                }
            }

        } else if (currentBrushType == '2') {

            foreach (EditeurCaseData c in cases) {
                if (c.type == '2') {
                    c.SetType('0', false);
                }
            }

        }

        caseClicked.SetType(currentBrushType, caseClicked.isBorder);
        levelWasChanged = true;
        if (currentBrushType == '2') {
            soundManager.PlayFXSound(soundClickCaseChibis);
        } else {
            soundManager.PlayFXSound(soundClickCase);
        }
    }

    /// <summary>
    /// Permet de changer l'objet a placer dans la grille. Appellée principalement par les boutons de l'UI.
    /// </summary>
    /// <param name="index">L'index du type de bloc qui sera à placer</param>
    public void SetCurrentBrush(string id, Button test, short panel, Sprite normalSprite) {
        currentBrushType = id[0];

        SpriteState spst = new SpriteState();
        spst.highlightedSprite = test.spriteState.highlightedSprite;
        spst.pressedSprite = test.spriteState.pressedSprite;

        if (panel >= 0) {
            buttonsCreates[panel].spriteState = spst;
            buttonsCreates[panel].GetComponent<Image>().sprite = spst.highlightedSprite;
            buttonsCreatesNormalSprite[panel] = normalSprite;
        }
        for(int i = 0; i < buttonsCreatesNormalSprite.Length; i++) {
            if (i != panel) {
                buttonsCreates[i].GetComponent<Image>().sprite = buttonsCreatesNormalSprite[i];
            }
        }
        OpenButtonPanel(-1); // Cache tous les panels des buttons de cases.
    }

    public void ModifyXSize(int number) {
        UpdateLevelTab();
        InitialiseGrid((ushort)(gridSizeX + number), gridSizeY);
        SetDataBack();
        levelWasChanged = true;
    }

    public void ModifyYSize(int number) {
        UpdateLevelTab();
        InitialiseGrid(gridSizeX, (ushort)(gridSizeY + number));
        SetDataBack();
        levelWasChanged = true;
    }

    public void ModifNbPushes(int number)
    {
        nbPushes = Mathf.Clamp(nbPushes + number, 0, 999); // Limitation du nombre de "Pushes" entre 0 et 999.
        textnbPush.text = "Pushes : " + nbPushes.ToString();
        levelWasChanged = true;
    }

    public void ResetMap() {
        InitialiseGrid(gridSizeX, gridSizeY);
    }

    /// <summary>
    /// Ouvre (ou ferme) les panels contenant les bouttons des différents cases.
    /// </summary>
    /// <param name="id">Le numéro du panel à ouvrir. Une valeur négative permet de tous les cacher.</param>
    public void OpenButtonPanel(int id) {
        for (int i = 0; i < panelsButtons.Length; i++) {
            panelsButtons[i].SetActive(false);
        }
        if (id >= 0 && id < panelsButtons.Length) {
            panelsButtons[id].SetActive(true);
        }
    }

    /// <summary>
    /// Crée un fichier .txt contenant les infos pour ensuite générer le lvl en 3D
    /// </summary>
    public void ExportTextFile(string lvl) {

        string path = Application.persistentDataPath + "/" + currentLevelName + ".lvl";
        lvl = OurUtilitysFonctions.EncryptText(lvl, "ChibIStheBEst");        
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs)) {
                writer.WriteLine(OurUtilitysFonctions.EncryptText(nbPushes.ToString(), "ChibIStheBEst"));
                writer.Write(lvl);
            }
        }
    }

    /*public void ExportNbPush(int nb)
    {
        string path = Application.persistentDataPath + "/nbpush.csv";
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            { 
                for (int i = 0; i < nb; i++)
                {
                    writer.Write(i.ToString() + ";");
                    writer.WriteLine(OurUtilitysFonctions.EncryptText(i.ToString(), "ChibIStheBEst"));
                }
            }
        }
    }*/

    /// <summary>
    /// Charge un niveau dans l'éditeur depuis un .txt du dossier ressources
    /// </summary>
    /// <param name="name">Nom du fichier (sans .txt)</param>
    public void LoadLevelResource(string name) {

        soundManager.PlayFXSound(soundNavigate);
        currentLevelName = name;
        levelNameText.text = currentLevelName;
        textFile = (TextAsset)Resources.Load(name);
        StringReader file = new StringReader(textFile.text); 
        // la 1ere fois pour calculer le nombre de ligne du fichier
        if (!int.TryParse(OurUtilitysFonctions.DecryptText(file.ReadLine(), "ChibIStheBEst"), out nbPushes))
        {
            nbPushes = 15;
        } 
        textnbPush.text = "Pushes : " + nbPushes.ToString();
        string lvlData = OurUtilitysFonctions.DecryptText(file.ReadToEnd(), "ChibIStheBEst");
        file.Close();
        ushort newSizeX = 0;
        ushort newSizeY = 0;
        string line;
        StringReader str = new StringReader(lvlData);
        while ((line = str.ReadLine()) != null)
        {
            newSizeY++;
            if (line.Length > newSizeX)
                newSizeX = (ushort)line.Length;
        }
        str.Close();
        SetMapSize(newSizeX, newSizeY);

        int csCount = cases.Count;
        for (int i = gridSizeX * gridSizeY; i < csCount; i++)
        { // Si il y a plus de cases que le nombre nécessaire.
            SendToPool(cases[0]);
        }

        for (int i = cases.Count; i < gridSizeX * gridSizeY; i++)
        {
            cases.Add(TakeFromPool());
        }

        StringReader str2 = new StringReader(lvlData);
        int lineY = 0;
        while ((line = str2.ReadLine()) != null)
        {

            for (ushort x = 0; x < gridSizeX; x++)
            {

                int caseNumber = lineY * gridSizeX + x;
                //caseToSetup.name = "Case " + x + "," + y; //Inutile dans un build
                cases[caseNumber].GetComponent<RectTransform>().localScale = Vector3.one;
                cases[caseNumber].SetPostion(x, (ushort)lineY);

                if (x == 0 || lineY == 0 || x == gridSizeX - 1 || lineY == gridSizeY - 1)
                { // C'est une bordure
                    cases[caseNumber].SetType(line[x], true);
                }
                else
                {
                    cases[caseNumber].SetType(line[x], false);
                }

            }
            lineY++;
        }
        str2.Close();
        panelListeNiveaux.SetActive(false);
        levelWasChanged = false;
    }

    /// <summary>
    /// Charge un niveau dans l'éditeur depuis un .txt du dossier ressources
    /// </summary>
    /// <param name="name">Nom du fichier (sans .txt)</param>
    public void LoadLevelCustom(string name) {

        soundManager.PlayFXSound(soundNavigate);
        if (!File.Exists(Application.persistentDataPath + "/" + name + ".lvl")) {
            //LoadLevelRessources("default");
            return;
        }
        currentLevelName = name;
        levelNameText.text = currentLevelName;
        StreamReader textFile = new StreamReader(Application.persistentDataPath + "/" + name + ".lvl");
        if (!int.TryParse(OurUtilitysFonctions.DecryptText(textFile.ReadLine(), "ChibIStheBEst"),out nbPushes))
        {
            nbPushes = 15;
        }
        textnbPush.text = "Pushes : " + nbPushes.ToString();
        string lvlData = OurUtilitysFonctions.DecryptText(textFile.ReadToEnd(), "ChibIStheBEst"); 
        textFile.Close();
        
        ushort newSizeX = 0;
        ushort newSizeY = 0;
        string line;
        StringReader str = new StringReader(lvlData);

        while ((line = str.ReadLine()) != null) {
            newSizeY++;
            if (line.Length > newSizeX)
                newSizeX = (ushort)line.Length;
        }
        str.Close();
        SetMapSize(newSizeX, newSizeY);

        int csCount = cases.Count;
        for (int i = gridSizeX * gridSizeY; i < csCount; i++) { // Si il y a plus de cases que le nombre nécessaire.
            SendToPool(cases[0]);
        }

        for (int i = cases.Count; i < gridSizeX * gridSizeY; i++) {
            cases.Add(TakeFromPool());
        }

        StringReader str2 = new StringReader(lvlData);
        int lineY = 0;
        while ((line = str2.ReadLine()) != null) {

            for (ushort x = 0; x < gridSizeX; x++) {

                int caseNumber = lineY * gridSizeX + x;
                //caseToSetup.name = "Case " + x + "," + y; //Inutile dans un build
                cases[caseNumber].GetComponent<RectTransform>().localScale = Vector3.one;
                cases[caseNumber].SetPostion(x, (ushort)lineY);

                if (x == 0 || lineY == 0 || x == gridSizeX - 1 || lineY == gridSizeY - 1) { // C'est une bordure
                    cases[caseNumber].SetType(line[x], true);
                } else {
                    cases[caseNumber].SetType(line[x], false);
                }

            }
            lineY++;
        }
        str2.Close();
        panelListeNiveaux.SetActive(false);
        levelWasChanged = false;
    }

    public void LoadDefaultLevel() {
        LoadLevelResource("default");
    }

    public void OpenLevelList() {
        
        for (int i = niveauxViewportContent.childCount - 1; i > 0; i--) { // Suppression de tous les bouttons (sauf celui par defaut)
            DestroyImmediate(niveauxViewportContent.GetChild(i).gameObject);
        }
        panelListeNiveaux.SetActive(true);
        string[] list = Directory.GetFiles(Application.persistentDataPath);
        for (int i = 0; i < list.Length; i++) {
            list[i] = list[i].Replace(Application.persistentDataPath, "");
            list[i] = list[i].Replace(".txt", "");
            list[i] = list[i].Replace("\\", "");
            list[i] = list[i].Replace("/", "");
            if (list[i].EndsWith(".lvl")) {
                GameObject button = Instantiate(buttonCustomLevelPrefab, niveauxViewportContent) as GameObject;
                button.GetComponentInChildren<Text>().text = list[i].Replace(".lvl", "");
            }
        }
    }
    
    public void CloseLevelList() {
        panelListeNiveaux.SetActive(false);
    }

    void UpdateLevelTab() {
        levelTab = new string[gridSizeY];
        for (int i = 0; i < gridSizeY; i++) {
            levelTab[i] = "";
        }

        int Y = 0;
        for (int i = 0; i < gridSizeX * gridSizeY; i++) {
            levelTab[Y] += mapGrid.transform.GetChild(i).GetComponent<EditeurCaseData>().type;
            if (i % gridSizeX == gridSizeX - 1) {
                Y++;
            }
        }



    }

    public void Btn_SaveAs(bool play) {
        
        playAfterSave = play;
        //On génère la string
        lvlDatas = "";
        UpdateLevelTab();
        for (int i = 0; i < gridSizeY; i++) {
            lvlDatas += levelTab[i] + System.Environment.NewLine;            
        }
        //print(lvlDatas);
        if (!lvlDatas.Contains("2") || !lvlDatas.Contains("F")) {
            //Debug.Log("Impossible de sauvegarder le niveau car il n'y a pas de sortie ou pas de chibis !");
            panelErreurChibisExit.SetActive(true);
            soundManager.PlayFXSound(soundError);
            return;
        }

        // Attention, code pas très joli... Rempli les tableaux avec les nombres de caisses et d'interrupteurs pour ensuite tester si le niveau est réalisable.
        //  '5' caisse marron | '6' caisse rouge | '7' caisse orange | '8' caisse Verte | '9' caisse Bleue |  caisse Jaune
        //  'A' bouton marron | 'D' bouton rouge | 'C' bouton orange | 'E' bouton vert  | 'B' bouton bleu  | 
        //  'a' marron marron | 'f' c roug b mar | 'k' orange marron | 'p' verte - marr | 'u' bleue marron | '%' jaune marron  
        //  'b' marron rouge  | 'i' rouge rouge  | 'n' orange rouge  | 's' verte rouge  | 'x' bleue rouge  | '$' jaune rouge 
        //  'c' marron orange | 'h' rouge orange | 'm' orange orange | 'r' verte orange | 'w" bleue orange | '#' jaune orange
        //  'd' marron vert   | 'j' rouge vert   | 'o' orange vert   | 't' verte vert   | 'y' bleue vert   | '!' jaune vert
        //  'e' marron bleu   | 'g' rouge bleu   | 'l' orange bleu   | 'q' verte bleu   | 'v' bleue bleu   | '&" jaune bleu

        int[] caissesCount = new int[5];
        int[] interrpteursCount = new int[5];

        caissesCount[0] = lvlDatas.Count(f => (f == '5') || (f == 'a') || (f == 'b') || (f == 'c') || (f == 'd') || (f == 'e')); //Nombre de caisses marrons.
        interrpteursCount[0] = lvlDatas.Count(f => (f == 'A') || (f == 'a') || (f == 'f') || (f == 'k') || (f == 'p') || (f == 'u') || (f == '%')); //Nombre d'interrupteurs marrons.

        caissesCount[1] = lvlDatas.Count(f => (f == '6') || (f == 'f') || (f == 'g') || (f == 'h') || (f == 'i') || (f == 'j')); //Nombre de caisses rouges.
        interrpteursCount[1] = lvlDatas.Count(f => (f == 'D') || (f == 'd') || (f == 'i') || (f == 'n') || (f == 's') || (f == 'x') || (f == '$')); //Nombre d'interrupteurs rouges.


        caissesCount[2] = lvlDatas.Count(f => (f == '7') || (f == 'k') || (f == 'l') || (f == 'm') || (f == 'n') || (f == 'o')); //Nombre de caisses oranges.        
        interrpteursCount[2] = lvlDatas.Count(f => (f == 'C') || (f == 'c') || (f == 'h') || (f == 'm') || (f == 'r') || (f == 'w') || (f == '#')); //Nombre d'interrupteurs oranges.

        caissesCount[3] = lvlDatas.Count(f => (f == '8') || (f == 'p') || (f == 'q') || (f == 'r') || (f == 's') || (f == 't')); //Nombre de caisses vertes.
        interrpteursCount[3] = lvlDatas.Count(f => (f == 'E') || (f == 'e') || (f == 'j') || (f == 'o') || (f == 't') || (f == 'y') || (f == '!')); //Nombre d'interrupteurs verts.

        caissesCount[4] = lvlDatas.Count(f => (f == '9') || (f == 'u') || (f == 'v') || (f == 'w') || (f == 'x') || (f == 'y')); //Nombre de caisses bleues.
        interrpteursCount[4] = lvlDatas.Count(f => (f == 'B') || (f == 'b') || (f == 'g') || (f == 'l') || (f == 'q') || (f == 'v') || (f == '&')); //Nombre d'interrupteurs bleus.

        for (int i = 0; i<5; i++) {
            //Debug.Log(i + " caisses:" + caissesCount[i] + " inter:" + interrpteursCount[i]);
            if (caissesCount[i] < interrpteursCount[i]) { // le nombre de caisses d'une couleur >= nombre d'interrupteurs de la même couleur
                panelErreurNumberInterrupteur.SetActive(true);
                soundManager.PlayFXSound(soundError);
                return;
            }
        }

        if (play && !levelWasChanged) {
            OptionManager.optionsInstance.TestCustomLevel(currentLevelName);
            SceneManager.LoadScene("Loading");
        } else {
            levelNameInputField.text = currentLevelName;
            panelSaveAs.SetActive(true);
        }
    }

    public void ValidateName() {
        currentLevelName = levelNameInputField.text;
        if (currentLevelName.Contains("/") || currentLevelName.Contains("\\")
         || currentLevelName.Contains("|") || currentLevelName.Contains("#")
         || currentLevelName.Contains(" ") || currentLevelName.Contains("?")) { //Le nom n'est pas valide
            ShowError("This name contains invalid caraters!");
            return;
        } else if (currentLevelName.Length < 4) {
            ShowError("This name is too short! It needs at least 4 character.");
            return;
        } else if (currentLevelName.Length > 16) {
            ShowError("This name is too long! You can't put more than 16 characters!");
            return;
        }

        if (!File.Exists(Application.persistentDataPath + "/" + currentLevelName + ".lvl")) {
            SaveOver();
            panelSaveAs.SetActive(false);
        } else {
            panelSaveAs.SetActive(false);
            panelSaveOver.SetActive(true);
        }

    }

    public void SaveOver() {
        panelSaveOver.SetActive(false);
        ExportTextFile(lvlDatas);
        TakeScreenShot();
        levelWasChanged = false;
        if (playAfterSave) {
            OptionManager.optionsInstance.TestCustomLevel(currentLevelName);
            SceneManager.LoadScene("Loading");
        }
    }
    
    public void TakeScreenShot()
    {
        Transform gridParent = mapGrid.transform.parent;
        /*Mise en place de la grille à la bonne position*/
        mapGrid.transform.SetParent(zoneCaptureGrille);
        zoneCaptureGrille.GetComponent<AspectRatioFitter>().aspectRatio = (float)gridSizeX / (float)gridSizeY;
        mapGrid.transform.localScale = Vector3.one;
        mapGrid.transform.localPosition = Vector3.one;
        ((RectTransform)mapGrid.transform).anchorMax = Vector2.one;
        ((RectTransform)mapGrid.transform).anchorMin = Vector2.zero;
        ((RectTransform)mapGrid.transform).anchoredPosition = Vector2.zero;
        ((RectTransform)mapGrid.transform).sizeDelta = Vector2.zero;

        Vector2 cellSize;
        if (gridSizeX >= gridSizeY) {
            cellSize = new Vector2(Screen.height / (float)gridSizeX, Screen.height / (float)gridSizeX);
        } else {
            cellSize = new Vector2(Screen.height / (float)gridSizeY, Screen.height / (float)gridSizeY);
        }
        mapGrid.cellSize = cellSize; 

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32); // la texture fera la taille de l'écran
        renderTexture.Create();
        screenShotCam.enabled = true;
        screenShotCam.targetTexture = renderTexture;  // defini la texture en cible pour la camera
        screenShotCam.Render(); // Fait un rendu de la Camera.

        RenderTexture.active = screenShotCam.targetTexture;
        texture2Dscreenshot = new Texture2D(Screen.height, Screen.height, TextureFormat.ARGB32, false);  // taille du snapshoot carré de la hauteur de l'écran
        texture2Dscreenshot.ReadPixels(new Rect(0, 0, Screen.height, Screen.height), 0, 0);
        texture2Dscreenshot.Apply();
        texture2Dscreenshot = ScaleTexture(texture2Dscreenshot, 256, 256); // Redimentionnement de l'image.

        // Encode texture into PNG
        byte[] bytes = texture2Dscreenshot.EncodeToPNG();
        string path = Application.persistentDataPath + "/" + currentLevelName + ".png";
        using (FileStream fs = new FileStream(path, FileMode.Create)) {
            using (StreamWriter writer = new StreamWriter(fs)) {
                var binary = new BinaryWriter(fs);
                binary.Write(bytes);
                fs.Close();
            }
        }

        screenShotCam.targetTexture = null;
        screenShotCam.enabled = false;

        /*Remise en place de la grille sur le canvas principal*/
        mapGrid.transform.SetParent(gridParent);
        // La grille doit être la 3ème dans la Hierarchy pour ne pas être devant d'autres element
        mapGrid.transform.SetSiblingIndex(2);
        mapGrid.transform.localScale = Vector3.one;
        mapGrid.transform.localPosition = Vector3.one;
        ((RectTransform)mapGrid.transform).anchorMax = ((RectTransform)mapGrid.transform).anchorMin = new Vector2(0, 1);
        ((RectTransform)mapGrid.transform).anchoredPosition = new Vector3(100, -100);
        SetMapSize(gridSizeX, gridSizeY);

        //Debug.Log("Capture du niveau prise !");
    }
    
    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        //float incX = (1.0f / (float)targetWidth);
        //float incY = (1.0f / (float)targetHeight);
        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }

    /*public void SwitchLevelType()
    {
        if (toggleLevelType.isOn)
        {
            toggleLevelType.GetComponentInChildren<Text>().text = "Resource";
            OptionManager.optionsInstance.levelIsCustom = false;
        }            
        else
        {
            toggleLevelType.GetComponentInChildren<Text>().text = "Custom";
            OptionManager.optionsInstance.levelIsCustom = true;
        }
    }*/

    void ShowError(string _errorMessage) {
        panelError.SetActive(true);
        errorMessage.text = _errorMessage;
        soundManager.PlayFXSound(soundError);
    }
    

}
