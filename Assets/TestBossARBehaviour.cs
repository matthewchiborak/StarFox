using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TestBossARBehaviourMode
{
    StandBy,
    Appear,
    HealthBarGrow,
    Attack,
    Destroyed
}

public class TestBossARBehaviour : MonoBehaviour {

    //Refence to the boss information
    public BossControlScript _bossControl;
    public UIController _UIControl;
    public GameManagerScript _gameManager;

    //Laser to fire
    public GameObject laser;
    public GameObject player;
    public Transform[] shotSpawn;

    //Shoots at a random interval between 2 values
    public float minTimeBetweenShot;
    public float maxTimeBetweenShots;
    private float[] amountOfTimeToPassBeforeFire;
    private float[] currentTimeBetweenShots;
    private float shotLead;
    public float shotSpeed;

    TestBossARBehaviourMode currentBehaviour;

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

    public ParticleSystem explosion;
    public Light explosionLight;
    public AudioSource explosionSource;

    // Use this for initialization
    void Start()
    {
        currentBehaviour = TestBossARBehaviourMode.StandBy;

        amountOfTimeToPassBeforeFire = new float[shotSpawn.Length];
        currentTimeBetweenShots = new float[shotSpawn.Length];

        for (int i = 0; i < amountOfTimeToPassBeforeFire.Length; i++)
        {
            amountOfTimeToPassBeforeFire[i] = Random.Range(minTimeBetweenShot, maxTimeBetweenShots);
            currentTimeBetweenShots[i] = Time.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentBehaviour)
        {
            case TestBossARBehaviourMode.StandBy:
                if (_bossControl.checkIfActive())
                {
                    currentBehaviour = TestBossARBehaviourMode.Appear;
                    timeOfAppear = Time.time;
                    currentPoint = 1;
                }
                break;

            case TestBossARBehaviourMode.Appear:
                transform.position = Vector3.Lerp(points[currentPoint - 1], points[currentPoint], (Time.time - timeOfAppear) / timeToReachDestination[currentPoint]);
                transform.rotation = Quaternion.Euler(Vector3.Lerp(rotations[currentPoint - 1], rotations[currentPoint], (Time.time - timeOfAppear) / timeToReachDestination[currentPoint]));

                if (Time.time - timeOfAppear > timeToReachDestination[currentPoint])
                {
                    currentPoint++;
                    timeOfAppear = Time.time;

                    if (currentPoint >= points.Length)
                    {
                        timeHealthBarStartGrow = Time.time;
                        currentBehaviour = TestBossARBehaviourMode.HealthBarGrow;
                        _UIControl.updateBossHealth(0);
                        _UIControl.activateHealthBar();
                        healthGrowSource.Play();
                        //currentBehaviour = TestBossBehaviourMode.Attack;
                        //_bossControl.setIsVulnerable(true);
                    }
                }
                break;

            case TestBossARBehaviourMode.HealthBarGrow:

                healthGrowSource.pitch = Mathf.Lerp(0.5f, 2.5f, (Time.time - timeHealthBarStartGrow) / timeForHealthBarToGrow);
                _UIControl.updateBossHealth((Time.time - timeHealthBarStartGrow) / timeForHealthBarToGrow);

                if (Time.time - timeHealthBarStartGrow > timeForHealthBarToGrow)
                {
                    _gameManager.allowHealthBarUpdates();
                    healthGrowSource.Stop();

                    currentBehaviour = TestBossARBehaviourMode.Attack;
                    _bossControl.setIsVulnerable(true);
                }
                break;

            case TestBossARBehaviourMode.Attack:

                for (int i = 0; i < shotSpawn.Length; i++)
                {
                    if (Time.time - currentTimeBetweenShots[i] > amountOfTimeToPassBeforeFire[i])
                    {
                        ///////////////////////
                        shotLead = (player.transform.position.z - shotSpawn[i].position.z) / (shotSpeed + player.GetComponent<PlayerControllerScript>().getCurrentSpeed());
                        shotLead = shotSpeed * shotLead;

                        Quaternion newAngle = Quaternion.Euler(Mathf.Atan2(player.transform.position.x - shotSpawn[i].position.x, player.transform.position.z + shotLead - shotSpawn[i].position.z), Mathf.Atan2(player.transform.position.y - shotSpawn[i].position.y, player.transform.position.z + shotLead - shotSpawn[i].position.z), 0);
                        GameObject newShot = Instantiate(laser, shotSpawn[i].position, newAngle);//Instantiate(enemyShot, transform.position, newAngle);

                        Vector3 direction = new Vector3(player.transform.position.x - shotSpawn[i].position.x, player.transform.position.y - shotSpawn[i].position.y, player.transform.position.z - shotSpawn[i].position.z).normalized;

                        newShot.GetComponent<Rigidbody>().velocity = direction * shotSpeed;
                        //transform.forward.normalized * -shotSpeed;

                        amountOfTimeToPassBeforeFire[i] = Random.Range(minTimeBetweenShot, maxTimeBetweenShots);
                        currentTimeBetweenShots[i] = Time.time;
                        ///////////////////////
                    }
                }

                //Check if the boss is defeated
                if (_bossControl.getCurrentHealthPercentage() <= 0)
                {
                    explosion.Play();
                    explosionLight.enabled = true;
                    currentBehaviour = TestBossARBehaviourMode.Destroyed;
                    GetComponent<Rigidbody>().useGravity = true;
                    explosionSource.Play();
                }

                break;

            case TestBossARBehaviourMode.Destroyed:
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Collide with bombs
        //if (other.gameObject.CompareTag("BombExplosion"))
        //{
        //    ownerOfWeakpoint.damageBoss(other.gameObject.GetComponent<BombExplosionControlScript>().getBombDamage());

        //    Debug.Log(other.gameObject.GetComponent<BombExplosionControlScript>().getBombDamage());
        //}
        if (other.gameObject.CompareTag("PlayerShot") || other.gameObject.CompareTag("ChargeShot"))
        {
            _bossControl.damageBoss(other.gameObject.GetComponent<LaserInformation>().damage);

            Destroy(other.gameObject);
        }
    }
}
