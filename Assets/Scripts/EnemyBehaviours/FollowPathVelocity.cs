using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathVelocity : MonoBehaviour {

    //These two must be the same size
    public Vector3[] points;
    public float[] timeToGetToNextPoint;
    public float zCordToActiveAt;

    private Transform transform;
    private float timeStartedCurrentPoint;
    private int currentPoint;
    private bool isActive;

    private float tilt;
    private float zTiltTurnFactor;

    private float minRotX;
    private float minRotY;
    private float maxRotX;
    private float maxRotY;
    private float maxRotZ;
    private float minRotZ;
    
    private Vector3 direction;

    private Quaternion startRotation;
    private Quaternion targetRotation;

    // Use this for initialization
    void Start()
    {
        transform = GetComponent<Transform>();
        currentPoint = 0;
        isActive = false;

        if (points.Length > timeToGetToNextPoint.Length)
        {
            Debug.Log("Each point does not have a time required to get to");
        }

        zTiltTurnFactor = 0.5f;
        minRotX = -45;
        minRotY = -45;
        maxRotX = 45;
        maxRotY = 45;
        maxRotZ = 90;
        minRotZ = -90;
        
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        startRotation = Quaternion.Euler(0, 0, 0);
        targetRotation = Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (currentPoint < points.Length)
            {
                //Update the rotation
                //if(GetComponent<Rigidbody>().velocity.x == 0)
                //{
                //    tilt = 0;
                //}
                //else
                //{
                //    tilt = maxRotX / GetComponent<Rigidbody>().velocity.x;
                //}

                // Vector3 rotation = new Vector3
                //(
                //    Mathf.Clamp(GetComponent<Rigidbody>().velocity.y * moveHorizontal * -tilt, minRotX, maxRotX),
                //    Mathf.Clamp(GetComponent<Rigidbody>().velocity.x * moveVertical * tilt, minRotX, maxRotX),
                //    Mathf.Clamp(GetComponent<Rigidbody>().velocity.x * -tilt * zTiltTurnFactor, minRotZ, maxRotZ)
                // );

                if (timeToGetToNextPoint[currentPoint] != 0)
                {
                    GetComponent<Rigidbody>().rotation = Quaternion.Lerp(startRotation, targetRotation, ((Time.time - timeStartedCurrentPoint) / (timeToGetToNextPoint[currentPoint]) * 2));
                }
                //GetComponent<Rigidbody>().rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
               

                if (currentPoint == 0 || ((Time.time - timeStartedCurrentPoint) > (timeToGetToNextPoint[currentPoint])))
                {
                    currentPoint++;
                    timeStartedCurrentPoint = Time.time;
                    
                    if(currentPoint < points.Length)
                    {
                        direction = (points[currentPoint] - points[currentPoint - 1]);

                        GetComponent<Rigidbody>().velocity = direction / timeToGetToNextPoint[currentPoint];

                        if (GetComponent<Rigidbody>().velocity.x != 0)
                        {
                            tilt = maxRotX / GetComponent<Rigidbody>().velocity.x;
                        }
                        else
                        {
                            tilt = 0;
                        }

                        startRotation = targetRotation;
                        targetRotation = Quaternion.Euler
                       (
                           Mathf.Clamp(GetComponent<Rigidbody>().velocity.y * -tilt, minRotX, maxRotX),
                           Mathf.Clamp(GetComponent<Rigidbody>().velocity.x * tilt, minRotY, maxRotY),
                           Mathf.Clamp(GetComponent<Rigidbody>().velocity.x * -tilt * zTiltTurnFactor, minRotZ, maxRotZ)
                        );
                        
                    }
                    else
                    {
                        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                        GetComponent<Rigidbody>().rotation = Quaternion.Euler(0, 0, 0);
                    }
                }
            }
        }
    }

    public bool checkIfIsActive()
    {
        return isActive;
    }

    public void activateEnemy()
    {
        isActive = true;
        timeStartedCurrentPoint = Time.time;
    }
}
