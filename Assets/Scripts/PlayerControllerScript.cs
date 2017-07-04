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

        //Banking
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
        if(isBanking)
        {
            //transform.rotation = Quaternion.Lerp(bankNeutral, bankingAngle, Time.time * bankRotationSpeed);
            currentBankAngle = Mathf.Lerp(0, bankingAngle, (Time.time - startBankTime) / timeForBank);
        }
        else
        {
            currentBankAngle = Mathf.Lerp(bankingAngle, 0, (Time.time - startBankTime) / timeForBank);
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
