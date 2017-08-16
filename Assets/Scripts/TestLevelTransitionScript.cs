using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLevelTransitionScript : MonoBehaviour {

    public string levelToTestLoading;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject.FindWithTag("StoredInfo").GetComponent<InfoToTakeInOutOfLevel>().reset();
            SceneManager.LoadScene(levelToTestLoading, LoadSceneMode.Single);
        }
    }
}
