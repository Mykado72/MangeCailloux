using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour {

    public RectTransform chibisIcon;
    private AsyncOperation async = null;
    public Text textVersion;

    void Start() 
    {
        textVersion.text = "Version : "+Application.version;
        StartCoroutine(Load());
    }

    
    IEnumerator Load ()
    {
        switch (OptionManager.optionsInstance.nameLevelToLoad)
        {
            case null:
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                    async = SceneManager.LoadSceneAsync("WebGLMainMenu");
                else
                    async = SceneManager.LoadSceneAsync("MainMenu");
                break;
            case "":
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                    async = SceneManager.LoadSceneAsync("WebGLMainMenu");
                else
                    async = SceneManager.LoadSceneAsync("MainMenu");
                break;
            default:
                async = SceneManager.LoadSceneAsync("Level");
                break;
        }
       
        while (!async.isDone) {
            // print(async.progress);
            chibisIcon.anchorMax = chibisIcon.anchorMin = new Vector2(async.progress-0.1f, 0);
            chibisIcon.anchoredPosition = Vector2.zero;
            yield return 0;
        }
        yield return new WaitForSeconds(1);
    }
}
