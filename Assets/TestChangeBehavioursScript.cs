using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChangeBehavioursScript : MonoBehaviour {

    public EnemyARControlScript objectToTest;
    public GameObject objectToTail;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.P))
        {
            objectToTest.setObjectToTail(objectToTail.transform);
            objectToTest.switchModes(EnemyARControlMode.tailOtherObject);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            objectToTail.GetComponent<TeammateARControlScript>().changeCircleCenter(new Vector3(0, -20, 0));
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            objectToTail.GetComponent<TeammateARControlScript>().changeCircleCenter(new Vector3(0, 50, 0));
        }
    }
}
