using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyARControlMode
{
    circlingAroundPoint,
    tailOtherObject
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

    private Transform objectToTail;
    public float distanceAwayFromPlayerToTail;
    private bool closeEnoughToLatchOnToTail;

    // Update is called once per frame
    void Update()
    {
        switch (currentMode)
        {
            case EnemyARControlMode.circlingAroundPoint:
                circlingAroundPoint();
                break;
            case EnemyARControlMode.tailOtherObject:
                tailOtherObject();
                break;
        }
    }

    public void setObjectToTail(Transform objectToTail)
    {
        this.objectToTail = objectToTail;
    }

    public void switchModes(EnemyARControlMode newMode)
    {
        currentMode = newMode;
        timeTransitionBegan = Time.time;
        inModeTransition = true;

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

            case EnemyARControlMode.tailOtherObject:
                transitionTime = 1;
                startPos = transform.position;
                startRot = transform.rotation;
                break;
        }
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

    private void tailOtherObject()
    {
        if(objectToTail == null)
        {
            switchModes(EnemyARControlMode.circlingAroundPoint);
        }
        if(objectToTail.GetComponent<TeammateARControlScript>())
        {
            if (objectToTail.GetComponent<TeammateARControlScript>().getCurrentMode() == TeammateARControlMode.retired)
            {
                switchModes(EnemyARControlMode.circlingAroundPoint);
            }
        }

        if (!modeInited)
        {
            modeInited = true;
        }

        if (inModeTransition)
        {
            //float distance = (objectToTail.position - transform.position).magnitude;
            //float angle = Mathf.Asin((objectToTail.position.y - transform.position.y) / ((objectToTail.position - transform.position).magnitude));
            //if(!closeEnoughToLatchOnToTail)
            {
                int yDirection = 1;
                if (objectToTail.position.y > transform.position.y)
                {
                    yDirection = -1;
                }
                transform.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(yDirection * Mathf.Asin((objectToTail.position.y - transform.position.y - objectToTail.transform.forward.y * distanceAwayFromPlayerToTail) / ((objectToTail.position - transform.position).magnitude)) * (180f / 3.1415f), (Mathf.Atan2((objectToTail.position.x - transform.position.x - objectToTail.transform.forward.x * distanceAwayFromPlayerToTail), (objectToTail.position.z - transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail)) * (180f / 3.1415f)), 0), (Time.time - timeTransitionBegan) / transitionTime);
                GetComponent<Rigidbody>().velocity = transform.forward * forwardSpeed * Time.deltaTime;

                if (Mathf.Sqrt((objectToTail.position.x - transform.position.x - objectToTail.transform.forward.x * distanceAwayFromPlayerToTail) * (objectToTail.position.x - transform.position.x - objectToTail.transform.forward.x * distanceAwayFromPlayerToTail) + (objectToTail.position.z - transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail) * (objectToTail.position.z - transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail)) < 1)//distanceAwayFromPlayerToTail)
                {
                    GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    inModeTransition = false;
                    //startPos = transform.position;
                    //startRot = transform.rotation;
                    //timeTransitionBegan = Time.time;
                    //closeEnoughToLatchOnToTail = true;
                }
            }
            //else
            //{
            //    transform.position = Vector3.Lerp(startPos, new Vector3(objectToTail.position.x - (distanceAwayFromPlayerToTail * transform.forward.normalized.x), objectToTail.position.y - (distanceAwayFromPlayerToTail * transform.forward.normalized.y), objectToTail.position.z - (distanceAwayFromPlayerToTail * transform.forward.normalized.z)), (Time.time - timeTransitionBegan) / transitionTime);
            //    transform.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(0, (Mathf.Atan2((objectToTail.position.x - transform.position.x), (objectToTail.position.z - transform.position.z)) * (180f / 3.1415f)), 0), (Time.time - timeTransitionBegan) / transitionTime);

            //    if ((Time.time - timeTransitionBegan > transitionTime))
            //    {
            //        Debug.Log("Fini");
            //        inModeTransition = false;
            //    }
            //}
        }

        if (!inModeTransition)
        {
            transform.rotation = Quaternion.Euler(0, (Mathf.Atan2((objectToTail.position.x - transform.position.x), (objectToTail.position.z - transform.position.z)) * (180f / 3.1415f)), 0);
            transform.position = new Vector3(objectToTail.position.x - (distanceAwayFromPlayerToTail * transform.forward.normalized.x), objectToTail.position.y - (distanceAwayFromPlayerToTail * transform.forward.normalized.y), objectToTail.position.z - (distanceAwayFromPlayerToTail * transform.forward.normalized.z));
        }
    }
}
