using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Only one of this should ever exist.

public class InfoToTakeInOutOfLevel : MonoBehaviour {

    private static InfoToTakeInOutOfLevel persistantInfo;
    private bool cameFromLevel;
    private int levelId;

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
        cameFromLevel = false;
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

    public bool getCameFromlevel()
    {
        return cameFromLevel;
    }
    public int getLevelId()
    {
        return levelId;
    }
    public void setLevelId(int id)
    {
        levelId = id;
    }

    public void hitCheckPoint(float zCord, int hits)
    {
        checkPointReached = true;
        checkPointZCord = zCord;
        storedHits = hits;
    }

    public void finishedLevel(int hits)
    {
        storedHits = hits;
        cameFromLevel = true;
    }
}
