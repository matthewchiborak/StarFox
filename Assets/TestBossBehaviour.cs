using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TestBossBehaviourMode
{
    StandBy,
    Appear,
    HealthBarGrow,
    Attack,
    Destroyed
}

public class TestBossBehaviour : MonoBehaviour {

    //Refence to the boss information
    public BossControlScript _bossControl;
    public UIController _UIControl;
    public GameManagerScript _gameManager;

    TestBossBehaviourMode currentBehaviour;

    //Appear
    //public Vector3 startPos;
    //public Vector3 endPos;
    public Vector3[] points;
    public Vector3[] rotations;
    public float[] timeToReachDestination;
    private int currentPoint;
    private float timeOfAppear;
    public float timeForHealthBarToGrow;
    private float timeHealthBarStartGrow;

    public AudioSource healthGrowSource;

	// Use this for initialization
	void Start ()
    {
        currentBehaviour = TestBossBehaviourMode.StandBy;
	}
    
	// Update is called once per frame
	void Update ()
    {
	    switch(currentBehaviour)
        {
            case TestBossBehaviourMode.StandBy:
                if(_bossControl.checkIfActive())
                {
                    currentBehaviour = TestBossBehaviourMode.Appear;
                    timeOfAppear = Time.time;
                    currentPoint = 1;
                }
                break;

            case TestBossBehaviourMode.Appear:
                transform.position = Vector3.Lerp(points[currentPoint - 1], points[currentPoint], (Time.time - timeOfAppear) / timeToReachDestination[currentPoint]);
                transform.rotation = Quaternion.Euler(Vector3.Lerp(rotations[currentPoint - 1], rotations[currentPoint], (Time.time - timeOfAppear) / timeToReachDestination[currentPoint]));

                if(Time.time - timeOfAppear > timeToReachDestination[currentPoint])
                {
                    currentPoint++;
                    timeOfAppear = Time.time;

                    if (currentPoint >= points.Length)
                    {
                        timeHealthBarStartGrow = Time.time;
                        currentBehaviour = TestBossBehaviourMode.HealthBarGrow;
                        _UIControl.updateBossHealth(0);
                        _UIControl.activateHealthBar();
                        healthGrowSource.Play();
                        //currentBehaviour = TestBossBehaviourMode.Attack;
                        //_bossControl.setIsVulnerable(true);
                    }
                }
                break;

            case TestBossBehaviourMode.HealthBarGrow:

                healthGrowSource.pitch = Mathf.Lerp(0.5f, 2.5f, (Time.time - timeHealthBarStartGrow) / timeForHealthBarToGrow);
                _UIControl.updateBossHealth((Time.time - timeHealthBarStartGrow) / timeForHealthBarToGrow);

                if (Time.time - timeHealthBarStartGrow > timeForHealthBarToGrow)
                {
                    _gameManager.allowHealthBarUpdates();
                    healthGrowSource.Stop();

                    currentBehaviour = TestBossBehaviourMode.Attack;
                    _bossControl.setIsVulnerable(true);
                }
                break;

            case TestBossBehaviourMode.Attack:
                //Activate health bar
                _UIControl.activateHealthBar();
                Debug.Log("I'm attacking");
                break;

            case TestBossBehaviourMode.Destroyed:
                break;
        }
	}
}
