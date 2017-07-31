using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventRotate : MonoBehaviour {

    public Transform camera;

	// Update is called once per frame
	void Update ()
    {
        //transform.rotation = new Quaternion(0, 0, 0, 1);
        //Always face the camera
        //float angle = Mathf.Atan2((camera.position.x - transform.position.x) , (camera.position.z - transform.position.z));

        /*float angleToFaceCam = atan((directToCam.z / directToCam.x));

	    if (directToCam.x > 0)
	    {
		    angleToFaceCam += 3.1415;
	    }

	    transform.SetRot(glm::fvec3(0, (angleToFaceCam + (3.1415 / 2)) * -1, 0));*/

        float angle = Mathf.Atan2((transform.position.x - camera.position.x), (transform.position.z - camera.position.z));
        
        transform.rotation = new Quaternion(0, angle, 0, 1);
    }
}
