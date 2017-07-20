using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeammateControlScript : MonoBehaviour {

    //These two must be the same size
    public Vector3[] points;
    public float[] timeToGetToNextPoint;

    private Transform transform;
    private float timeStartedCurrentPoint;
    private int currentPoint;
    private bool isActive;

    private float tilt;
    private float yTilt;
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
        //transform = GetComponent<Transform>();
        //currentPoint = 0;
        //isActive = true;

        //if (points.Length > timeToGetToNextPoint.Length)
        //{
        //    Debug.Log("Each point does not have a time required to get to");
        //}

        //zTiltTurnFactor = 0.5f;
        //minRotX = -45;
        //minRotY = -45;
        //maxRotX = 45;
        //maxRotY = 45;
        //maxRotZ = 90;
        //minRotZ = -90;

        //GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        //startRotation = Quaternion.Euler(0, 0, 0);
        //targetRotation = Quaternion.Euler(0, 0, 0);
    }

    void init()
    {
        transform = GetComponent<Transform>();
        currentPoint = 0;
        
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

        timeStartedCurrentPoint = Time.time;

        isActive = true;
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

                    if (currentPoint < points.Length)
                    {
                        direction = (points[currentPoint] - points[currentPoint - 1]);

                        GetComponent<Rigidbody>().velocity = direction / timeToGetToNextPoint[currentPoint];

                        if(GetComponent<Rigidbody>().velocity.x != 0)
                        {
                            tilt = maxRotX / GetComponent<Rigidbody>().velocity.x;
                        }
                        else
                        {
                            tilt = 0;
                        }
                        if (GetComponent<Rigidbody>().velocity.y != 0)
                        {
                            yTilt = maxRotY / GetComponent<Rigidbody>().velocity.y;
                        }
                        else
                        {
                            yTilt = 0;
                        }
                        
                        startRotation = targetRotation;
                        Vector3 initTargetRot = new Vector3
                        //targetRotation = Quaternion.Euler
                       (
                           Mathf.Clamp(GetComponent<Rigidbody>().velocity.y * yTilt, minRotX, maxRotX),
                           Mathf.Clamp(GetComponent<Rigidbody>().velocity.x * -tilt, minRotY, maxRotY),
                           Mathf.Clamp(GetComponent<Rigidbody>().velocity.x * -tilt * zTiltTurnFactor, minRotZ, maxRotZ)
                        );
                        
                        //Correct the signs
                        if(direction.x < 0 && initTargetRot.y > 0)
                        {
                            initTargetRot.y *= -1;
                        }
                        if (direction.x > 0 && initTargetRot.y < 0)
                        {
                            initTargetRot.y *= -1;
                        }
                        if (direction.y > 0 && initTargetRot.x > 0)
                        {
                            initTargetRot.x *= -1;
                        }
                        if (direction.y < 0 && initTargetRot.x < 0)
                        {
                            initTargetRot.x *= -1;
                        }

                        targetRotation = Quaternion.Euler(initTargetRot);
                    }
                    else
                    {
                        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                        GetComponent<Rigidbody>().rotation = Quaternion.Euler(0, 0, 0);
                    }
                }
            }
        }
        else
        {
            init();
        }
    }

    public bool checkIfIsActive()
    {
        return isActive;
    }
}
