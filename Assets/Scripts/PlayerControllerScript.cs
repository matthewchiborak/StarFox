using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour {

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
    public Transform shotSpawn1;
    public Transform shotSpawn2;

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
    }

    //Should be used instead of update when dealing with object with rigidbody because of physics calculations
    //Done before physics calculations
    void FixedUpdate()
    {
        //Get user input and move the player if the game is still in progess
        //Want get axis because will want controller support
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = -1 * Input.GetAxis("Vertical");

        //movement = new Vector3(moveHorizontal, moveVertical, 0);
        rb.velocity = new Vector3(moveHorizontal * currentSpeed, moveVertical * verticalSpeed, 0);

        //Barrel roll functionallity
        if (Input.GetKeyDown(KeyCode.Q) && !rollingL && !rollingR)
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
        if (Input.GetKeyDown(KeyCode.E) && !rollingL && !rollingR)
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
        if (!rollingL && !rollingR)
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
                //transform.rotation = Quaternion.Lerp(bankNeutral, bankingAngle, Time.time * bankRotationSpeed);
                currentBankAngle = Mathf.Lerp(0, bankingAngle, (Time.time - startBankTime) / timeForBank);
            }
            else
            {
                currentBankAngle = Mathf.Lerp(bankingAngle, 0, (Time.time - startBankTime) / timeForBank);
            }
        }

        //If barrel rolling, set the banking angle to the current point in the barrel roll
        if (rollingL || rollingR)
        {
            if(rollingL)
            {
                currentBankAngle = Mathf.Lerp(0, 360, (Time.time - startRollTime) / durationOfRoll);
            }
            else if(rollingR)
            {
                currentBankAngle = Mathf.Lerp(0, -360, (Time.time - startRollTime) / durationOfRoll);
            }

            if((Time.time - startRollTime) > durationOfRoll)
            {
                currentBankAngle = 0;
                rollingL = false;
                rollingR = false;
                rollTrails.Stop();
            }
        }
       
        rotation = new Vector3
        (
            Mathf.Clamp(rb.velocity.y * -tilt, minRotX, maxRotX),
            Mathf.Clamp(rb.velocity.x * tilt, minRotX, maxRotX),
            currentBankAngle
        );
        rb.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        
        
        //Reset speed if 

        //Check if the player is firing
        if (Input.GetButtonDown("Fire1"))
        {
            //Create the shot
            Instantiate(laserShot, shotSpawn1.position, rb.rotation);
            Instantiate(laserShot, shotSpawn2.position, rb.rotation);
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
                currentSpeed = bankSpeed - speed;
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

            //bankNeutral = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
            //if((Input.GetKey(KeyCode.Q)))
            //{
            //    bankingAngle = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -90);
            //}
            //else
            //{
            //    bankingAngle = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 90);
            //}
            //transform.rotation = Quaternion.Lerp(bankNeutral, bankingAngle, Time.time * bankRotationSpeed);
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
            

            //Reseting banking
            //bankNeutral = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
            //bankingAngle = Quaternion.Euler(transform.rotation.x, transform.rotation.y, bankingAngle.z);
            //transform.rotation = Quaternion.Lerp(bankingAngle, bankNeutral, Time.time * bankRotationSpeed);
        }

        tilt = maxRotX / currentSpeed;
    }
}
