using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform transformToFollow;
    private Transform cameraTransform;
    private PlayerControllerScript playerScript;

    private float zOffset;
    private float startYPosition;
    private bool startedLoop;

    void Start()
    {
        cameraTransform = GetComponent<Transform>();
        cameraTransform.localPosition = new Vector3(0, 0, transformToFollow.localPosition.z + zOffset);
        playerScript = transformToFollow.GetComponentInParent<PlayerControllerScript>();
        zOffset = playerScript.cameraOffset;

        startedLoop = false;
        startYPosition = 0;
    }

    // Update is called once per frame
    void Update ()
    {
        //Never go backwards
        if (!playerScript.getIsSomerSaulting())
        {
            cameraTransform.localPosition = new Vector3(0, 0, transformToFollow.localPosition.z + zOffset);

            if(startedLoop)
            {
                startedLoop = false;
            }
        }
        else
        {
            if(!startedLoop)
            {
                startedLoop = true;
                startYPosition = transformToFollow.position.y;
            }

            cameraTransform.localPosition = new Vector3(cameraTransform.position.x, transformToFollow.position.y - startYPosition, cameraTransform.position.z);
        }
	}
}
