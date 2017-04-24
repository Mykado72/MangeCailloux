using UnityEngine;
using System.Collections;

public class CameraBehaviors: MonoBehaviour {

    public static CameraBehaviors mainCameraInstance; // Singelton.

    public enum mode { Intro, Follow, OrbitAroundTarget, Fix, Outro };
    public mode camMode;
    //public float initialHeight=1; // A mettre directmeent sur le transform de la camera
    public float initialZtranslate; //Le décalage sur l'axe Z
    public float finalIntroHeight;
    public float finalInGameHeight;
    public float behavior;

    public float introZtranslate; //Le décalage sur l'axe Z
    public float inGameZtranslate; //Le décalage sur l'axe Z
    public float introAcceleration;
    public float introMaxSpeed;
    public float inGameSpeed;
    public float orbitSpeed;
    public float orbitRotationSpeed;
    public float introTime;

    private const float orbitDistance = 5;
    private const float orbitHeight = 1.5f;
    private float orbitTargetLookHeight;
    private float introTimer;
    [SerializeField] private Transform victoireCamTarget;
    [SerializeField] private Transform defaiteCamTarget;
    private Camera thisCamera;
    private float orthoZoomSpeed = 0.5f;
    private float perspectiveZoomSpeed = 0.5f;

    private Transform myTransform;
    private Transform target;
    private float speed = 0.0f;

    void Awake()
    {
        if (mainCameraInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            mainCameraInstance = this;            
        }
    }

    void Start () {

        thisCamera = gameObject.GetComponent<Camera>();
        myTransform = transform;
        speed = 0.0f;
        camMode = mode.Intro;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        myTransform.position = new Vector3(target.position.x, myTransform.position.y, target.position.z - initialZtranslate); //Placement au bon endroit pile avant la première frame
        
    }

    void Update () {

        if (camMode == mode.Intro && introTimer > introTime) {
            camMode = mode.Follow;
        } else {
            introTimer = introTimer + Time.deltaTime;
        }


        switch (camMode)
        {
            case mode.Intro:
                Intro();
                break;
            case mode.Follow:
                // PinchZoom();
                Follow();
                break;
            case mode.OrbitAroundTarget:
                OrbitArroundTarget();
                break;
            case mode.Fix:
                Fix();
                break;
            case mode.Outro:
                Outro();
                break;
            default:
                break;
        }
    }

    void Intro()
    {
        if (speed < introMaxSpeed) {
            speed = speed + introAcceleration;
        }
        myTransform.position = Vector3.Lerp(myTransform.position, new Vector3(target.position.x, finalIntroHeight, target.position.z - introZtranslate), Time.deltaTime * speed);
    }

    void Follow()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, new Vector3(target.position.x, finalInGameHeight, target.position.z - inGameZtranslate), Time.deltaTime * inGameSpeed);
    }

    void OrbitArroundTarget()
    {
        myTransform.LookAt(new Vector3(target.position.x, orbitTargetLookHeight, target.position.z));
        transform.RotateAround(target.position, Vector3.up, orbitRotationSpeed * Time.deltaTime);
        /*Vector3 desiredPosition = new Vector3(transform.position.x - target.position.x, orbitHeight, transform.position.z - target.position.z).normalized * orbitDistance + target.position;
        myTransform.position = Vector3.MoveTowards(myTransform.position, desiredPosition, Time.deltaTime * orbitSpeed);*/
    }    

    void Fix()
    {

    }

    void Outro()
    {

    }

    public void Win() {
        //camPosition(mainCameraInstance.victoireCam);
        target = victoireCamTarget;
        myTransform.position = target.position + Vector3.up * orbitHeight - Vector3.forward * orbitDistance;
        camMode = mode.OrbitAroundTarget;
        orbitTargetLookHeight = 0.2f;
    }

    public void Lose() {
        //camPosition(mainCameraInstance.victoireCam);        
        target = defaiteCamTarget;
        myTransform.position = target.position + Vector3.up * orbitHeight - Vector3.forward * orbitDistance;
        camMode = mode.OrbitAroundTarget;
        orbitTargetLookHeight = -0.2f; // Centrer sur le petit chibis triste :'(
    }

    /*public void camPosition(Transform position)
    {
        myTransform.position = position.position;
        myTransform.rotation = position.rotation;
    }*/

    void PinchZoom()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (thisCamera.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                thisCamera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                thisCamera.orthographicSize = Mathf.Max(thisCamera.orthographicSize, 0.1f);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                thisCamera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                thisCamera.fieldOfView = Mathf.Clamp(thisCamera.fieldOfView, 50f, 110f);
            }
        }
    }

    void PanCam()
    { 
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);
        }
    }
}