using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCompleteCameraControl : MonoBehaviour {

    public Transform transformToFollow;
    private Transform cameraTransform;
    private PlayerControllerScript playerScript;

    private float zOffset;
    private float startYPosition;
    private bool startedLoop;

    private float currentAngle;
    private float angleIncrement;

    private float yOffset;

    void Start()
    {
        cameraTransform = GetComponent<Transform>();
        playerScript = transformToFollow.GetComponentInParent<PlayerControllerScript>();
        zOffset = playerScript.cameraOffset;
        cameraTransform.localPosition = new Vector3(0, 0, transformToFollow.localPosition.z + zOffset);

        startedLoop = false;
        startYPosition = 0;

        currentAngle = 90;// 270f;
        angleIncrement = 5;//0.1f;

        yOffset = 5;
    }

    // Update is called once per frame
    void Update()
    {
        //cameraTransform.localPosition = new Vector3(0, 0, transformToFollow.localPosition.z + zOffset);
        //cameraTransform.localPosition = transformToFollow.position + (transformToFollow.forward.normalized * zOffset);
        //cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, transformToFollow.position.y, cameraTransform.localPosition.z);

        //Slowly rotate around the player
        cameraTransform.position = new Vector3(transformToFollow.position.x + Mathf.Cos(currentAngle * (3.1415f / 180f)) * zOffset, transformToFollow.position.y + yOffset, transformToFollow.position.z + Mathf.Sin(currentAngle * (3.1415f / 180f)) * zOffset);

        cameraTransform.LookAt(transformToFollow);

        currentAngle += angleIncrement * Time.deltaTime;
    }
}
