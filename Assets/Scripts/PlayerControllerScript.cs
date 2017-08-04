using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour {

    //Access the UI control script to access/update game variable
    public UIController _UIController;

    private Transform transform;
    private Rigidbody rb;
    private Vector3 movement;
    private Vector3 rotation;

    private bool controlsEnabled;

    private float moveHorizontal;
    private float moveVertical;
    private float moveHorizontalRaw;
    private float moveVerticalRaw;

    private float speed;
    private float verticalSpeed;
    private float bankSpeed;
    private float currentSpeed;
    private float tilt;
    private float zTiltTurnFactor;

    private float minRotX;
    private float minRotY;
    private float maxRotX;
    private float maxRotY;
    private float maxRotZ;
    private float minRotZ;

    private float bankRotationSpeed;
    private float bankingAngle;
    private float currentBankAngle;
    private bool isBanking;
    private bool startBankTimeTaken;
    private float startBankTime;
    private float timeForBank;

    //Laser shot functionality
    public GameObject laserShot;
    public Transform shotSpawn;
    public GameObject twinShot;
    public GameObject hyperShot;
    public int currentLaserMode;
    //public Transform shotSpawn2;

    //Barrel roll functionality
    private bool rollingL;
    private bool tappingL;
    private float lastTapL;
    private float tapTime;
    private bool rollingR;
    private bool tappingR;
    private float lastTapR;

    private float durationOfRoll;
    private float startRollTime;
    public ParticleSystem rollTrails;

    //Boost and break functionality
    public ParticleSystem exhaustTrailL;
    public ParticleSystem exhaustTrailR;
    private float normalExhaustLifeTime;
    private float boostExhaustLifeTime;
    private float breakExhaustLifeTime;
    public AudioSource boostSource;
    public AudioSource breakSource;
    private bool boostTriggered;

    //Variables reflecting UI elements
    private int maxBombs;
    private int numBombs;
    private float maxHealth;
    private float currentHealth;
    public AudioSource hitSource;

    private float maxBoost;
    private float currentBoost;
    private float boostRate;
    private bool boostRecovering;

    //Forward movement
    private int notAtBoss; // 0 if at boss. 1 if not. Multipied by the current speed because not move during boss
    private float normalVelocity;
    private float boostVelocity;
    private float breakVelocity;
    private float currentForwardVelocity;

    //Camera offset
    public float cameraOffset;

    //Bomb functionality
    public GameObject bombShot;
    public Transform bombSpawn;
    private GameObject currentBomb;
    public AudioSource pickupSound;

    //Somersault
    private bool isSomerSaulting;
    private float somerSaultVelocity;

    //Hit effect
    private float durationOfDamageFlash;
    private float currentTimeOfDamageFlash;
    private float timeBetweenFlashes;
    private bool flashOn;
    private float currentTimeBetweenFlashes;

    public Renderer[] rend;

    //Charge shot
    private float fireTimePressed;
    private float durationNeededForCharge;
    public GameObject chargeShot;
    public GameObject twinChargeShot;
    public GameObject hyperChargeShot;
    private GameObject _ChargeShot;
    public Transform chargeShotSpawn;
    public AudioSource lockonSource;
    public GameObject lockonCursor;

    //Ring functionallity
    private float silverRingRecoverAmount;
    private float goldRingRecoverAmount;
    private int numGoldRings;
    public GameObject goldRingAround;
    public GameObject silverRingAround;
    private float timeRingGoldAppear;
    private float timeRingSilverAppear;
    private float durationOfRingOnScreen;

    //Shot deflecting
    public AudioSource deflectionSource;

    //All range
    //TODO change to private when done testing
    public bool inAllRange;

    private float ARTurnRateX;
    private float ARTurnRateY;
    private float allRangeTurnAngleX;
    private float allRangeTurnAngleY;
    private float ARSomersaultTurnRatePenalty;
    private bool isUturning;
    private float uTurnVelocity;
    private bool turnDuringUturnEnabled;
    private float ARUTurnRatePenalty;

    //Game over state
    private bool isDead;
    public ParticleSystem fire;
    public ParticleSystem explosion;
    private float timeOfDeath;
    private float timeToExplode;
    private bool exploded;
    public GameObject[] visualComponents;
    private float crashAngle;
    private float crashAngleIncrement;
    public AudioSource explosionSource;

    private float timeControlsDisabled;
    private float timeForRotationToLevelOutOnLevelComplete;
    private Quaternion rotationAtTimeOfControlsDisabled;
    private Quaternion defaultRotation;

    //For testing purposes
    public bool isInvinsible;

    // Use this for initialization
    void Start ()
    {
        transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        currentLaserMode = 0;

        controlsEnabled = true;

        minRotX = -45;
        minRotY = -45;
        maxRotX = 45;
        maxRotY = 45;
        maxRotZ = 90;
        minRotZ = -90;

        speed = 20;
        bankSpeed = 30;
        currentSpeed = speed;

        verticalSpeed = speed;

        tilt = maxRotX / speed;
        zTiltTurnFactor = 0.5f;

        bankRotationSpeed = 0.1f;
        isBanking = false;
        currentBankAngle = 0;
        startBankTimeTaken = false;
        timeForBank = 0.25f;

        rollingL = false;
        tappingL = false;
        lastTapL = Time.time;
        tapTime = 0.25f;
        rollingR = false;
        tappingR = false;
        lastTapR = Time.time;
        durationOfRoll = 0.5f;
        startRollTime = 0;

        normalExhaustLifeTime = 0.15f;
        boostExhaustLifeTime = 0.3f;
        breakExhaustLifeTime = 0.05f;
        boostTriggered = false;

        maxBombs = 9;
        numBombs = 3;
        maxHealth = 100;
        currentHealth = maxHealth;
        maxBoost = 100;
        currentBoost = 0;
        boostRate = 40; //points per second?
        boostRecovering = false;

        notAtBoss = 1;
        //normalVelocity = 5;
        //boostVelocity = 10f;
        //breakVelocity = 1f;
        normalVelocity = 10;
        boostVelocity = 15f;
        breakVelocity = 5f;
        currentForwardVelocity = normalVelocity;

        currentBomb = null;
        isSomerSaulting = false;
        somerSaultVelocity = 35;

        durationOfDamageFlash = 1;
        currentTimeOfDamageFlash = Time.time - durationOfDamageFlash;
        timeBetweenFlashes = 0.05f;
        currentTimeBetweenFlashes = Time.time - timeBetweenFlashes;

        flashOn = false;
        
        fireTimePressed = Time.time;
        durationNeededForCharge = 1.5f;

        silverRingRecoverAmount = 50;
        goldRingRecoverAmount = 100;
        numGoldRings = 0;
        durationOfRingOnScreen = 1;
        timeRingGoldAppear = Time.time - durationOfRingOnScreen;
        timeRingSilverAppear = Time.time - durationOfRingOnScreen;

        allRangeTurnAngleY = 0;
        ARTurnRateX = 100;
        ARTurnRateY = 100;
        allRangeTurnAngleX = 0;
        ARSomersaultTurnRatePenalty = 20;
        isUturning = false;
        uTurnVelocity = 25;
        turnDuringUturnEnabled = true;
        ARUTurnRatePenalty = 2;

        isDead = false;
        timeToExplode = 3;
        exploded = false;
        crashAngle = 0;
        crashAngleIncrement = 150f;

        timeForRotationToLevelOutOnLevelComplete = 1;
        defaultRotation = Quaternion.Euler(0, 0, 0);
}

    //Should be used instead of update when dealing with object with rigidbody because of physics calculations
    //Done before physics calculations
    void FixedUpdate()
    {
        if (!isDead)
        {
            if (!inAllRange)
            {
                corridorControl();
            }
            else
            {
                allRangeControl();
            }
        }
        else
        {
            crashShip();

            if (!inAllRange)
            {
                forcePlayerInPlayAreaCorridor();
            }

            _UIController.updateUI(numBombs, currentHealth / maxHealth, currentBoost / maxBoost, numGoldRings, transform.position.z);
        }
    }

    //Cause the arwing to bank to the left or right
    void setIsBanking()
    {
        if(isBanking)
        {
            if((Input.GetKey(KeyCode.Q) && moveHorizontal < 0)
                || (Input.GetKey(KeyCode.E) && moveHorizontal > 0))
            {
                currentSpeed = bankSpeed;
            }
            else
            {
                currentSpeed = speed;
            }

            if((Input.GetKey(KeyCode.Q)))
            {
                bankingAngle = 90;
               
            }
            else if((Input.GetKey(KeyCode.E)))
            {
                bankingAngle = -90;
            }

            if(!startBankTimeTaken)
            {
                startBankTime = Time.time;
                startBankTimeTaken = true;
            }

        }
        else
        {
            currentSpeed = speed;

            if (startBankTimeTaken)
            {
                startBankTime = Time.time;
                bankingAngle = currentBankAngle;
                startBankTimeTaken = false;
            }
            
        }

        tilt = maxRotX / currentSpeed;
    }

    //Handle Collsions
    void OnTriggerEnter(Collider other)
    {
        //Collide with bombs
        if (other.gameObject.CompareTag("BombPickup") && !isDead)
        {
            pickupSound.Play();
            Destroy(other.gameObject);

            if (numBombs < 9)
            {
                numBombs++;
            }
        }

        if (other.gameObject.CompareTag("LaserPickup") && !isDead)
        {
            if(currentLaserMode < 2)
            {
                currentLaserMode++;
            }
            pickupSound.Play();
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("SilverRing") && !isDead)
        {
            currentHealth += silverRingRecoverAmount;

            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            timeRingSilverAppear = Time.time;
            pickupSound.Play();
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("GoldRing") && !isDead)
        {
            numGoldRings++;
            currentHealth += goldRingRecoverAmount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            
            timeRingGoldAppear = Time.time;

            if (numGoldRings >= 3)
            {
                numGoldRings = 0;
                _UIController.GetComponent<UIController>().doubleLifeBar();
                maxHealth = maxHealth * 2;
                currentHealth = currentHealth * 2;
            }
            pickupSound.Play();
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("EnemyShot"))
        {
            //Rolling negate damage
            if(!rollingL && !rollingR)
            {
                damagePlayer(other.gameObject.GetComponent<LaserInformation>().damage);
            }
            else
            {
                //Deflect the shot
                deflectionSource.Play();
            }

            Destroy(other.gameObject);
        }
    }

    void corridorControl()
    {
        //Get user input and move the player if the game is still in progess
        //Want get axis because will want controller support
        if (controlsEnabled)
        {
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = -1 * Input.GetAxis("Vertical");
        }
        else
        {
            moveHorizontal = 0;
            moveVertical = 0;

            transform.rotation = Quaternion.Lerp(rotationAtTimeOfControlsDisabled, defaultRotation, (Time.time - timeControlsDisabled) / timeForRotationToLevelOutOnLevelComplete);
        }

        //Check if the player wants to perform a somersault
        if (controlsEnabled && Input.GetKey(KeyCode.R) && (currentBoost == 0) && !boostRecovering && !isSomerSaulting && !rollingL && !rollingR && moveVertical > 0)
        {
            isSomerSaulting = true;
            currentForwardVelocity = 0;
            boostSource.Play();
        }

        //Force the player to stay in the game area
        forcePlayerInPlayAreaCorridor();
        //if (transform.position.x < (cameraOffset))
        //{
        //    transform.position = new Vector3(cameraOffset, transform.position.y, transform.position.z);
        //}
        //if (transform.position.x > (-1 * cameraOffset))
        //{
        //    transform.position = new Vector3(-1 * cameraOffset, transform.position.y, transform.position.z);
        //}
        //if (transform.position.y < (cameraOffset / 2))
        //{
        //    transform.position = new Vector3(transform.position.x, cameraOffset / 2, transform.position.z);
        //}
        //if (transform.position.y > (cameraOffset / -2) && !isSomerSaulting) //If somersaulting allow go out of bounds
        //{
        //    transform.position = new Vector3(transform.position.x, cameraOffset / -2, transform.position.z);
        //}

        //Movement when somersaulting will be different
        if (!isSomerSaulting)
        {
            rb.velocity = new Vector3(moveHorizontal * currentSpeed, moveVertical * verticalSpeed, currentForwardVelocity);
        }

        //If performing a somersault advance the somersault
        if (isSomerSaulting)
        {
            //Rotate the arwing accordingly
            float angle = Mathf.Lerp(0, -360, currentBoost / maxBoost);

            //transform.eulerAngles = new Vector3(angle, 0, 0); //HERE Mathf.Clamp(currentBankAngle + rb.velocity.x * -tilt * zTiltTurnFactor, minRotZ, maxRotZ)
            //transform.eulerAngles = new Vector3(angle, 0, currentBankAngle);
            transform.eulerAngles = new Vector3(angle, 0, Mathf.Clamp(currentBankAngle + rb.velocity.x * -tilt * zTiltTurnFactor, minRotZ, maxRotZ));
            rb.velocity = new Vector3(transform.forward.x * somerSaultVelocity + moveHorizontal * currentSpeed, transform.forward.y * somerSaultVelocity, transform.forward.z * somerSaultVelocity);
            //Mathf.Clamp(rb.velocity.x * tilt, minRotX, maxRotX)
            //rotation = new Vector3
            //(
            //    rb.rotation.x,
            //    Mathf.Clamp(rb.velocity.x * tilt, minRotX, maxRotX) + rb.rotation.y,
            //    rb.rotation.z
            //);
            //rb.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);

            currentBoost += (boostRate * Time.deltaTime);
            if (currentBoost > maxBoost)
            {
                currentBoost = maxBoost;
                isSomerSaulting = false;
                currentForwardVelocity = normalVelocity * notAtBoss;
            }
        }

        //Barrel roll functionallity
        if (controlsEnabled && Input.GetKeyDown(KeyCode.Q) && !rollingL && !rollingR && !isSomerSaulting)
        {
            if (!tappingL)
            {
                tappingL = true;
                lastTapL = Time.time;
            }
            else
            {
                //Start barrel roll to the left
                rollingL = true;
                startRollTime = Time.time;
                rollTrails.Play();
            }
        }
        //Time out the key press if pressed
        if (tappingL && Time.time - lastTapL > tapTime)
        {
            tappingL = false;
        }
        //Right direction barrel roll
        if (controlsEnabled && Input.GetKeyDown(KeyCode.E) && !rollingL && !rollingR && !isSomerSaulting)
        {
            if (!tappingR)
            {
                tappingR = true;
                lastTapR = Time.time;
            }
            else
            {
                //Start barrel roll to the right
                rollingR = true;
                startRollTime = Time.time;
                rollTrails.Play();
            }
        }
        //Time out the key press if pressed
        if (tappingR && Time.time - lastTapR > tapTime)
        {
            tappingR = false;
        }

        //Banking
        if ((!rollingL && !rollingR))// && !isSomerSaulting)
        {
            if (Input.GetKey(KeyCode.Q) ^ Input.GetKey(KeyCode.E))
            {
                isBanking = true;
            }
            else
            {
                isBanking = false;
            }
            setIsBanking();

            //Rotation the z-axis based on the banking
            if (isBanking)
            {
                currentBankAngle = Mathf.Lerp(0, bankingAngle, (Time.time - startBankTime) / timeForBank);
            }
            else
            {
                currentBankAngle = Mathf.Lerp(bankingAngle, 0, (Time.time - startBankTime) / timeForBank);
            }
        }

        //If barrel rolling, set the banking angle to the current point in the barrel roll
        if ((rollingL || rollingR) && !isSomerSaulting)
        {
            if (rollingL)
            {
                currentBankAngle = Mathf.Lerp(0, 360, (Time.time - startRollTime) / durationOfRoll);
                if (moveHorizontal < 0)
                {
                    currentSpeed = bankSpeed;
                }
                else
                {
                    currentSpeed = speed;
                }
            }
            else if (rollingR)
            {
                currentBankAngle = Mathf.Lerp(0, -360, (Time.time - startRollTime) / durationOfRoll);
                if (moveHorizontal > 0)
                {
                    currentSpeed = bankSpeed;
                }
                else
                {
                    currentSpeed = speed;
                }
            }

            if ((Time.time - startRollTime) > durationOfRoll)
            {
                currentBankAngle = 0;
                rollingL = false;
                rollingR = false;
                rollTrails.Stop();
            }
        }

        if (!isSomerSaulting)
        {
            if (rollingL || rollingR)
            {
                rotation = new Vector3
                (
                Mathf.Clamp(rb.velocity.y * -tilt, minRotX, maxRotX),
                Mathf.Clamp(rb.velocity.x * tilt, minRotX, maxRotX),
                currentBankAngle
            );
                rb.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            }
            else if(controlsEnabled)
            {
                rotation = new Vector3
                (
                Mathf.Clamp(rb.velocity.y * -tilt, minRotX, maxRotX),
                Mathf.Clamp(rb.velocity.x * tilt, minRotX, maxRotX),
                Mathf.Clamp(currentBankAngle + rb.velocity.x * -tilt * zTiltTurnFactor, minRotZ, maxRotZ)
            );
                rb.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            }
        }
        else
        {
            // rb.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, currentBankAngle);
        }

        //Breaking and boosting
        //TODO Check if have enough meter to do so
        //Maybe should lerp to the new life time and speeds
        //ALSO TODO ACTUALLY ADjUST THE SPEED WHENEVER YOU DECIDE TO ACTUALLY IMPLEMENT THAT

        //Boost
        if (controlsEnabled && Input.GetKey(KeyCode.R) && (currentBoost < maxBoost) && !boostRecovering && !isSomerSaulting)
        {
            currentBoost += (boostRate * Time.deltaTime);
            if (currentBoost > maxBoost)
            {
                currentBoost = maxBoost;
            }

            if (!boostTriggered)
            {
                boostTriggered = true;
                boostSource.Play();
            }

            exhaustTrailL.startLifetime = boostExhaustLifeTime;
            exhaustTrailR.startLifetime = boostExhaustLifeTime;

            currentForwardVelocity = boostVelocity * notAtBoss;
        }
        //Break
        else if (controlsEnabled && Input.GetKey(KeyCode.F) && (currentBoost < maxBoost) && !boostRecovering && !isSomerSaulting)
        {
            currentBoost += (boostRate * Time.deltaTime);
            if (currentBoost > maxBoost)
            {
                currentBoost = maxBoost;
            }

            if (!boostTriggered)
            {
                boostTriggered = true;
                breakSource.Play();
            }

            exhaustTrailL.startLifetime = breakExhaustLifeTime;
            exhaustTrailR.startLifetime = breakExhaustLifeTime;

            currentForwardVelocity = breakVelocity * notAtBoss;
        }
        else if (!isSomerSaulting)//Normal
        {
            boostRecovering = true;

            currentBoost -= (boostRate * Time.deltaTime);
            if (currentBoost <= 0)
            {
                currentBoost = 0;
                boostRecovering = false;
                boostTriggered = false;
                boostSource.Stop();
                breakSource.Stop();
            }

            exhaustTrailL.startLifetime = normalExhaustLifeTime;
            exhaustTrailR.startLifetime = normalExhaustLifeTime;

            currentForwardVelocity = normalVelocity * notAtBoss;
        }


        //Check if the player is firing
        if (controlsEnabled && Input.GetButtonDown("Fire1"))
        {
            //Create the shot
            if (currentLaserMode == 0)
            {
                Instantiate(laserShot, shotSpawn.position, rb.rotation);
            }
            else if (currentLaserMode == 1)
            {
                Instantiate(twinShot, shotSpawn.position, rb.rotation);
            }
            else if (currentLaserMode == 2)
            {
                Instantiate(hyperShot, shotSpawn.position, rb.rotation);
            }

            //TODO the quick hold tripple shot

            //Charge Shot
            fireTimePressed = Time.time;
            //Create the charge shot
            if (currentLaserMode == 0)
            {
                _ChargeShot = Instantiate(chargeShot, chargeShotSpawn.position, rb.rotation);
            }
            else if (currentLaserMode == 1)
            {
                _ChargeShot = Instantiate(twinChargeShot, chargeShotSpawn.position, rb.rotation);
            }
            else if (currentLaserMode == 2)
            {
                _ChargeShot = Instantiate(hyperChargeShot, chargeShotSpawn.position, rb.rotation);
            }
            //_ChargeShot = Instantiate(chargeShot, chargeShotSpawn.position, rb.rotation);
            _ChargeShot.GetComponent<ChargeShotControllerScript>().player = gameObject;
            _ChargeShot.GetComponent<ChargeShotControllerScript>().chargeShotSpawn = bombSpawn;
        }
        //Check if release a charge shot if held long enough
        if (controlsEnabled && Input.GetButtonUp("Fire1"))
        {
            if (_ChargeShot != null)
            {
                //If enough time passed, active the charge shot and release it
                if (Time.time - fireTimePressed > durationNeededForCharge)
                {
                    _ChargeShot.GetComponent<ChargeShotControllerScript>().fire();
                }
                //If not, destroy it
                else
                {
                    //Destroy(_ChargeShot);
                }
            }
        }

        //Check if firing a bomb
        if (controlsEnabled && Input.GetButtonDown("Fire2"))
        {
            //Create the shot
            if (numBombs > 0 && currentBomb == null)
            {
                currentBomb = Instantiate(bombShot, bombSpawn.position, rb.rotation);
                numBombs--;

                if (_ChargeShot != null)
                {
                    if (_ChargeShot.GetComponent<ChargeShotControllerScript>().homingTarget != null)
                    {
                        currentBomb.GetComponent<BombShotControlScript>().homingTarget = _ChargeShot.GetComponent<ChargeShotControllerScript>().homingTarget;
                    }
                }

            }
            else if (currentBomb != null)
            {
                currentBomb.GetComponent<BombShotControlScript>().explode();
            }
        }

        //Check if has a charge shot
        if (_ChargeShot != null)
        {
            if (_ChargeShot.GetComponent<ChargeShotControllerScript>().homingTarget == null)
            {
                //Check if charge shot is ready
                if (Time.time - fireTimePressed > durationNeededForCharge)
                {
                    //Check if the direction the player is aiming collides with an enemy
                    RaycastHit hit;

                    if (Physics.Raycast(bombSpawn.position, transform.forward.normalized, out hit))//, 6 * col.radius))
                    {
                        if (hit.collider.gameObject.CompareTag("Enemy"))
                        {
                            //If does collide, mark that enemy has the lock on target and give that target to the charge shot so if released, will home in on that enemy
                            _ChargeShot.GetComponent<ChargeShotControllerScript>().homingTarget = hit.collider.gameObject;

                            //Also play a sound effect
                            lockonSource.Play();

                            //Move the secondary cursor to the emeny
                            hit.collider.gameObject.GetComponent<DamagableByPlayer>().changeLockOnStatus(true);
                        }
                    }
                }
            }
        }

        //Update the visual effect for the rings being collected
        if (Time.time - timeRingGoldAppear < durationOfRingOnScreen)
        {
            if (Time.time - timeRingGoldAppear < durationOfRingOnScreen / 2)
            {
                float scale = Mathf.Lerp(0, 1, (Time.time - timeRingGoldAppear) / (durationOfRingOnScreen / 2));
                goldRingAround.transform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                float scale = Mathf.Lerp(1, 0, (Time.time - timeRingGoldAppear - (durationOfRingOnScreen / 2)) / (durationOfRingOnScreen / 2));
                goldRingAround.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        if (Time.time - timeRingSilverAppear < durationOfRingOnScreen)
        {
            if (Time.time - timeRingSilverAppear < durationOfRingOnScreen / 2)
            {
                float scale = Mathf.Lerp(0, 1, (Time.time - timeRingSilverAppear) / (durationOfRingOnScreen / 2));
                silverRingAround.transform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                float scale = Mathf.Lerp(1, 0, (Time.time - timeRingSilverAppear - (durationOfRingOnScreen / 2)) / (durationOfRingOnScreen / 2));
                silverRingAround.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        //Finally update the UI
        _UIController.updateUI(numBombs, currentHealth / maxHealth, currentBoost / maxBoost, numGoldRings, transform.position.z);
    }

    void allRangeControl()
    {
        //Get user input and move the player if the game is still in progess
        //Want get axis because will want controller support
        if(controlsEnabled)
        {
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = -1 * Input.GetAxis("Vertical");
            moveHorizontalRaw = Input.GetAxisRaw("Horizontal");
            moveVerticalRaw = -1 * Input.GetAxisRaw("Vertical");
        }
        else
        {
            moveHorizontal = 0;
            moveVertical = 0;
            moveHorizontalRaw = 0;
            moveVerticalRaw = 0;
        }
        

        //Check if the player wants to perform a somersault
        if (controlsEnabled && Input.GetKey(KeyCode.R) && (currentBoost == 0) && !boostRecovering && !isSomerSaulting && !isUturning && !rollingL && !rollingR && moveVertical > 0)
        {
            isSomerSaulting = true;
            currentForwardVelocity = 0;
            boostSource.Play();
        }
        //U-turn
        if (controlsEnabled && Input.GetKey(KeyCode.F) && (currentBoost == 0) && !boostRecovering && !isSomerSaulting && !isUturning && !rollingL && !rollingR && moveVertical > 0)
        {
            isUturning = true;
            currentForwardVelocity = 0;
            boostSource.Play();
        }

        //Force the player to stay in the game area. If leave do a uturn to get back into the area
        //if (transform.position.x < (cameraOffset))
        //{
        //    transform.position = new Vector3(cameraOffset, transform.position.y, transform.position.z);
        //}
        //if (transform.position.x > (-1 * cameraOffset))
        //{
        //    transform.position = new Vector3(-1 * cameraOffset, transform.position.y, transform.position.z);
        //}
        //if (transform.position.y < (cameraOffset / 2))
        //{
        //    transform.position = new Vector3(transform.position.x, cameraOffset / 2, transform.position.z);
        //}
        //if (transform.position.y > (cameraOffset / -2) && !isSomerSaulting) //If somersaulting allow go out of bounds
        //{
        //    transform.position = new Vector3(transform.position.x, cameraOffset / -2, transform.position.z);
        //}

        //Turning
        if (isSomerSaulting)
        {
            allRangeTurnAngleY += (ARTurnRateY / ARSomersaultTurnRatePenalty) * moveHorizontal * Time.deltaTime;
        }
        else if((isUturning && currentBoost / maxBoost > 0.5f && turnDuringUturnEnabled))
        {
            allRangeTurnAngleY += (ARTurnRateY / ARUTurnRatePenalty) * moveHorizontal * Time.deltaTime;
        }
        else if(!isUturning)
        {
            allRangeTurnAngleY += ARTurnRateY * moveHorizontal * Time.deltaTime;
        }

        //Movement when somersaulting will be different
        if (!isSomerSaulting && !isUturning)
        {
            //rb.velocity = new Vector3(moveHorizontal * currentSpeed, moveVertical * verticalSpeed, currentForwardVelocity);

            //Rotate based on the axis inputs
            //rb.rotation = Quaternion.Euler(0, allRangeTurnAngleY * moveHorizontal, 0);
            //allRangeTurnAngleY += 20 * moveHorizontal;
            
            if (moveVerticalRaw != 0)
            {
                allRangeTurnAngleX -= ARTurnRateX * moveVertical * Time.deltaTime;
                allRangeTurnAngleX = Mathf.Clamp(allRangeTurnAngleX, minRotX, maxRotX);
            }
            else if (allRangeTurnAngleX != 0)
            {
                bool positive = allRangeTurnAngleX > 0;

                if (positive)
                {
                    allRangeTurnAngleX -= ARTurnRateX * Time.deltaTime;
                    if (allRangeTurnAngleX < 0)
                    {
                        allRangeTurnAngleX = 0;
                    }
                }
                else
                {
                    allRangeTurnAngleX += ARTurnRateX * Time.deltaTime;
                    if (allRangeTurnAngleX > 0)
                    {
                        allRangeTurnAngleX = 0;
                    }
                }
            }

            rb.velocity = transform.forward * currentForwardVelocity;
            //If banking, increase the horizontal speed
            rb.velocity += transform.up.normalized * (moveHorizontalRaw * (currentSpeed - speed));
            
            //rb.velocity = new Vector3(rb.velocity.x + (moveHorizontalRaw * (currentSpeed - speed)), rb.velocity.y, rb.velocity.z);
        }

        //If performing a somersault advance the somersault
        if (isSomerSaulting)
        {
            //Rotate the arwing accordingly
            float angle = Mathf.Lerp(0, -360, currentBoost / maxBoost);

            //transform.eulerAngles = new Vector3(angle, 0, Mathf.Clamp(currentBankAngle + rb.velocity.x * -tilt * zTiltTurnFactor, minRotZ, maxRotZ));
            transform.eulerAngles = new Vector3(angle, allRangeTurnAngleY, Mathf.Clamp(currentBankAngle, minRotZ, maxRotZ));
            rb.velocity = new Vector3(transform.forward.x * somerSaultVelocity + moveHorizontal * currentSpeed, transform.forward.y * somerSaultVelocity, transform.forward.z * somerSaultVelocity);
            
            currentBoost += (boostRate * Time.deltaTime);
            if (currentBoost > maxBoost)
            {
                currentBoost = maxBoost;
                isSomerSaulting = false;
                allRangeTurnAngleX = 0;
                currentForwardVelocity = normalVelocity * notAtBoss;
            }
        }
        //Uturning
        if(isUturning)
        {
            //Rotate the arwing accordingly
            if (currentBoost / maxBoost < 0.5f)
            {
                float angle = Mathf.Lerp(0, -180, 2 * currentBoost / maxBoost);
                transform.eulerAngles = new Vector3(angle, allRangeTurnAngleY, Mathf.Clamp(currentBankAngle, minRotZ, maxRotZ));
                //rb.velocity = new Vector3(transform.forward.x * uTurnVelocity, transform.forward.y * uTurnVelocity, transform.forward.z * uTurnVelocity);
            }
            else
            {
                float angle = Mathf.Lerp(0, -180, (2 * currentBoost / maxBoost) - 1);
                transform.eulerAngles = new Vector3(-180, allRangeTurnAngleY, currentBankAngle + angle);
                //rb.velocity = new Vector3(transform.forward.x * uTurnVelocity + moveHorizontal * currentSpeed, transform.forward.y * uTurnVelocity, transform.forward.z * uTurnVelocity);
            }

            //rb.velocity = new Vector3(transform.forward.x * uTurnVelocity + moveHorizontal * currentSpeed, transform.forward.y * uTurnVelocity, transform.forward.z * uTurnVelocity);
            rb.velocity = new Vector3(transform.forward.x * uTurnVelocity, transform.forward.y * uTurnVelocity, transform.forward.z * uTurnVelocity);

            currentBoost += (boostRate * Time.deltaTime);
            if (currentBoost > maxBoost)
            {
                turnDuringUturnEnabled = true;
                currentBoost = maxBoost;
                isUturning = false;
                allRangeTurnAngleY -= 180;
                allRangeTurnAngleX = 0;
                transform.eulerAngles = new Vector3(0, allRangeTurnAngleY, currentBankAngle);
                currentForwardVelocity = normalVelocity * notAtBoss;
            }
        }

        //Barrel roll functionallity
        if (controlsEnabled && Input.GetKeyDown(KeyCode.Q) && !rollingL && !rollingR && !isSomerSaulting && !isUturning)
        {
            if (!tappingL)
            {
                tappingL = true;
                lastTapL = Time.time;
            }
            else
            {
                //Start barrel roll to the left
                rollingL = true;
                startRollTime = Time.time;
                rollTrails.Play();
            }
        }
        //Time out the key press if pressed
        if (tappingL && Time.time - lastTapL > tapTime)
        {
            tappingL = false;
        }
        //Right direction barrel roll
        if (controlsEnabled && Input.GetKeyDown(KeyCode.E) && !rollingL && !rollingR && !isSomerSaulting && !isUturning)
        {
            if (!tappingR)
            {
                tappingR = true;
                lastTapR = Time.time;
            }
            else
            {
                //Start barrel roll to the right
                rollingR = true;
                startRollTime = Time.time;
                rollTrails.Play();
            }
        }
        //Time out the key press if pressed
        if (tappingR && Time.time - lastTapR > tapTime)
        {
            tappingR = false;
        }

        //Banking
        if ((!rollingL && !rollingR))// && !isSomerSaulting)
        {
            if (Input.GetKey(KeyCode.Q) ^ Input.GetKey(KeyCode.E))
            {
                isBanking = true;
            }
            else
            {
                isBanking = false;
            }
            setIsBanking();

            //Rotation the z-axis based on the banking
            if (isBanking)
            {
                currentBankAngle = Mathf.Lerp(0, bankingAngle, (Time.time - startBankTime) / timeForBank);
            }
            else
            {
                currentBankAngle = Mathf.Lerp(bankingAngle, 0, (Time.time - startBankTime) / timeForBank);
            }
        }

        //If barrel rolling, set the banking angle to the current point in the barrel roll
        if ((rollingL || rollingR) && !isSomerSaulting && !isUturning)
        {
            if (rollingL)
            {
                currentBankAngle = Mathf.Lerp(0, 360, (Time.time - startRollTime) / durationOfRoll);
                if (moveHorizontal < 0)
                {
                    currentSpeed = bankSpeed;
                }
                else
                {
                    currentSpeed = speed;
                }
            }
            else if (rollingR)
            {
                currentBankAngle = Mathf.Lerp(0, -360, (Time.time - startRollTime) / durationOfRoll);
                if (moveHorizontal > 0)
                {
                    currentSpeed = bankSpeed;
                }
                else
                {
                    currentSpeed = speed;
                }
            }

            if ((Time.time - startRollTime) > durationOfRoll)
            {
                currentBankAngle = 0;
                rollingL = false;
                rollingR = false;
                rollTrails.Stop();
            }
        }

        if (!isSomerSaulting && !isUturning)
        {
            if (rollingL || rollingR)
            {
                rotation = new Vector3
                (
                    allRangeTurnAngleX,
                    allRangeTurnAngleY,
                    //Mathf.Clamp(rb.velocity.y * -tilt, minRotX, maxRotX),
                    //Mathf.Clamp(rb.velocity.x * tilt, minRotX, maxRotX),
                    currentBankAngle
                );
                rb.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            }
            else
            {
                rotation = new Vector3
                (
                    //Mathf.Clamp(rb.velocity.y * -tilt, minRotX, maxRotX),
                    //0,
                    allRangeTurnAngleX,
                    //Mathf.Clamp(rb.velocity.x * tilt, minRotX, maxRotX),
                    allRangeTurnAngleY,
                    Mathf.Clamp(currentBankAngle, minRotZ, maxRotZ)
                    //Mathf.Clamp(currentBankAngle + rb.velocity.x * -tilt * zTiltTurnFactor, minRotZ, maxRotZ)
                );
                rb.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            }
        }

        //Breaking and boosting
        //Boost
        if (controlsEnabled && Input.GetKey(KeyCode.R) && (currentBoost < maxBoost) && !boostRecovering && !isSomerSaulting && !isUturning)
        {
            currentBoost += (boostRate * Time.deltaTime);
            if (currentBoost > maxBoost)
            {
                currentBoost = maxBoost;
            }

            if (!boostTriggered)
            {
                boostTriggered = true;
                boostSource.Play();
            }

            exhaustTrailL.startLifetime = boostExhaustLifeTime;
            exhaustTrailR.startLifetime = boostExhaustLifeTime;

            currentForwardVelocity = boostVelocity * notAtBoss;
        }
        //Break
        else if (controlsEnabled && Input.GetKey(KeyCode.F) && (currentBoost < maxBoost) && !boostRecovering && !isSomerSaulting && !isUturning)
        {
            currentBoost += (boostRate * Time.deltaTime);
            if (currentBoost > maxBoost)
            {
                currentBoost = maxBoost;
            }

            if (!boostTriggered)
            {
                boostTriggered = true;
                breakSource.Play();
            }

            exhaustTrailL.startLifetime = breakExhaustLifeTime;
            exhaustTrailR.startLifetime = breakExhaustLifeTime;

            currentForwardVelocity = breakVelocity * notAtBoss;
        }
        else if (!isSomerSaulting && !isUturning)//Normal
        {
            boostRecovering = true;

            currentBoost -= (boostRate * Time.deltaTime);
            if (currentBoost <= 0)
            {
                currentBoost = 0;
                boostRecovering = false;
                boostTriggered = false;
                boostSource.Stop();
                breakSource.Stop();
            }

            exhaustTrailL.startLifetime = normalExhaustLifeTime;
            exhaustTrailR.startLifetime = normalExhaustLifeTime;

            currentForwardVelocity = normalVelocity * notAtBoss;
        }


        //Check if the player is firing
        if (Input.GetButtonDown("Fire1"))
        {
            //Create the shot
            if (currentLaserMode == 0)
            {
                Instantiate(laserShot, shotSpawn.position, rb.rotation);
            }
            else if (currentLaserMode == 1)
            {
                Instantiate(twinShot, shotSpawn.position, rb.rotation);
            }
            else if (currentLaserMode == 2)
            {
                Instantiate(hyperShot, shotSpawn.position, rb.rotation);
            }

            //TODO the quick hold tripple shot

            //Charge Shot
            fireTimePressed = Time.time;
            //Create the charge shot
            if (currentLaserMode == 0)
            {
                _ChargeShot = Instantiate(chargeShot, chargeShotSpawn.position, rb.rotation);
            }
            else if (currentLaserMode == 1)
            {
                _ChargeShot = Instantiate(twinChargeShot, chargeShotSpawn.position, rb.rotation);
            }
            else if (currentLaserMode == 2)
            {
                _ChargeShot = Instantiate(hyperChargeShot, chargeShotSpawn.position, rb.rotation);
            }
            //_ChargeShot = Instantiate(chargeShot, chargeShotSpawn.position, rb.rotation);
            _ChargeShot.GetComponent<ChargeShotControllerScript>().player = gameObject;
            _ChargeShot.GetComponent<ChargeShotControllerScript>().chargeShotSpawn = bombSpawn;
        }
        //Check if release a charge shot if held long enough
        if (Input.GetButtonUp("Fire1"))
        {
            if (_ChargeShot != null)
            {
                //If enough time passed, active the charge shot and release it
                if (Time.time - fireTimePressed > durationNeededForCharge)
                {
                    _ChargeShot.GetComponent<ChargeShotControllerScript>().fire();
                }
                //If not, destroy it
                else
                {
                    //Destroy(_ChargeShot);
                }
            }
        }

        //Check if firing a bomb
        if (Input.GetButtonDown("Fire2"))
        {
            //Create the shot
            if (numBombs > 0 && currentBomb == null)
            {
                currentBomb = Instantiate(bombShot, bombSpawn.position, rb.rotation);
                numBombs--;

                if (_ChargeShot != null)
                {
                    if (_ChargeShot.GetComponent<ChargeShotControllerScript>().homingTarget != null)
                    {
                        currentBomb.GetComponent<BombShotControlScript>().homingTarget = _ChargeShot.GetComponent<ChargeShotControllerScript>().homingTarget;
                    }
                }

            }
            else if (currentBomb != null)
            {
                currentBomb.GetComponent<BombShotControlScript>().explode();
            }
        }

        //Check if has a charge shot
        if (_ChargeShot != null)
        {
            if (_ChargeShot.GetComponent<ChargeShotControllerScript>().homingTarget == null)
            {
                //Check if charge shot is ready
                if (Time.time - fireTimePressed > durationNeededForCharge)
                {
                    //Check if the direction the player is aiming collides with an enemy
                    RaycastHit hit;

                    if (Physics.Raycast(bombSpawn.position, transform.forward.normalized, out hit))//, 6 * col.radius))
                    {
                        if (hit.collider.gameObject.CompareTag("Enemy"))
                        {
                            //If does collide, mark that enemy has the lock on target and give that target to the charge shot so if released, will home in on that enemy
                            _ChargeShot.GetComponent<ChargeShotControllerScript>().homingTarget = hit.collider.gameObject;

                            //Also play a sound effect
                            lockonSource.Play();

                            //Move the secondary cursor to the emeny
                            hit.collider.gameObject.GetComponent<DamagableByPlayer>().changeLockOnStatus(true);
                        }
                    }
                }
            }
        }

        //Update the visual effect for the rings being collected
        if (Time.time - timeRingGoldAppear < durationOfRingOnScreen)
        {
            if (Time.time - timeRingGoldAppear < durationOfRingOnScreen / 2)
            {
                float scale = Mathf.Lerp(0, 1, (Time.time - timeRingGoldAppear) / (durationOfRingOnScreen / 2));
                goldRingAround.transform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                float scale = Mathf.Lerp(1, 0, (Time.time - timeRingGoldAppear - (durationOfRingOnScreen / 2)) / (durationOfRingOnScreen / 2));
                goldRingAround.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        if (Time.time - timeRingSilverAppear < durationOfRingOnScreen)
        {
            if (Time.time - timeRingSilverAppear < durationOfRingOnScreen / 2)
            {
                float scale = Mathf.Lerp(0, 1, (Time.time - timeRingSilverAppear) / (durationOfRingOnScreen / 2));
                silverRingAround.transform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                float scale = Mathf.Lerp(1, 0, (Time.time - timeRingSilverAppear - (durationOfRingOnScreen / 2)) / (durationOfRingOnScreen / 2));
                silverRingAround.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        //Finally update the UI
        _UIController.updateUI(numBombs, currentHealth / maxHealth, currentBoost / maxBoost, numGoldRings, transform.position.z);
    }

    void Update()
    {
        if (Time.time - currentTimeOfDamageFlash < durationOfDamageFlash)
        {
            if (Time.time - currentTimeBetweenFlashes > timeBetweenFlashes)
            {
                currentTimeBetweenFlashes = Time.time;

                if (flashOn)
                {
                    flashOn = false;
                    for(int i = 0; i < rend.Length; i++)
                    {
                        rend[i].material.SetFloat("_FlashTintBool", 0);
                    }
                }
                else
                {
                    flashOn = true;
                    for (int i = 0; i < rend.Length; i++)
                    {
                        rend[i].material.SetFloat("_FlashTintBool", 1);
                    }
                }
            }
        }
        else if(flashOn)
        {
            flashOn = false;
            for (int i = 0; i < rend.Length; i++)
            {
                rend[i].material.SetFloat("_FlashTintBool", 0);
            }
        }
    }

    private void crashShip()
    {
        if(!exploded && Time.time - timeOfDeath > timeToExplode)
        {
            for(int i = 0; i < visualComponents.Length; i++)
            {
                visualComponents[i].SetActive(false);
            }

            fire.Stop();
            explosion.Play();
            explosionSource.Play();
            exploded = true;
        }
        else
        {
            //Spin ship
            if (!exploded)
            {
                transform.Rotate(0, 0, crashAngleIncrement * Time.deltaTime);
            }
        }
    }

    public bool getIsSomerSaulting()
    {
        return isSomerSaulting;
    }

    public void disableCursor()
    {
        visualComponents[0].SetActive(false);
        visualComponents[1].SetActive(false);
    }

    public void damagePlayer(float damage)
    {
        //Cant be hurt if not in control
        if (controlsEnabled)
        {
            if (!isInvinsible)
            {
                currentHealth -= damage;
            }
            hitSource.Play();

            currentTimeOfDamageFlash = Time.time;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                //Debug.Log("Game over");

                //_UIController.updateUI(numBombs, currentHealth / maxHealth, currentBoost / maxBoost, numGoldRings, transform.position.z);

                isDead = true;
                controlsEnabled = false;

                isSomerSaulting = false;
                isUturning = false;
                rollingL = false;
                rollingR = false;

                timeOfDeath = Time.time;
                crashAngle = transform.rotation.z;

                //Active the fire
                fire.Play();
                //_gameManager.playerHasDied();
            }
        }
    }
    
    public float getCurrentHealth()
    {
        return currentHealth;
    }

    public float getPercentageDurationForChargeShot()
    {
        return (Time.time - fireTimePressed) / durationNeededForCharge;
    }

    public float getCurrentLaser()
    {
        return currentLaserMode;
    }

    public float getCurrentSpeed()
    {
        return currentSpeed;
    }

    public float getDefaultForwardSpeed()
    {
        return normalVelocity;
    }

    public float getCurrentForwardVelocity()
    {
        return currentForwardVelocity;
    }

    public void setAtBoss(bool isAtBoss)
    {
        if(isAtBoss)
        {
            notAtBoss = 0;
        }
        else
        {
            notAtBoss = 1;
        }
    }

    public bool isInAllRange()
    {
        return inAllRange;
    }

    public Vector3 getForward()
    {
        return transform.forward;
    }

    public void movePlayerBackIntoBounds()
    {
        if(!isUturning && !isSomerSaulting && !rollingL && !rollingR)
        {
            currentBoost = 0;
            turnDuringUturnEnabled = false;
            isUturning = true;
        }
    }

    public void setPlayerControlEnable(bool status)
    {
        controlsEnabled = status;

        if(!status)
        {
            timeControlsDisabled = Time.time;
            rotationAtTimeOfControlsDisabled = transform.rotation;
        }
    }

    //public void cancelMovement()
    //{
    //    rb.velocity = new Vector3(0, 0, 0);
    //}

    public bool getPlayerControlEnabled()
    {
        return controlsEnabled;
    }

    public bool getIsUturning()
    {
        return isUturning;
    }

    private void forcePlayerInPlayAreaCorridor()
    {
        //Force the player to stay in the game area
        if (transform.position.x < (cameraOffset))
        {
            transform.position = new Vector3(cameraOffset, transform.position.y, transform.position.z);
        }
        if (transform.position.x > (-1 * cameraOffset))
        {
            transform.position = new Vector3(-1 * cameraOffset, transform.position.y, transform.position.z);
        }
        if (transform.position.y < (cameraOffset / 2))
        {
            transform.position = new Vector3(transform.position.x, cameraOffset / 2, transform.position.z);
        }
        if (transform.position.y > (cameraOffset / -2) && !isSomerSaulting) //If somersaulting allow go out of bounds
        {
            transform.position = new Vector3(transform.position.x, cameraOffset / -2, transform.position.z);
        }
    }
}
