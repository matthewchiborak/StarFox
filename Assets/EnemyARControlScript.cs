using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyARControlMode
{
    circlingAroundPoint,
    tailOtherObject,
    tailPlayer,
    doNothingForTimeThenReturnToTailing,
    doNothingAfterPlayerLosesThenCircle,
    retire
}

public class EnemyARControlScript : MonoBehaviour {

    public GameManagerScript _gameManager;
    public GameObject enemyShot;
    public GameObject player;
    public float distanceToPlayerToShootAtIt;
    public float timeBetweenShots;
    private float timeOfLastShot;
    public float damage;
    public float shotSpeed;

    public EnemyARControlMode currentMode;
    private EnemyARControlMode previousMode;
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
    public float distanceTooCloseToPlayerToTail;
    private bool closeEnoughToLatchOnToTail;

    private float shotLead;
    private Vector3 shotDirectionNotNorm;
    private Vector3 shotDirection;
    private float timeToReachPlayer;
    private float timeToReachPlayer2;
    private float timeToReachPlayer3;
    private float distanceToLeadShot;

    //Changing height variables
    private float heightChangeAngle;
    private float heightChangeAngleIncrement;
    private bool changingHeight;
    private int yDirection;
    private float heightChangeAngleMax;
    //
    private bool heightIsChangingWhileTailing;
    private Quaternion tailHeightChangeRotStart;
    private float tailHeightChangeRotBegin;

    public float doNothingDelay;
    private bool delayAlreadyElapsed;

    //Tailing player
    private float tilt;
    private float zTiltTurnFactor;

    private float minRotX;
    private float minRotY;
    private float maxRotX;
    private float maxRotY;
    private float maxRotZ;
    private float minRotZ;

    public float doNothingAfterPlayerTailDelay;

    void Start()
    {
        previousMode = currentMode;
        timeOfLastShot = Time.time;
        heightChangeAngle = 0;
        heightChangeAngleIncrement = 100f;
        heightChangeAngleMax = 45;
        delayAlreadyElapsed = false;

        zTiltTurnFactor = 0.5f;
        minRotX = -45;
        minRotY = -45;
        maxRotX = 45;
        maxRotY = 45;
        maxRotZ = 90;
        minRotZ = -90;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentMode)
        {
            case EnemyARControlMode.circlingAroundPoint:
                circlingAroundPoint();
                takeShotAtPlayerIfCloseEnough();
                break;
            case EnemyARControlMode.tailOtherObject:
                tailOtherObject();
                break;
            case EnemyARControlMode.tailPlayer:
                tailPlayer();
                takeShotAtPlayerIfCloseEnough();
                break;
            case EnemyARControlMode.doNothingForTimeThenReturnToTailing:
                doNothingForTimeThenReturnToTailing();
                break;
            case EnemyARControlMode.doNothingAfterPlayerLosesThenCircle:
                doNothingAfterPlayerLosesThenCircle();
                break;
            case EnemyARControlMode.retire:
                retired();
                break;
        }

        //Check if level is over
        if(currentMode != EnemyARControlMode.retire)
        {
            if(_gameManager.checkIfBossIsDestroyed())
            {
                switchModes(EnemyARControlMode.retire);
            }
        }
    }

    public void setObjectToTail(Transform objectToTail)
    {
        this.objectToTail = objectToTail;
    }

    private void takeShotAtPlayerIfCloseEnough()
    {
        if((Time.time - timeOfLastShot) > timeBetweenShots)
        {
            if((player.transform.position - transform.position).magnitude < distanceToPlayerToShootAtIt)
            {
                //Shoot at the player
                //Estimate the time to reach player
                Vector3 normDirection = player.GetComponent<PlayerControllerScript>().getForward().normalized;
                timeToReachPlayer = -1 * ((((shotSpeed * (player.transform.position.z + distanceToLeadShot) - player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.z * transform.position.z) / (shotSpeed - player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.z)) - player.transform.position.z + distanceToLeadShot) / player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.z);
                timeToReachPlayer2 = -1 * ((((shotSpeed * (player.transform.position.x + distanceToLeadShot) - player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.x * transform.position.x) / (shotSpeed - player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.x)) - player.transform.position.x + distanceToLeadShot) / player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.x);
                timeToReachPlayer3 = -1 * ((((shotSpeed * (player.transform.position.y + distanceToLeadShot) - player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.y * transform.position.y) / (shotSpeed - player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.y)) - player.transform.position.y + distanceToLeadShot) / player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.y);

                timeToReachPlayer = (timeToReachPlayer + timeToReachPlayer2 + timeToReachPlayer3) / 3;

                shotDirection = new Vector3(
                    (transform.position.x - player.transform.position.x + (timeToReachPlayer * player.GetComponent<PlayerControllerScript>().getForward().x * player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity())),
                    (transform.position.y - player.transform.position.y + (timeToReachPlayer * player.GetComponent<PlayerControllerScript>().getForward().y * player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity())),
                    (transform.position.z - player.transform.position.z + (timeToReachPlayer * player.GetComponent<PlayerControllerScript>().getForward().z * player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity()))
                );

                shotDirection = shotDirection.normalized;

                shotDirection = new Vector3(
                    shotDirection.x * -shotSpeed,//(shotDirectionNotNorm.x / timeToReachPlayer),//-shotSpeed,
                    shotDirection.y * -shotSpeed, //(shotDirectionNotNorm.y / timeToReachPlayer),//-shotSpeed,
                    shotDirection.z * -shotSpeed //(shotDirectionNotNorm.z / timeToReachPlayer)//-shotSpeed
                    );

                Quaternion newAngle = Quaternion.Euler(Mathf.Atan2(player.transform.position.x - transform.position.x, player.transform.position.z + shotLead - transform.position.z), Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.z + shotLead - transform.position.z), 0);
                GameObject newShot = Instantiate(enemyShot, transform.position, newAngle);

                Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z).normalized;
                newShot.GetComponent<Rigidbody>().velocity = direction * shotSpeed;

                timeOfLastShot = Time.time;
            }
        }
    }

    public void switchModes(EnemyARControlMode newMode)
    {
        previousMode = currentMode;
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

            case EnemyARControlMode.tailPlayer:
                transitionTime = 1;
                startPos = transform.position;
                startRot = transform.rotation;
                break;

            case EnemyARControlMode.tailOtherObject:
                if (objectToTail.GetComponentInParent<TeammateARControlScript>().tryToTail(gameObject))
                {
                    transitionTime = 1;
                    startPos = transform.position;
                    startRot = transform.rotation;
                }
                else
                {
                    switchModes(previousMode);
                }
                break;   
        }
    }

    private void circlingAroundPoint()
    {
        if (!modeInited)
        {
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

        if (!changingHeight)
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
            if (Mathf.Abs(heightChangeAngle) > 45)
            {
                heightChangeAngle = yDirection * heightChangeAngleMax;
            }

            if (yDirection < 0 && pointToCircleAround.y < transform.position.y)
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

    private void tailOtherObject()
    {
        if (objectToTail == null)
        {
            switchModes(EnemyARControlMode.circlingAroundPoint);
        }
        if (objectToTail.GetComponent<TeammateARControlScript>())
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

        if ((Time.time - timeOfLastShot) > timeBetweenShots)
        {
            //Estimate the time to reach player
            Vector3 normDirection = objectToTail.forward.normalized;
            timeToReachPlayer = -1 * ((((shotSpeed * (objectToTail.position.z + distanceToLeadShot) - objectToTail.GetComponentInParent<TeammateARControlScript>().forwardSpeed * normDirection.z * transform.position.z) / (shotSpeed - objectToTail.GetComponentInParent<TeammateARControlScript>().forwardSpeed * normDirection.z)) - objectToTail.position.z + distanceToLeadShot) / objectToTail.GetComponentInParent<TeammateARControlScript>().forwardSpeed * normDirection.z);
            timeToReachPlayer2 = -1 * ((((shotSpeed * (objectToTail.position.x + distanceToLeadShot) - objectToTail.GetComponentInParent<TeammateARControlScript>().forwardSpeed * normDirection.x * transform.position.x) / (shotSpeed - objectToTail.GetComponentInParent<TeammateARControlScript>().forwardSpeed * normDirection.x)) - objectToTail.position.x + distanceToLeadShot) / objectToTail.GetComponentInParent<TeammateARControlScript>().forwardSpeed * normDirection.x);
            timeToReachPlayer3 = -1 * ((((shotSpeed * (objectToTail.position.y + distanceToLeadShot) - objectToTail.GetComponentInParent<TeammateARControlScript>().forwardSpeed * normDirection.y * transform.position.y) / (shotSpeed - objectToTail.GetComponentInParent<TeammateARControlScript>().forwardSpeed * normDirection.y)) - objectToTail.position.y + distanceToLeadShot) / objectToTail.GetComponentInParent<TeammateARControlScript>().forwardSpeed * normDirection.y);

            timeToReachPlayer = (timeToReachPlayer + timeToReachPlayer2 + timeToReachPlayer3) / 2;// 3;

            shotDirection = new Vector3(
                (transform.position.x - objectToTail.position.x + (timeToReachPlayer * objectToTail.forward.x * objectToTail.GetComponentInParent<TeammateARControlScript>().forwardSpeed)),
                (transform.position.y - objectToTail.position.y + (timeToReachPlayer * objectToTail.forward.y * objectToTail.GetComponentInParent<TeammateARControlScript>().forwardSpeed)),
                (transform.position.z - objectToTail.position.z + (timeToReachPlayer * objectToTail.forward.z * objectToTail.GetComponentInParent<TeammateARControlScript>().forwardSpeed))
            );

            shotDirection = shotDirection.normalized;

            shotDirection = new Vector3(
                shotDirection.x * -shotSpeed,//(shotDirectionNotNorm.x / timeToReachPlayer),//-shotSpeed,
                shotDirection.y * -shotSpeed, //(shotDirectionNotNorm.y / timeToReachPlayer),//-shotSpeed,
                shotDirection.z * -shotSpeed //(shotDirectionNotNorm.z / timeToReachPlayer)//-shotSpeed
                );

            Quaternion newAngle = Quaternion.Euler(Mathf.Atan2(objectToTail.position.x - transform.position.x, objectToTail.position.z + shotLead - transform.position.z), Mathf.Atan2(objectToTail.position.y - transform.position.y, objectToTail.position.z + shotLead - transform.position.z), 0);
            GameObject newShot = Instantiate(enemyShot, transform.position, newAngle);

            Vector3 direction = new Vector3(objectToTail.position.x - transform.position.x, objectToTail.position.y - transform.position.y, objectToTail.position.z - transform.position.z).normalized;
            newShot.GetComponent<Rigidbody>().velocity = direction * shotSpeed;

            timeOfLastShot = Time.time;
        }

        if (inModeTransition)
        {
            Vector3 direction = new Vector3(objectToTail.transform.position.x - transform.position.x - objectToTail.transform.forward.x * distanceAwayFromPlayerToTail, objectToTail.transform.position.y - transform.position.y - objectToTail.transform.forward.y * distanceAwayFromPlayerToTail, objectToTail.transform.position.z - transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail).normalized;

            if (Mathf.Sqrt((objectToTail.position.x - transform.position.x - objectToTail.transform.forward.x * distanceAwayFromPlayerToTail) * (objectToTail.position.x - transform.position.x - objectToTail.transform.forward.x * distanceAwayFromPlayerToTail) + (objectToTail.position.z - transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail) * (objectToTail.position.z - transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail)) < 1)//distanceAwayFromPlayerToTail)
            {
                inModeTransition = false;
            }
            else
            {
                GetComponent<Rigidbody>().velocity = direction * forwardSpeed;
                if ((Time.time - timeTransitionBegan) > transitionTime)
                    transform.LookAt(new Vector3(objectToTail.transform.position.x - objectToTail.transform.forward.x * distanceAwayFromPlayerToTail, objectToTail.transform.position.y - objectToTail.transform.forward.y * distanceAwayFromPlayerToTail, objectToTail.transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail));
            }

            if ((Time.time - timeTransitionBegan) < transitionTime)
            {
                int yDirection = 1;
                if (objectToTail.position.y > transform.position.y)
                {
                    yDirection = -1;
                }
                float tempAngle = yDirection * Mathf.Asin((objectToTail.position.y - transform.position.y - objectToTail.transform.forward.y * distanceAwayFromPlayerToTail) / ((objectToTail.position - transform.position).magnitude)) * (180f / 3.1415f);
                if ((objectToTail.position.y > transform.position.y && tempAngle > 0) || (objectToTail.position.y < transform.position.y && tempAngle < 0))
                {
                    tempAngle *= -1;
                }
                transform.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(tempAngle, (Mathf.Atan2((objectToTail.position.x - transform.position.x - objectToTail.transform.forward.x * distanceAwayFromPlayerToTail), (objectToTail.position.z - transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail)) * (180f / 3.1415f)), 0), (Time.time - timeTransitionBegan) / transitionTime);
            }
        }

        if (!inModeTransition)
        {
            Vector3 direction = new Vector3(objectToTail.transform.position.x - transform.position.x, objectToTail.transform.position.y - transform.position.y, objectToTail.transform.position.z - transform.position.z).normalized;

            //if(distanceTooCloseToPlayerToTail)
            if (Mathf.Sqrt((objectToTail.position.x - transform.position.x) * (objectToTail.position.x - transform.position.x) + (objectToTail.position.z - transform.position.z) * (objectToTail.position.z - transform.position.z)) < distanceTooCloseToPlayerToTail)//distanceAwayFromPlayerToTail)
            {
                GetComponent<Rigidbody>().velocity = direction * (objectToTail.GetComponent<TeammateARControlScript>().getSpeed() / 2);
            }
            else
            {
                GetComponent<Rigidbody>().velocity = direction * (objectToTail.GetComponent<TeammateARControlScript>().getSpeed());
            }

            transform.LookAt(new Vector3(objectToTail.transform.position.x, objectToTail.transform.position.y, objectToTail.transform.position.z));
        }
    }

    private void tailPlayer()
    {
        if (!modeInited)
        {
            modeInited = true;
        }

        if (inModeTransition)
        {
            Vector3 direction = new Vector3(player.transform.position.x - transform.position.x - objectToTail.transform.forward.x * distanceAwayFromPlayerToTail, player.transform.position.y - transform.position.y - objectToTail.transform.forward.y * distanceAwayFromPlayerToTail, player.transform.position.z - transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail).normalized;

            if (Mathf.Sqrt((objectToTail.position.x - transform.position.x - objectToTail.transform.forward.x * distanceAwayFromPlayerToTail) * (objectToTail.position.x - transform.position.x - objectToTail.transform.forward.x * distanceAwayFromPlayerToTail) + (objectToTail.position.z - transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail) * (objectToTail.position.z - transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail)) < 1)//distanceAwayFromPlayerToTail)
            {
                inModeTransition = false;
            }
            else
            {
                GetComponent<Rigidbody>().velocity = direction * forwardSpeed;
                if((Time.time - timeTransitionBegan) > transitionTime)
                    transform.LookAt(new Vector3(player.transform.position.x - player.transform.forward.x * distanceAwayFromPlayerToTail, player.transform.position.y - objectToTail.transform.forward.y * distanceAwayFromPlayerToTail, player.transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail));
            }

            if ((Time.time - timeTransitionBegan) < transitionTime)
            {
                int yDirection = 1;
                if (objectToTail.position.y > transform.position.y)
                {
                    yDirection = -1;
                }
                float tempAngle = yDirection * Mathf.Asin((objectToTail.position.y - transform.position.y - objectToTail.transform.forward.y * distanceAwayFromPlayerToTail) / ((objectToTail.position - transform.position).magnitude)) * (180f / 3.1415f);
                if ((objectToTail.position.y > transform.position.y && tempAngle > 0) || (objectToTail.position.y < transform.position.y && tempAngle < 0))
                {
                    tempAngle *= -1;
                }
                transform.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(tempAngle, (Mathf.Atan2((objectToTail.position.x - transform.position.x - objectToTail.transform.forward.x * distanceAwayFromPlayerToTail), (objectToTail.position.z - transform.position.z - objectToTail.transform.forward.z * distanceAwayFromPlayerToTail)) * (180f / 3.1415f)), 0), (Time.time - timeTransitionBegan) / transitionTime);
            }
            
            //////////////////
            if (player.GetComponent<PlayerControllerScript>().getIsSomerSaulting() || player.GetComponent<PlayerControllerScript>().getIsUturning())
            {
                switchModes(EnemyARControlMode.doNothingAfterPlayerLosesThenCircle);
            }
        }

        if (!inModeTransition)
        {
            Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z).normalized;

            //if(distanceTooCloseToPlayerToTail)
            if (Mathf.Sqrt((objectToTail.position.x - transform.position.x) * (objectToTail.position.x - transform.position.x) + (objectToTail.position.z - transform.position.z) * (objectToTail.position.z - transform.position.z)) < distanceTooCloseToPlayerToTail)//distanceAwayFromPlayerToTail)
            {
                GetComponent<Rigidbody>().velocity = direction * (player.GetComponent<PlayerControllerScript>().getDefaultForwardSpeed() / 2);
            }
            else
            {
                GetComponent<Rigidbody>().velocity = direction * (player.GetComponent<PlayerControllerScript>().getDefaultForwardSpeed());
            }

            transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z));

            if (player.GetComponent<PlayerControllerScript>().getIsSomerSaulting() || player.GetComponent<PlayerControllerScript>().getIsUturning())
            {
                switchModes(EnemyARControlMode.doNothingAfterPlayerLosesThenCircle);
            }
        }
    }
    

    private void retired()
    {
        if (!modeInited)
        {
            transform.rotation = Quaternion.Euler(0, (Mathf.Atan2((transform.position.x - pointToCircleAround.x), (transform.position.z - pointToCircleAround.z)) * (180f / 3.1415f)), 0);
            GetComponent<Rigidbody>().velocity = transform.forward * forwardSpeed;// * Time.deltaTime;
            modeInited = true;
        }

        if (inModeTransition)
        {
            transform.rotation = Quaternion.Lerp(startRot, endRot, (Time.time - timeTransitionBegan) / transitionTime);
            if ((Time.time - timeTransitionBegan) > transitionTime)
            {
                inModeTransition = false;
            }
        }

        GetComponent<Rigidbody>().velocity = transform.forward * forwardSpeed;
    }

    public void changeCircleCenter(Vector3 centerPoint)
    {
        pointToCircleAround = centerPoint;

        if (currentMode == EnemyARControlMode.circlingAroundPoint)
        {
            switchModes(EnemyARControlMode.circlingAroundPoint);
        }
    }

    private void doNothingAfterPlayerLosesThenCircle()
    {
        if(!inModeTransition)
        {
            GetComponent<Rigidbody>().velocity = transform.forward * (forwardSpeed * 5);
            inModeTransition = false;
        }

        if (inModeTransition)
        {
            if ((Time.time - timeTransitionBegan) > doNothingAfterPlayerTailDelay)
            {
                switchModes(EnemyARControlMode.circlingAroundPoint);
            }
        }
    }

    private void returnToTailingPrevious()
    {
        currentMode = EnemyARControlMode.tailOtherObject;
        timeTransitionBegan = Time.time;
        inModeTransition = true;

        transitionTime = 1;
        startPos = transform.position;
        startRot = transform.rotation;
    }
    private void doNothingForTimeThenReturnToTailing()
    {
        if(!modeInited)
        {
            GetComponent<Rigidbody>().velocity = transform.forward * (forwardSpeed / 2);
            modeInited = true;
        }

        if (inModeTransition)
        {
            GetComponent<Rigidbody>().velocity = transform.forward * (forwardSpeed / 2);
            inModeTransition = false;
        }

        if(!inModeTransition)
        {
            if((Time.time - timeTransitionBegan) > doNothingDelay)
            {
                returnToTailingPrevious();
            }
        }
    }
}
