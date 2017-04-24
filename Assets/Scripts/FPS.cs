using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPS : MonoBehaviour {

	private float updateInterval;	
	private float accum; // FPS accumulated over the interval
	private float frames; // Frames drawn over the interval
	private float timeleft; // Left time for current interval

    private Text FPSIndicator;
	
	void Start () {
		FPSIndicator = GetComponent<Text>();
		updateInterval = 0.5f;
		accum = 0.0f;
		frames = 0f;
		timeleft = updateInterval;
	}
	
	void Update () {
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
            FPSIndicator.text = "FPS : " + (accum / frames).ToString("f2"); // display two fractional digits (f2 format)

            timeleft = updateInterval;
			accum = 0.0f;
			frames = 0f;

        }

    }
}
