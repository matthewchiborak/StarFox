using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionBriefingBlinking : MonoBehaviour {

    public Material eyesOpen;
    public Material eyesClosed;
    public Renderer target;

    private bool eyesAreClosed;

    public float minTimeBetweenBlinks;
    public float maxTimeBetweenBlinks;
    private float timeBetweenBlinks;
    private float timeOfLastBlink;

    public float timesEyesRemainClosed;
    private float timeEyesLastClosed;

	// Use this for initialization
	void Start ()
    {
        timeBetweenBlinks = Random.Range(minTimeBetweenBlinks, maxTimeBetweenBlinks);
        timeOfLastBlink = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(eyesAreClosed)
        {
            //Wait until open the eyes
            if ((Time.time - timeEyesLastClosed) > timesEyesRemainClosed)
            {
                timeBetweenBlinks = Random.Range(minTimeBetweenBlinks, maxTimeBetweenBlinks);
                timeOfLastBlink = Time.time;
                target.material = eyesOpen;
                eyesAreClosed = false;
            }
        }
        else
        {
            //Wait until need to blink
            if((Time.time - timeOfLastBlink) > timeBetweenBlinks)
            {
                timeEyesLastClosed = Time.time;
                target.material = eyesClosed;
                eyesAreClosed = true;
            }
        }
    }
}
