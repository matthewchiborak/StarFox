using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventRotate : MonoBehaviour {

	// Update is called once per frame
	void Update ()
    {
        transform.rotation = new Quaternion(0, 0, 0, 1);	
	}
}
