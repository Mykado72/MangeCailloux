using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxManager : MonoBehaviour {

    
    [SerializeField] private Material[] skybox;
    
    /// <summary>
    /// Remplace la skybox au runtime.
    /// </summary>
    /// <param name="index">Index de la skybox dans le tableau.</param>
	public void SetSkybox (int index) {

        if (index > skybox.Length || index < 0) {
            return;
        } else {
            RenderSettings.skybox = skybox[index];
        }

	}


}
