using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ButtonEditorBrush : MonoBehaviour {

    [SerializeField]
    private short panelID;
    [SerializeField]
    private string brush;
    [SerializeField]
    private Sprite normalSprite;
    private Button myButton;

	void Start () {
        myButton = GetComponent<Button>();
    }
	
	public void OnClick() {
        LvlEditor.mainLvlEditor.SetCurrentBrush(brush, myButton, panelID, normalSprite);
	}
}
