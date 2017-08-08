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
	}
}
