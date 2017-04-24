using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlesTactiles : MonoBehaviour {
    
    public Sprite handOpen;
    public Sprite handClose;
    public Sprite shootOn;
    public Sprite shootOff;

    /// <summary>
    /// Image du point de départ du drag.
    /// </summary>
    private RectTransform touchPoint;
    private GameObject pad;
    private Image handIcon;
    private Image shootIcon;
    private int halfScreenX;
    private Vector2 startPosition;
    private int fingerID;
    /// <summary>
    /// Indique si un doigt est sur l'écran en train de diriger le personnage.
    /// </summary>
    private bool isDraging;
    private bool usePad;
    private int[] m_tabToucheDir = { 0, 0, 0, 0 };
    private int[] m_tabToucheAction = { 0, 0 };

    void Start () {
        if (Application.platform != RuntimePlatform.Android) {
            Destroy(this.gameObject); // On supprime le controle par le tactile qui ne sert à rien
        }
        halfScreenX = Screen.width / 2;
        touchPoint = transform.FindChild("TouchPoint").GetComponent<RectTransform>();
        pad = transform.FindChild("Joystick").gameObject;
        handIcon = transform.FindChild("Main").GetComponent<Image>();
        shootIcon = transform.FindChild("Pied").GetComponent<Image>();
        touchPoint.gameObject.SetActive(false);

        if (PlayerPrefs.GetInt("usePad", 1) == 1) {
            Destroy(touchPoint.gameObject);
            usePad = true;
        } else {
            Destroy(pad);          
        }
    }
	
	void Update () {

        if (!usePad) {
            /* Gestion des controles sans le pad */
            if (!isDraging) {
                if (Input.touchCount > 0) {
                    for (int i = 0; i < Input.touchCount; i++) {
                        //print(Input.touches[i].position + " id: " + Input.touches[i].fingerId);
                        if (Input.touches[i].position.x < halfScreenX && Input.touches[i].phase == TouchPhase.Began) {
                            //print("OK " + i + " id:" + Input.touches[i].fingerId);
                            fingerID = Input.touches[i].fingerId;
                            isDraging = true;
                            touchPoint.gameObject.SetActive(true);
                            touchPoint.position = Input.touches[i].position;// Pas sûr que la formule soit la bonne si l'écran n'est pas en 16:9
                            startPosition = Input.touches[i].position;
                            break;
                        }

                    }
                }
            } else {
                int touchIndex = -1;
                for (int i = 0; i < Input.touchCount; i++) {
                    if (Input.touches[i].fingerId == fingerID) {
                        touchIndex = i;
                        break;
                    }
                }

                if (touchIndex == -1) { // On ne trouve pas le doigt donc c'est qu'il a été levé.
                                        //print("Le doigt s'est levé");
                    touchPoint.gameObject.SetActive(false);
                    isDraging = false;
                    for (int i = 0; i < 4; i++) {
                        m_tabToucheDir[i] = 0;
                    }
                    m_tabToucheDir = new int[] { 0, 0, 0, 0 }; // On arrête tout mouvement
                } else {

                    Vector2 deltaPosition = Input.touches[touchIndex].position - startPosition;

                    if (deltaPosition.magnitude > 40) { // en nombre de pixels
                                                        //print("DeltaPosition : " + deltaPosition);
                        if (deltaPosition.y > Mathf.Abs(deltaPosition.x) * 1.5f) {
                            //print("UP");
                            m_tabToucheDir = new int[] { 1, 0, 0, 0 };
                        } else if (deltaPosition.y < -Mathf.Abs(deltaPosition.x) * 1.5f) {
                            //print("DOWN");
                            m_tabToucheDir = new int[] { 0, 1, 0, 0 };
                        } else if (deltaPosition.x > Mathf.Abs(deltaPosition.y) * 1.5f) {
                            //print("RIGHT");
                            m_tabToucheDir = new int[] { 0, 0, 0, 1 };
                        } else if (deltaPosition.x < -Mathf.Abs(deltaPosition.y) * 1.5f) {
                            //print("LEFT");
                            m_tabToucheDir = new int[] { 0, 0, 1, 0 };
                        } else {
                            m_tabToucheDir = new int[] { 0, 0, 0, 0 }; // On arrête tout mouvement
                        }


                    } else {
                        m_tabToucheDir = new int[] { 0, 0, 0, 0 }; // On arrête tout mouvement
                    }
                }
            }
        }


	}

    public void MainDown() {
        m_tabToucheAction = new int[]{ 1, 0 };
        handIcon.sprite = handClose;
    }

    public void MainUp() {
        m_tabToucheAction = new int[] { 0, 0 };
        handIcon.sprite = handOpen;
    }

    public void ShootDown() {
        m_tabToucheAction = new int[] { 0, 1 };
        shootIcon.sprite = shootOn;
    }

    public void ShootUp() {
        m_tabToucheAction = new int[] { 0, 0 };
        shootIcon.sprite = shootOff;
    }

    public void MarteauDown()
    {
        // m_tabToucheAction = new int[] { 1, 0 };
        // hammerIcon.sprite = hammerClose;
    }

    public void MarteauUp()
    {
        // m_tabToucheAction = new int[] { 0, 0 };
        // hammerIcon.sprite = hammerOpen;
    }

    public void RightPressed() {
        m_tabToucheDir = new int[] { 0, 0, 0, 1 };
    }
    public void LeftPressed() {
        m_tabToucheDir = new int[] { 0, 0, 1, 0 };
    }
    public void UpPressed() {
        m_tabToucheDir = new int[] { 1, 0, 0, 0 };
    }
    public void DownPressed() {
        m_tabToucheDir = new int[] { 0, 1, 0, 0 };
    }
    public void StopChibis() {
        m_tabToucheDir = new int[] { 0, 0, 0, 0 };
    }





    public int[] GetTabToucheDir() {
        return m_tabToucheDir;
    }

    public int[] GetTabToucheAction() {
        return m_tabToucheAction;
    }

}
