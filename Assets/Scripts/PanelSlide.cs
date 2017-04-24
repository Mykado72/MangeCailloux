using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSlide : MonoBehaviour {
    
    private RectTransform myRectTransform;

    private float timer;
    private Vector2 startAnchor;
    private Vector2 targetAnchor;


    public void Awake() {
        myRectTransform = GetComponent<RectTransform>();
        startAnchor = myRectTransform.anchoredPosition3D;
    }    

    public void Move (Vector2 startPosition, Vector2 endPosition) {
        if (startPosition == endPosition) {
            myRectTransform.anchoredPosition3D = startAnchor = targetAnchor = startPosition;
        } else {
            myRectTransform.anchoredPosition3D = startAnchor = startPosition;
            targetAnchor = endPosition;
            timer = 0f;
            gameObject.SetActive(true);
            enabled = true;
        }

    }

    void Update () {
        timer = Mathf.Clamp01(timer + Time.deltaTime);
        myRectTransform.anchoredPosition3D = Vector2.Lerp(startAnchor, targetAnchor, Mathf.Sin(timer* (Mathf.PI/2)));
        if (targetAnchor != Vector2.zero &&  myRectTransform.anchoredPosition == targetAnchor) {
            gameObject.SetActive(false);
        }else if (myRectTransform.anchoredPosition == targetAnchor) {
            enabled = false;
        }
    }

}
