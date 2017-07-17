using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class FollowPoint
//{
//    public Vector3 position;
//    public float timeToNextPosition;
//}

public class FollowPath : MonoBehaviour {

    //These two must be the same size
    public Vector3[] points;
    public float[] timeToGetToNextPoint;
    public float zCordToActiveAt;

    private Transform transform;
    private float timeStartedCurrentPoint;
    private int currentPoint;
    private bool isActive;

	// Use this for initialization
	void Start ()
    {
        transform = GetComponent<Transform>();
        currentPoint = 1;
        isActive = false;	

        if(points.Length > timeToGetToNextPoint.Length)
        {
            Debug.Log("Each point does not have a time required to get to");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(isActive)
        {
            if(currentPoint < points.Length)
            {
                transform.position = Vector3.Lerp(points[currentPoint - 1], points[currentPoint], (Time.time - timeStartedCurrentPoint) / (timeToGetToNextPoint[currentPoint]));
                //Debug.Log((Time.time - timeStartedCurrentPoint) / (timeToGetToNextPoint[currentPoint]));

                if((Time.time - timeStartedCurrentPoint) > (timeToGetToNextPoint[currentPoint]))
                {
                    currentPoint++;
                    timeStartedCurrentPoint = Time.time;
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
