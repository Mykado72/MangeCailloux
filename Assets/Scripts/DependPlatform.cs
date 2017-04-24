using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependPlatform : MonoBehaviour {

    public RuntimePlatform[] platformKO;

    // Use this for initialization
    void Awake () {
        foreach (RuntimePlatform platform in platformKO)
        {
            if (Application.platform == platform)
                this.gameObject.SetActive(false);
        }
    }
}