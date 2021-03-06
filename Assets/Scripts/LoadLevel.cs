using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class LoadLevel : MonoBehaviour {
    private GameObject goLevel3D;
    public GameObject[] listeItems;
    public GameObject[,,] tabLevel3d;
    //  public GameObject[][][] jaggedtabLevel3d; 
    private List<Caracteristics> listMoveableObjectCaract;
    private List<Caracteristics> golistInter;
    private int scale = 2;
    private string line;
    private int counterZ = 0;
    private int posX = 0;
    private int lineZ;
    private GameObject door;
    private TextAsset PrnFile;
    private Vector3 doorPosition;
    private string MD5string;
    private OptionManager options;
    [SerializeField] private Text levelNameText;

    void Awake() {
        doorPosition = Vector3.zero;
    }

    void Start() {
        options = OptionManager.optionsInstance;
        goLevel3D = gameObject; // object parent du Niveau
        listMoveableObjectCaract = GameObject.Find("Managers").GetComponent<MovementManager>().listMoveableObjectCaracts;
        golistInter = GameObject.Find("Managers").GetComponent<LogicManager>().listInter;

        if (options.levelIsCustom == false) { // C'est un niveau fait par nous.
            LoadInResources(options.nameLevelToLoad);
        } else if (File.Exists(Application.persistentDataPath + "/" + options.nameLevelToLoad + ".lvl") || File.Exists(Application.persistentDataPath + "/Downloaded/" + options.nameLevelToLoad + ".lvl")) { // Le fichier personnalisé existe bien.
            LoadInpersistentDataPath(options.nameLevelToLoad);
        } else {
            Debug.LogError("Le fichier est introuvable. Ce n'est pas logique, il y a un petit soucis de code quelque part ! :P");
            UnityEngine.SceneManagement.SceneManager.LoadScene(0); // Retour au menu.
        }

        Movements.InitializeDatas(tabLevel3d, listeItems, GameObject.FindGameObjectWithTag("Player")); // On met à jour les données dans le script static (c'est peut-être pas la meilleure façon de faire).
                                                                                                       //       Movements.InitializeDatasjagged(jaggedtabLevel3d,posX,counterZ); 
        RotateDoor();

        // Affichage du nom du lvl chargé.
        if (OptionManager.optionsInstance.levelIsCustomDownloaded) {
            levelNameText.text = options.nameLevelToLoad.Split('#')[1] + " by " + options.nameLevelToLoad.Split('#')[0];
        } else {
            levelNameText.text = options.nameLevelToLoad;
        }

        Destroy(this); // Le script deviens inutile
    }

    private void LoadInpersistentDataPath(string fileName) {
        StreamReader textFile;
        if (options.levelIsCustomDownloaded == true) {
            textFile = new StreamReader(Application.persistentDataPath + "/Downloaded/" + fileName + ".lvl");
        } else {
            textFile = new StreamReader(Application.persistentDataPath + "/" + fileName + ".lvl");
        }
        if (!int.TryParse(OurUtilitysFonctions.DecryptText(textFile.ReadLine(), "ChibIStheBEst"), out options.pushLeft)) {
            options.pushLeft = 15;
        }
        counterZ++;
        string lvlData = OurUtilitysFonctions.DecryptText(textFile.ReadToEnd(), "ChibIStheBEst");
        textFile.Close();
        StringReader file1 = new StringReader(lvlData);
        while ((line = file1.ReadLine()) != null) {
            counterZ++;
            if (line.Length > posX)
                posX = line.Length;   // posX = nombre max de caractères par ligne
        }
        Debug.Log("Niveau : X:" + posX + " Y:" + counterZ);
        tabLevel3d = new GameObject[posX, 3, counterZ];   // déclaration du tableau avec les bonnes dimensions.
                                                          /*     jaggedtabLevel3d = new GameObject[posX][][];
                                                                 for (int x = 0; x < posX; x++)
                                                                 {
                                                                     jaggedtabLevel3d[x] = new GameObject[3][];
                                                                     for (int y = 0; y < 3; y++)
                                                                     {
                                                                         jaggedtabLevel3d[x][y] = new GameObject[counterZ];
                                                                     }
                                                                 } */
        lineZ = counterZ - 1;
        file1.Close();    // ferme le fichier1
        counterZ = 0;  // on recommence
        CreateLevel(new StringReader(lvlData));
    }

    private void LoadInResources(string fileName) {
        // on va lire 2 fois le fichiers
        PrnFile = (TextAsset) Resources.Load(fileName);

        StringReader textFile = new StringReader(PrnFile.text);
        if (!int.TryParse(OurUtilitysFonctions.DecryptText(textFile.ReadLine(), "ChibIStheBEst"), out options.pushLeft)) {
            options.pushLeft = 15;
        }
        counterZ++;
        string lvlData = OurUtilitysFonctions.DecryptText(textFile.ReadToEnd(), "ChibIStheBEst");
        textFile.Close();
        StringReader file1 = new StringReader(lvlData);
        while ((line = file1.ReadLine()) != null) {
            counterZ++;
            if (line.Length > posX)
                posX = line.Length;   // posX = nombre max de caractères par ligne
        }
        //Debug.Log("Niveau : X:" + posX + " Y:" + counterZ);
        tabLevel3d = new GameObject[posX, 3, counterZ];   // déclaration du tableau avec les bonnes dimensions.
                                                          /*       jaggedtabLevel3d = new GameObject[posX][][];
                                                                 for (int x = 0; x < posX; x++)
                                                                 {
                                                                     jaggedtabLevel3d[x] = new GameObject[3][];
                                                                     for (int y = 0; y < 3; y++)
                                                                     {
                                                                         jaggedtabLevel3d[x][y] = new GameObject[counterZ];
                                                                     }
                                                                 } */

        lineZ = counterZ - 1;
        file1.Close();    // ferme le fichier1
        counterZ = 0;  // on recommence
        CreateLevel(new StringReader(lvlData));
    }

    private void CreateLevel(StringReader lvlData) {
        GameObject goY0 = new GameObject("Y0");
        goY0.transform.parent = goLevel3D.transform;

        GameObject objectSol = null;  // Utile lorsqu'il faut mettre un sol sous l'objet instancié.
        GameObject objectIntancied = null;
        Caracteristics objectIntanciedCaracteristics = null;
        Caracteristics.types objectType;
        while ((line = lvlData.ReadLine()) != null) {  // tant qu'on est pas à la fin du fichier on boucle
            GameObject goZ = new GameObject("Z" + (lineZ - counterZ).ToString());
            goZ.transform.parent = goY0.transform;

            for (int counterX = 0; counterX < line.Length; counterX++) {   // on boucle sur chaque caractère de la ligne
                int typeObject = OurUtilitysFonctions.CharToInt(line[counterX]);          // le caractère 0 = 48 en ACSII

                if ((typeObject < 35 || typeObject > 66) && typeObject < listeItems.Length) { // L'objets n'est pas une caisse sur un interrupteur.
                    objectIntancied = Instantiate(listeItems[typeObject], goZ.transform);
                    objectIntanciedCaracteristics = objectIntancied.GetComponent<Caracteristics>();
                    objectType = objectIntanciedCaracteristics.whatIsIt;

                    switch (objectType) {
                        case Caracteristics.types.Door:
                            door = objectIntancied;  // necessaire pour faire la rotation plus tard
                            doorPosition = new Vector3(counterX, 0, lineZ - counterZ);
                            tabLevel3d[counterX, 0, lineZ - counterZ] = objectIntancied;    // on ajout l'objet dans le tableau
                                                                                            // jaggedtabLevel3d[counterX][0][lineZ - counterZ] = objectIntancied;   
                            break;
                        case Caracteristics.types.Player:
                            objectIntancied.name = "Player";
                            objectSol = Instantiate(listeItems[0], goZ.transform) as GameObject;
                            objectSol.transform.localPosition = new Vector3(counterX * scale, 0.0f, (lineZ - counterZ) * scale);
                            objectSol.name = "X" + counterX + " Z" + (lineZ - counterZ) + " " + listeItems[0].name;
                            tabLevel3d[counterX, 0, lineZ - counterZ] = objectSol;          // on ajout un objet sol dans le tableau à la dimension Y=0   
                                                                                            // jaggedtabLevel3d[counterX][0][lineZ - counterZ] = objectSol;
                                                                                            // tabLevel3d[counterX, 1, lineZ - counterZ] = objectIntancied;    // on ajout le joueur dans le tableau à la dimension Y=1
                            break;
                        case Caracteristics.types.Inter:
                            golistInter.Add(objectIntanciedCaracteristics); // c'est un Inter ajoute à la liste des Inter
                            objectIntanciedCaracteristics.goWhereObjectIs = objectIntancied;
                            tabLevel3d[counterX, 0, lineZ - counterZ] = objectIntancied;    // on ajout l'objet dans le tableau
                            break;
                        default:
                            if (objectType != Caracteristics.types.Sol) {
                                if (objectIntanciedCaracteristics.b_CanBePush || objectIntanciedCaracteristics.b_CanBeWalkOn) { // si l'objet peut être poussé il faut ajouter un sol en dessous et l'ajouté à la liste des object qui peuvent se déplacer
                                    if (!objectIntancied.CompareTag("Player") && objectIntanciedCaracteristics.b_CanBePush == true) {
                                        listMoveableObjectCaract.Add(objectIntanciedCaracteristics); // on ajoute l'objet à la liste des objects déplaçables.
                                    }
                                    objectSol = Instantiate(listeItems[0], goZ.transform) as GameObject;
                                    objectSol.transform.localPosition = new Vector3(counterX * scale, 0.0f, (lineZ - counterZ) * scale);
                                    objectSol.name = "X" + counterX + " Z" + (lineZ - counterZ) + " " + listeItems[0].name;
                                    objectIntanciedCaracteristics.goBeforeThisObject = (GameObject)objectSol;
                                    objectIntanciedCaracteristics.goWhereObjectIs = (GameObject)objectSol;
                                }
                            }
                            tabLevel3d[counterX, 0, lineZ - counterZ] = objectIntancied;    // on ajout l'objet dans le tableau
                                                                                            // jaggedtabLevel3d[counterX][0][lineZ - counterZ] = objectIntancied;  
                            break;
                    }
                    objectIntancied.transform.localPosition = new Vector3(counterX * scale, 0.0f, (lineZ - counterZ) * scale);
                    objectIntancied.name = "X" + counterX + " Z" + (lineZ - counterZ) + " " + listeItems[typeObject].name;
                    // objectIntancied.transform.parent = goY0.transform;

                } else if (typeObject >= 36 && typeObject <= 66 && typeObject != 61) { // L'ojets a créer est une caisse sur un interrupteur. (l'item 61 n'est pas une caisse sur inter)

                    objectIntancied = Instantiate(listeItems[OurUtilitysFonctions.CaisseFromCaisseSurInter(typeObject)], goZ.transform);
                    objectIntancied.transform.localPosition = new Vector3(counterX * scale, 0.0f, (lineZ - counterZ) * scale);
                    objectIntancied.name = "X" + counterX + " Z" + (lineZ - counterZ) + " " + listeItems[OurUtilitysFonctions.CaisseFromCaisseSurInter(typeObject)].name;

                    objectIntanciedCaracteristics = objectIntancied.GetComponent<Caracteristics>();

                    GameObject interrupteur = Instantiate(listeItems[OurUtilitysFonctions.InterFromCaisseSurInter(typeObject)], goZ.transform) as GameObject;
                    golistInter.Add(interrupteur.GetComponent<Caracteristics>()); // c'est un Inter ajoute à la liste des Inter
                    
                    interrupteur.transform.localPosition = new Vector3(counterX * scale, 0.0f, (lineZ - counterZ) * scale);
                    interrupteur.name = "X" + counterX + " Z" + (lineZ - counterZ) + " " + listeItems[OurUtilitysFonctions.InterFromCaisseSurInter(typeObject)].name;

                    objectIntanciedCaracteristics.goWhereObjectIs = interrupteur;
                    objectIntanciedCaracteristics.goBeforeThisObject = interrupteur;
                    tabLevel3d[counterX, 0, lineZ - counterZ] = objectIntancied;    // on ajout l'objet dans le tableau
                    listMoveableObjectCaract.Add(objectIntanciedCaracteristics);



                } else { // le caractère est au dela de la liste et des caises sur interrupteurs
                    tabLevel3d[counterX, 0, lineZ - counterZ] = null;   // on met du vide
                    Debug.LogWarning("Hey ! Le caractère " + typeObject + " dans le fichier est plus grand que la liste d'objet.");
                    // jaggedtabLevel3d[counterX][0][lineZ - counterZ] = null;
                }
            }


            counterZ++;
        }
        lvlData.Close();
    }

    void RotateDoor() {
        bool m_mustRotateDoor = false;
        Vector3 m_doorPosition_zmin = OurUtilitysFonctions.ClampPosition(new Vector3(doorPosition.x, doorPosition.y, doorPosition.z - 1));
        Vector3 m_doorPosition_zmax = OurUtilitysFonctions.ClampPosition(new Vector3(doorPosition.x, doorPosition.y, doorPosition.z + 1));
        if (tabLevel3d[(int)doorPosition.x, 0, (int)m_doorPosition_zmin.z] != null) {
            if (tabLevel3d[(int)doorPosition.x, 0, (int)m_doorPosition_zmin.z].tag == "Mur")
                m_mustRotateDoor = true;
        }
        if (tabLevel3d[(int)doorPosition.x, 0, (int)m_doorPosition_zmax.z] != null) {
            if (tabLevel3d[(int)doorPosition.x, 0, (int)m_doorPosition_zmax.z].tag == "Mur")
                m_mustRotateDoor = true;
        }
        if (m_mustRotateDoor)
            door.transform.Rotate(0, 90, 0);
    }

    /*   public void ScreenShot()
       {
           Application.CaptureScreenshot(options.nameLevelToLoad + ".png");
           Debug.Log(options.nameLevelToLoad + ".png");
       } */
}