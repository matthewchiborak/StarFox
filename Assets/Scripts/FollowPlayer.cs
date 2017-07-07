using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform transformToFollow;
    private Transform cameraTransform;
    private PlayerControllerScript playerScript;

    private float zOffset;

    void Start()
    {
        cameraTransform = GetComponent<Transform>();
        cameraTransform.localPosition = new Vector3(0, 0, transformToFollow.localPosition.z + zOffset);
        playerScript = transformToFollow.GetComponentInParent<PlayerControllerScript>();
        zOffset = playerScript.cameraOffset;
    }

    // Update is called once per frame
    void Update ()
    {
        //Never go backwards
        if (!playerScript.getIsSomerSaulting())
        {
            cameraTransform.localPosition = new Vector3(0, 0, transformToFollow.localPosition.z + zOffset);
        }
	}
}
