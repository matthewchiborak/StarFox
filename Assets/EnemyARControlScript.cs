using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyARControlMode
{
    circlingAroundPoint
}

public class EnemyARControlScript : MonoBehaviour {

    public EnemyARControlMode currentMode;
    private bool modeInited;
    private bool inModeTransition;
    private float transitionTime;
    private float timeTransitionBegan;

    private Vector3 startPos;
    private Vector3 endPos;
    private Quaternion startRot;
    private Quaternion endRot;

    public Vector3 pointToCircleAround;
    public float startingAngleOnCircle;
    public bool clockWise;
    public float circleRadius;
    public float forwardSpeed;

    private bool movingAwayFromCenter;
    private bool reachedRadius;

    // Update is called once per frame
    void Update()
    {
        switch (currentMode)
        {
            case EnemyARControlMode.circlingAroundPoint:
                circlingAroundPoint();
                break;
        }
    }

    public void switchModes(EnemyARControlMode newMode)
    {
        switch (newMode)
        {
            case EnemyARControlMode.circlingAroundPoint:
                //First needs to get to the radius then need to turn to follow the radius
                //startPos = transform.position;
                reachedRadius = false;
                transitionTime = 1;
                startRot = transform.rotation;
                if (Mathf.Sqrt((transform.position.x - pointToCircleAround.x) * (transform.position.x - pointToCircleAround.x) + (transform.position.z - pointToCircleAround.z) * (transform.position.z - pointToCircleAround.z)) < circleRadius)
                {
                    //Mode away from the center
                    movingAwayFromCenter = true;
                    endRot = Quaternion.Euler(0, (Mathf.Atan2((transform.position.x - pointToCircleAround.x), (transform.position.z - pointToCircleAround.z)) * (180f / 3.1415f)), 0);
                }
                else
                {
                    //Move towards the center
                    movingAwayFromCenter = false;
                    endRot = Quaternion.Euler(0, 180 + (Mathf.Atan2((transform.position.x - pointToCircleAround.x), (transform.position.z - pointToCircleAround.z)) * (180f / 3.1415f)), 0);
                }
                break;
        }

        currentMode = newMode;
        inModeTransition = true;
    }

    private void circlingAroundPoint()
    {
        if (!modeInited)
        {
            //transform.position = new Vector3(0, pointToCircleAround.y, circleRadius);
            transform.position = new Vector3(circleRadius * Mathf.Sin(startingAngleOnCircle) + pointToCircleAround.x, pointToCircleAround.y, circleRadius * Mathf.Cos(startingAngleOnCircle) + pointToCircleAround.z);

            modeInited = true;
        }

        if (inModeTransition)
        {
            transform.rotation = Quaternion.Lerp(startRot, endRot, (Time.time - timeTransitionBegan) / transitionTime);
            if ((!reachedRadius && movingAwayFromCenter && Mathf.Sqrt((transform.position.x - pointToCircleAround.x) * (transform.position.x - pointToCircleAround.x) + (transform.position.z - pointToCircleAround.z) * (transform.position.z - pointToCircleAround.z)) > circleRadius)
                || (!reachedRadius && !movingAwayFromCenter && Mathf.Sqrt((transform.position.x - pointToCircleAround.x) * (transform.position.x - pointToCircleAround.x) + (transform.position.z - pointToCircleAround.z) * (transform.position.z - pointToCircleAround.z)) < circleRadius))
            {
                reachedRadius = true;
                timeTransitionBegan = Time.time;
                startRot = transform.rotation;
            }

            if (reachedRadius)
            {
                if (clockWise)
                {
                    endRot = Quaternion.Euler(0, 90 + (Mathf.Atan2((transform.position.x - pointToCircleAround.x), (transform.position.z - pointToCircleAround.z)) * (180f / 3.1415f)), 0);
                }
                else
                {
                    endRot = Quaternion.Euler(0, -90 + (Mathf.Atan2((transform.position.x - pointToCircleAround.x), (transform.position.z - pointToCircleAround.z)) * (180f / 3.1415f)), 0);
                }

                if ((Time.time - timeTransitionBegan) > transitionTime)
                {
                    inModeTransition = false;
                }
            }
        }

        if (!inModeTransition)
        {
            //Rotate so that ship is on the radius of the circle
            if (clockWise)
            {
                transform.rotation = Quaternion.Euler(0, 90 + (Mathf.Atan2((transform.position.x - pointToCircleAround.x), (transform.position.z - pointToCircleAround.z)) * (180f / 3.1415f)), 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, -90 + (Mathf.Atan2((transform.position.x - pointToCircleAround.x), (transform.position.z - pointToCircleAround.z)) * (180f / 3.1415f)), 0);
            }
        }

        //Set the correct velocity
        GetComponent<Rigidbody>().velocity = transform.forward * forwardSpeed * Time.deltaTime;
    }
}
