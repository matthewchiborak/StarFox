using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeammateARControlMode
{
    circlingAroundPoint,
    retired
}

public class TeammateARControlScript : MonoBehaviour {

    public UIController _UIController;

    public TeammateARControlMode currentMode;
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

    private bool beingTailed;
    private GameObject enemyTailing;
    public TextAsset enemyOnTailDialog;
    public TextAsset thankYouDialog;

    //Changing height variables
    //private float timeHeightChangeStarted;
    //private float timeToChangeHeight;
    private float heightChangeAngle;
    private float heightChangeAngleIncrement;
    private bool changingHeight;
    private int yDirection;
    private float heightChangeAngleMax;
    

    // Use this for initialization
    void Start ()
    {
        enemyTailing = null;
        heightChangeAngle = 0;
        heightChangeAngleIncrement = 100f;
        heightChangeAngleMax = 45;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    switch(currentMode)
        {
            case TeammateARControlMode.circlingAroundPoint:
                circlingAroundPoint();
                break;
            case TeammateARControlMode.retired:
                retired();
                break;
        }

        //Check if enemy still on tail
        if(beingTailed)
        {
            if(enemyTailing == null)
            {
                beingTailed = false;
                _UIController.loadDialog(thankYouDialog);
            }
        }
	}

    public bool tryToTail(GameObject tailer)
    {
        if(!beingTailed)
        {
            beingTailed = true;
            enemyTailing = tailer;
            _UIController.loadDialog(enemyOnTailDialog);
            return true;
        }

        return false;
    }

    public float getSpeed()
    {
        return forwardSpeed;
    }

    public TeammateARControlMode getCurrentMode()
    {
        return currentMode;
    }

    public void changeCircleCenter(Vector3 centerPoint)
    {
        pointToCircleAround = centerPoint;

        if(currentMode == TeammateARControlMode.circlingAroundPoint)
        {
            switchModes(TeammateARControlMode.circlingAroundPoint);
        }
    }

    public bool checkIsChangingHeight()
    {
        return changingHeight;
    }

    public void switchModes(TeammateARControlMode newMode)
    {
        switch (newMode)
        {
            case TeammateARControlMode.circlingAroundPoint:
                //First needs to get to the radius then need to turn to follow the radius
                //startPos = transform.position;
                reachedRadius = false;
                transitionTime = 1;
                startRot = transform.rotation;
                if(Mathf.Sqrt((transform.position.x - pointToCircleAround.x) * (transform.position.x - pointToCircleAround.x) + (transform.position.z - pointToCircleAround.z) * (transform.position.z - pointToCircleAround.z)) < circleRadius)
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
            case TeammateARControlMode.retired:
                startRot = transform.rotation;
                endRot = Quaternion.Euler(0, (Mathf.Atan2((transform.position.x - pointToCircleAround.x), (transform.position.z - pointToCircleAround.z)) * (180f / 3.1415f)), 0);
                transitionTime = 1;
                timeTransitionBegan = Time.time;
                break;
        }

        timeTransitionBegan = Time.time;
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

            if(reachedRadius)
            {
                if (clockWise)
                {
                    endRot = Quaternion.Euler(0, 90 + (Mathf.Atan2((transform.position.x - pointToCircleAround.x), (transform.position.z - pointToCircleAround.z)) * (180f / 3.1415f)), 0);
                }
                else
                {
                    endRot = Quaternion.Euler(0, -90 + (Mathf.Atan2((transform.position.x - pointToCircleAround.x), (transform.position.z - pointToCircleAround.z)) * (180f / 3.1415f)), 0);
                }

                if((Time.time - timeTransitionBegan) > transitionTime)
                {
                    inModeTransition = false;
                }
            }
        }

        if(!inModeTransition)
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
        
        
        if(!changingHeight)
        {
            //Change if height change is needed
            if (pointToCircleAround.y > transform.position.y)
            {
                yDirection = -1;
                changingHeight = true;
            }
            else if (pointToCircleAround.y < transform.position.y)
            {
                yDirection = 1;
                changingHeight = true;
            }
        }
        else
        {
            //Advance the heightChange
            transform.Rotate(heightChangeAngle, 0, 0);
            heightChangeAngle += yDirection * heightChangeAngleIncrement * Time.deltaTime;
            if(Mathf.Abs(heightChangeAngle) > 45)
            {
                heightChangeAngle = yDirection * heightChangeAngleMax;
            }

            if(yDirection < 0 && pointToCircleAround.y < transform.position.y)
            {
                changingHeight = false;
                transform.position = new Vector3(transform.position.x, pointToCircleAround.y, transform.position.z);
                heightChangeAngle = 0;
            }
            else if (yDirection > 0 && pointToCircleAround.y > transform.position.y)
            {
                changingHeight = false;
                transform.position = new Vector3(transform.position.x, pointToCircleAround.y, transform.position.z);
                heightChangeAngle = 0;
            }
        }
        
        //Set the correct velocity
        GetComponent<Rigidbody>().velocity = transform.forward * forwardSpeed;// * Time.deltaTime;
    }

    private void retired()
    {
        if (!modeInited)
        {
            transform.rotation = Quaternion.Euler(0, (Mathf.Atan2((transform.position.x - pointToCircleAround.x), (transform.position.z - pointToCircleAround.z)) * (180f / 3.1415f)), 0);
            GetComponent<Rigidbody>().velocity = transform.forward * forwardSpeed;// * Time.deltaTime;
            modeInited = true;
        }

        if(inModeTransition)
        {
            transform.rotation = Quaternion.Lerp(startRot, endRot, (Time.time - timeTransitionBegan) / transitionTime);
            if((Time.time - timeTransitionBegan) > transitionTime)
            {
                inModeTransition = false;
            }
        }

        GetComponent<Rigidbody>().velocity = transform.forward * forwardSpeed;// * Time.deltaTime;
    }
}
