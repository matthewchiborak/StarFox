using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelEndTriggerType
{
    bossDestroyed,
    timeLimit
}

public class TriggerLevelEnd : MonoBehaviour {

    public UIController _UIController;
    public GameManagerScript _gameManager;

    private bool endTriggered;
    public LevelEndTriggerType _triggerType;
    public BossControlScript boss;

    public float timeForLevelToEnd;
    private float timeLevelStarted;

	// Use this for initialization
	void Start ()
    {
        timeLevelStarted = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(!endTriggered)
        {
            switch(_triggerType)
            {
                case LevelEndTriggerType.bossDestroyed:
                    if(boss.getIsVulnerable() && boss.getCurrentHealthPercentage() <= 0)
                    {
                        _UIController.increaseHits(boss.hits);
                        _gameManager.endLevel();
                        endTriggered = true;
                    }
                    break;
                case LevelEndTriggerType.timeLimit:
                    if((Time.time - timeLevelStarted) > timeForLevelToEnd)
                    {
                        _gameManager.endLevel();
                        endTriggered = true;
                    }
                    break;
            }
        }
	}
}
