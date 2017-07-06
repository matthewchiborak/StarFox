using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform transformToFollow;
    private Transform cameraTransform;

    public float zOffset;

    void Start()
    {
        cameraTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update ()
    {
        cameraTransform.localPosition = new Vector3(0, 0, transformToFollow.localPosition.z + zOffset);
	}
}
