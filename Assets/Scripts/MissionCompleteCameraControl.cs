using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCompleteCameraControl : MonoBehaviour {

    public Transform transformToFollow;
    private Transform cameraTransform;
    private PlayerControllerScript playerScript;

    private Vector3 start;
    private Vector3 end;
    private float timeToGetToPlace;
    private float startTime;

    private float zOffset;
    private float startYPosition;
    private bool startedLoop;

    private float currentAngle;
    private float angleIncrement;

    private float yOffset;

    private bool isRotating;

    private Quaternion startRot;
    private Quaternion endRot;
    public Transform transformForCalc;

    private bool playerInAllRange;

    

    void Start()
    {
        cameraTransform = GetComponent<Transform>();
        playerScript = transformToFollow.GetComponentInParent<PlayerControllerScript>();
        zOffset = playerScript.cameraOffset;
        //cameraTransform.localPosition = new Vector3(0, 0, transformToFollow.localPosition.z + zOffset);

        yOffset = 5;
        
        startedLoop = false;
        startYPosition = 0;

        if(!playerScript.isInAllRange())
            currentAngle = 90;// 270f;

        angleIncrement = 5;//0.1f;

        start = cameraTransform.position;
        //end = new Vector3(0, transformToFollow.position.y + yOffset, transformToFollow.localPosition.z + zOffset);
        end = new Vector3(transformToFollow.position.x + Mathf.Cos(currentAngle * (3.1415f / 180f)) * zOffset, transformToFollow.position.y + yOffset, transformToFollow.position.z + Mathf.Sin(currentAngle * (3.1415f / 180f)) * zOffset);

        startRot = cameraTransform.rotation;

        transformForCalc.position = end;
        transformForCalc.LookAt(transformToFollow);
        endRot = transformForCalc.rotation;

        //All range
        playerInAllRange = playerScript.isInAllRange();
        ////

        isRotating = false;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //cameraTransform.localPosition = new Vector3(0, 0, transformToFollow.localPosition.z + zOffset);
        //cameraTransform.localPosition = transformToFollow.position + (transformToFollow.forward.normalized * zOffset);
        //cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, transformToFollow.position.y, cameraTransform.localPosition.z);

        if (!playerInAllRange)
        {
            if (isRotating)
            {
                //Slowly rotate around the player
                cameraTransform.position = new Vector3(transformToFollow.position.x + Mathf.Cos(currentAngle * (3.1415f / 180f)) * zOffset, transformToFollow.position.y + yOffset, transformToFollow.position.z + Mathf.Sin(currentAngle * (3.1415f / 180f)) * zOffset);

                cameraTransform.LookAt(transformToFollow);

                currentAngle += angleIncrement * Time.deltaTime;
            }
            else
            {
                cameraTransform.position = Vector3.Lerp(start, end, (Time.time - startTime) / timeToGetToPlace);
                cameraTransform.rotation = Quaternion.Lerp(startRot, endRot, (Time.time - startTime) / timeToGetToPlace);
                if ((Time.time - startTime) > timeToGetToPlace)
                {
                    isRotating = true;
                }
            }
        }
        else
        {
            //All range
            if (isRotating)
            {
                //Slowly rotate around the player
                cameraTransform.position = new Vector3(transformToFollow.position.x + Mathf.Sin(currentAngle * (3.1415f / 180f)) * zOffset, transformToFollow.position.y, transformToFollow.position.z + Mathf.Cos(currentAngle * (3.1415f / 180f)) * zOffset);

                cameraTransform.LookAt(transformToFollow);

                currentAngle -= angleIncrement * Time.deltaTime;
            }
            else
            {
                cameraTransform.localPosition = transformToFollow.position + (transformToFollow.forward.normalized * zOffset);
                cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, transformToFollow.position.y, cameraTransform.localPosition.z);
                cameraTransform.LookAt(transformToFollow);

                if ((Time.time - startTime) > timeToGetToPlace)
                {
                    isRotating = true;
                }
            }
        }
    }
    
    public void setTime(float t)
    {
        playerInAllRange = transformToFollow.GetComponentInParent<PlayerControllerScript>().isInAllRange();
        if(playerInAllRange)
        {
            currentAngle = Quaternion.Angle(transform.rotation, Quaternion.identity);
        }

        startTime = Time.time;
        timeToGetToPlace = t;
    }

    public void setCustomCameraPoint(Vector3 updatedPlayersPosition, float startYPosition)
    {
        start = new Vector3(transform.position.x, startYPosition, transform.position.z);
        end = new Vector3(updatedPlayersPosition.x + Mathf.Cos(currentAngle * (3.1415f / 180f)) * zOffset, updatedPlayersPosition.y + yOffset, updatedPlayersPosition.z + Mathf.Sin(currentAngle * (3.1415f / 180f)) * zOffset);
    }
}
