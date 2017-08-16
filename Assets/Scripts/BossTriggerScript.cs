using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossTriggerType
{
    zCordReached,
    timeElapsed,
    certainTargetsDestroyed,
    zCordThenTimeElapsed
}

public class BossTriggerScript : MonoBehaviour {

    private bool bossTriggered;

    public GameManagerScript _gameManager;
    public Transform player;
    public BossTriggerType _bossTriggerType;
    
    public float zCordToTriggerBoss;

    public float timeNeededToPassForBossAppearAR;

    public GameObject[] targetsToDestroy;
    private bool aTargetStillExist;
    

    // Use this for initialization
    void Start ()
    {
        _gameManager.setTimeNeededToPassForBossAppearAR(timeNeededToPassForBossAppearAR);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!bossTriggered)
        {
            switch (_bossTriggerType)
            {
                case BossTriggerType.zCordReached:
                    if(player.position.z > zCordToTriggerBoss)
                    {
                        bossTriggered = _gameManager.triggerBoss();
                    }
                    break;
                case BossTriggerType.timeElapsed:
                    if((Time.time - _gameManager.getTimeARStarted()) > timeNeededToPassForBossAppearAR)
                    {
                        bossTriggered = _gameManager.triggerBoss();
                    }
                    break;
                case BossTriggerType.certainTargetsDestroyed:
                    aTargetStillExist = false;
                    for(int i = 0; i < targetsToDestroy.Length; i++)
                    {
                        if(targetsToDestroy[i] != null)
                        {
                            aTargetStillExist = true;
                        }
                    }
                    if(!aTargetStillExist)
                    {
                        bossTriggered = _gameManager.triggerBoss();
                    }
                    break;
                case BossTriggerType.zCordThenTimeElapsed:
                    if (player.position.z > zCordToTriggerBoss)
                    {
                        if ((Time.time - _gameManager.getTimeARStarted()) > timeNeededToPassForBossAppearAR)
                        {
                            bossTriggered = _gameManager.triggerBoss();
                        }
                    }
                    break;
            }
        }
	}
}
