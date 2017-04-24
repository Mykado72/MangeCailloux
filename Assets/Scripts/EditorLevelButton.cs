using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EditorLevelButton : MonoBehaviour {

    /// <summary>
    /// Indique si le niveau à charger est un niveau perso.
    /// </summary>
    [SerializeField]
    private bool isCustomLevel;

    void Start () {
        if (isCustomLevel) {
            GetComponent<Button>().onClick.AddListener(() => { LvlEditor.mainLvlEditor.LoadLevelCustom(GetComponentInChildren<Text>().text); }); // Fonctionne bien même si ne s'affiche pas dans l'inspector.
        } else {
            GetComponent<Button>().onClick.AddListener(() => { LvlEditor.mainLvlEditor.LoadLevelResource(GetComponentInChildren<Text>().text); }); // Fonctionne bien même si ne s'affiche pas dans l'inspector.
        }

        GetComponent<RectTransform>().localScale = Vector3.one;
    }

}
