using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Only one of this should ever exist.

public class InfoToTakeInOutOfLevel : MonoBehaviour {

    private static InfoToTakeInOutOfLevel persistantInfo;

    private bool checkPointReached;
    private float checkPointZCord;

    private int storedHits;
    private int storedBombs;

	// Use this for initialization
	void Start ()
    { 
		
	}
	
    public void reset()
    {
        checkPointReached = false;
        checkPointZCord = 0;
        storedHits = 0;
        storedBombs = 0;
    }

    void Awake()
    {
        if (persistantInfo == null)
        {
            DontDestroyOnLoad(gameObject);
            persistantInfo = this;
        }
        else if (persistantInfo != this)
        {
            Destroy(persistantInfo);
        }
    }

    public bool getCheckPointReached()
    {
        return checkPointReached;
    }
    public float getCheckPointZcord()
    {
        return checkPointZCord;
    }
    public int getStoredHits()
    {
        return storedHits;
    }

    public void hitCheckPoint(float zCord, int hits)
    {
        checkPointReached = true;
        checkPointZCord = zCord;
        storedHits = hits;
    }
}
