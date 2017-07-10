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

    private float moveHorizontal;
    private float moveVertical;

    private float speed;
    private float verticalSpeed;
    private float bankSpeed;
    private float currentSpeed;
    private float tilt;

    private float minRotX;
    private float minRotY;
    private float maxRotX;
    private float maxRotY;

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
    private bool atBoss;
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
    private GameObject _ChargeShot;
    public Transform chargeShotSpawn;

    // Use this for initialization
    void Start ()
    {
        transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        minRotX = -45;
        minRotY = -45;
        maxRotX = 45;
        maxRotY = 45;

        speed = 20;
        bankSpeed = 30;
        currentSpeed = speed;

        verticalSpeed = speed;

        tilt = maxRotX / speed;

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

        atBoss = false;
        normalVelocity = 5;
        boostVelocity = 10f;
        breakVelocity = 1f;
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
    }

    //Should be used instead of update when dealing with object with rigidbody because of physics calculations
    //Done before physics calculations
    void FixedUpdate()
    {
        //Get user input and move the player if the game is still in progess
        //Want get axis because will want controller support
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = -1 * Input.GetAxis("Vertical");

        //Check if the player wants to perform a somersault
        if (Input.GetKey(KeyCode.R) && (currentBoost == 0) && !boostRecovering && !isSomerSaulting && !rollingL && !rollingR && moveVertical > 0)
        {
            isSomerSaulting = true;
            currentForwardVelocity = 0;
            boostSource.Play();
        }

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

        //Movement when somersaulting will be different
        if(!isSomerSaulting)
        {
            rb.velocity = new Vector3(moveHorizontal * currentSpeed, moveVertical * verticalSpeed, currentForwardVelocity);
        }

        //If performing a somersault advance the somersault
        if(isSomerSaulting)
        {
            //Rotate the arwing accordingly
            float angle = Mathf.Lerp(0, -360, currentBoost / maxBoost);

            //transform.eulerAngles = new Vector3(angle, 0, 0);
            transform.eulerAngles = new Vector3(angle, 0, currentBankAngle);
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
                currentForwardVelocity = normalVelocity;
            }
        }

        //Barrel roll functionallity
        if (Input.GetKeyDown(KeyCode.Q) && !rollingL && !rollingR && !isSomerSaulting)
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
        if (Input.GetKeyDown(KeyCode.E) && !rollingL && !rollingR && !isSomerSaulting)
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
            if(rollingL)
            {
                currentBankAngle = Mathf.Lerp(0, 360, (Time.time - startRollTime) / durationOfRoll);
                if(moveHorizontal < 0)
                {
                    currentSpeed = bankSpeed;
                }
                else
                {
                    currentSpeed = speed;
                }
            }
            else if(rollingR)
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

            if((Time.time - startRollTime) > durationOfRoll)
            {
                currentBankAngle = 0;
                rollingL = false;
                rollingR = false;
                rollTrails.Stop();
            }
        }

        if (!isSomerSaulting)
        {
            rotation = new Vector3
            (
                Mathf.Clamp(rb.velocity.y * -tilt, minRotX, maxRotX),
                Mathf.Clamp(rb.velocity.x * tilt, minRotX, maxRotX),
                currentBankAngle
            );
            rb.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
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
        if (Input.GetKey(KeyCode.R) && (currentBoost < maxBoost) && !boostRecovering && !isSomerSaulting)
        {
            currentBoost += (boostRate * Time.deltaTime);
            if (currentBoost > maxBoost)
            {
                currentBoost = maxBoost;
            }

            if(!boostTriggered)
            {
                boostTriggered = true;
                boostSource.Play();
            }
            
            exhaustTrailL.startLifetime = boostExhaustLifeTime;
            exhaustTrailR.startLifetime = boostExhaustLifeTime;

            currentForwardVelocity = boostVelocity;
        }
        //Break
        else if(Input.GetKey(KeyCode.F) && (currentBoost < maxBoost) && !boostRecovering && !isSomerSaulting)
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

            currentForwardVelocity = breakVelocity;
        }
        else if(!isSomerSaulting)//Normal
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

            currentForwardVelocity = normalVelocity;
        }
        

        //Check if the player is firing
        if (Input.GetButtonDown("Fire1"))
        {
            //Create the shot
            Instantiate(laserShot, shotSpawn.position, rb.rotation);

            //TODO the quick hold tripple shot

            //Charge Shot
            fireTimePressed = Time.time;
            //Create the charge shot
            _ChargeShot = Instantiate(chargeShot, chargeShotSpawn.position, rb.rotation);
            _ChargeShot.GetComponent<ChargeShotControllerScript>().player = gameObject;
            _ChargeShot.GetComponent<ChargeShotControllerScript>().chargeShotSpawn = bombSpawn;
        }
        //Check if release a charge shot if held long enough
        if(Input.GetButtonUp("Fire1"))
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
            if(numBombs > 0 && currentBomb == null)
            {
                currentBomb = Instantiate(bombShot, bombSpawn.position, rb.rotation);
                numBombs--;
            }
            else if(currentBomb != null)
            {
                currentBomb.GetComponent<BombShotControlScript>().explode();
            }
        }

        //Finally update the UI
        _UIController.updateUI(numBombs, currentHealth/maxHealth, currentBoost/maxBoost);
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
        if (other.gameObject.CompareTag("BombPickup"))
        {
            pickupSound.Play();
            Destroy(other.gameObject);

            if (numBombs < 9)
            {
                numBombs++;
            }
        }
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

    public bool getIsSomerSaulting()
    {
        return isSomerSaulting;
    }

    public void damagePlayer(float damage)
    {
        currentHealth -= damage;
        hitSource.Play();

        currentTimeOfDamageFlash = Time.time;

        if (currentHealth <= 0)
        {
            Debug.Log("Game over");
        }
    }

    public float getPercentageDurationForChargeShot()
    {
        return (Time.time - fireTimePressed) / durationNeededForCharge;
    }
}
