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
        playerScript = transformToFollow.GetComponentInParent<PlayerControllerScript>();
        zOffset = playerScript.cameraOffset;
        cameraTransform.localPosition = new Vector3(0, 0, transformToFollow.localPosition.z + zOffset);
        
        startedLoop = false;
        startYPosition = 0;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!playerScript.isInAllRange())
        {
            //Never go backwards
            if (!playerScript.getIsSomerSaulting())
            {
                cameraTransform.localPosition = new Vector3(0, 0, transformToFollow.localPosition.z + zOffset);

                if (startedLoop)
                {
                    startedLoop = false;
                }
            }
            else
            {
                if (!startedLoop)
                {
                    startedLoop = true;
                    startYPosition = transformToFollow.position.y;
                }

                cameraTransform.localPosition = new Vector3(cameraTransform.position.x, transformToFollow.position.y - startYPosition, cameraTransform.position.z);
            }
        }
        else
        {
            //Stay behind the player always because is in all range mode
            //Vector3 direction = transformToFollow.forward.normalized * zOffset;
            if(!playerScript.getIsSomerSaulting())
            {
                cameraTransform.localPosition = transformToFollow.position + (transformToFollow.forward.normalized * zOffset);
                cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, transformToFollow.position.y, cameraTransform.localPosition.z);
            }
            cameraTransform.LookAt(transformToFollow);
           // cameraTransform.localRotation = Quaternion.
            //cameraTransform.localRotation = Quaternion.Euler(0, transformToFollow.rotation.y, 0);
            //transform.rotation = Quaternion.AngleAxis(30, Vector3.up);
            //transformToFollow.rotation;
        }
    }
}
