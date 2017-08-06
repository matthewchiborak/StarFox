using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionToARCamera : MonoBehaviour {

    public Transform player;
    public Transform transformForCalc;
    private float zOffset;

    bool hasStarted;
    //
    private Vector3 startPos;
    private Quaternion startRot;

    private Quaternion midRot;

    private Vector3 midPos;
    //

    private float transitionTime;
    private float timeTransitionStarted;

    public bool isFinished;

    private Vector3 goalVec;
    
    void init()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        timeTransitionStarted = Time.time;

        transformForCalc.position = new Vector3(player.position.x + 10, player.position.y + 10, player.position.z + 10);
        transformForCalc.LookAt(player);
        midRot = transformForCalc.rotation;

        zOffset = player.GetComponentInParent<PlayerControllerScript>().cameraOffset;

        hasStarted = true;
        isFinished = false;
    }

    public void setTransitionTime(float transTime)
    {
        transitionTime = transTime;
    }
    
	
	// Update is called once per frame
	void Update ()
    {
		if(!hasStarted)
        {
            init();
        }

        if (!isFinished)
        {
            if ((Time.time - timeTransitionStarted) < (transitionTime / 3))
            {
                //Forward pan
                goalVec = new Vector3(player.position.x + 10, player.position.y + 10, player.position.z + 10);
                transform.position = Vector3.Lerp(startPos, goalVec, (Time.time - timeTransitionStarted) / (transitionTime / 3));
                transform.rotation = Quaternion.Lerp(startRot, midRot, (Time.time - timeTransitionStarted) / (transitionTime / 3));
            }
            else if ((Time.time - timeTransitionStarted) - (transitionTime / 3f) < (transitionTime / 3f))
            {
                //Stare
                transform.position = new Vector3(player.position.x + 10, player.position.y + 10, player.position.z + 10);

                midPos = transform.position;
            }
            else if ((Time.time - timeTransitionStarted) - (2f * (transitionTime / 3f)) < (transitionTime / 3f))
            {
                //pan back
                goalVec = new Vector3(player.position.x, player.position.y, player.position.z + zOffset);
                transform.position = Vector3.Lerp(midPos, goalVec, ((Time.time - timeTransitionStarted) - (2 * transitionTime / 3)) / (transitionTime / 3));
                transform.rotation = Quaternion.Lerp(midRot, Quaternion.identity, ((Time.time - timeTransitionStarted) - (2 * transitionTime / 3)) / (transitionTime / 3));
            }
            else
            {
                isFinished = true;
            }
        }
    }
}
