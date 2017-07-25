using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailPlayer : MonoBehaviour {

    public GameObject player;

    private bool continueTailing;

    private float tilt;
    private float zTiltTurnFactor;

    private float minRotX;
    private float minRotY;
    private float maxRotX;
    private float maxRotY;
    private float maxRotZ;
    private float minRotZ;
    private float distanceAwayFromPlayerToThenDelete;

    

    // Use this for initialization
    void Start ()
    {
        continueTailing = true;
        
        zTiltTurnFactor = 0.5f;
        minRotX = -45;
        minRotY = -45;
        maxRotX = 45;
        maxRotY = 45;
        maxRotZ = 90;
        minRotZ = -90;

        distanceAwayFromPlayerToThenDelete = 300;
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<Rigidbody>().velocity = 
        //Quaternion newAngle = Quaternion.Euler(Mathf.Atan2(player.transform.position.x - transform.position.x, player.transform.position.z - transform.position.z), Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z), 0);

        PlayerControllerScript playerControllScript = player.GetComponent<PlayerControllerScript>();

        if(!playerControllScript.isInAllRange())
        { 
            if (continueTailing && !playerControllScript.getIsSomerSaulting() && player.transform.position.z > transform.position.z)
            {
                Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z).normalized;
                direction.z = 1;
                GetComponent<Rigidbody>().velocity = direction * playerControllScript.getDefaultForwardSpeed();

                tilt = maxRotX / playerControllScript.getDefaultForwardSpeed();

                //Rotate to make it look natural
                Vector3 rotation = new Vector3
                   (
                   Mathf.Clamp(GetComponent<Rigidbody>().velocity.y * -tilt, minRotX, maxRotX),
                   Mathf.Clamp(GetComponent<Rigidbody>().velocity.x * tilt, minRotX, maxRotX),
                   Mathf.Clamp(GetComponent<Rigidbody>().velocity.x * -tilt * zTiltTurnFactor, minRotZ, maxRotZ)
               );
                GetComponent<Rigidbody>().rotation = Quaternion.Euler(-rotation.x, rotation.y + 180, rotation.z);
            }
            else
            {
                continueTailing = false;
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 1) * playerControllScript.getDefaultForwardSpeed() * 2.5f;
                GetComponent<Rigidbody>().rotation = Quaternion.Euler(0, 180, 0);

                if (transform.position.z - player.transform.position.z > distanceAwayFromPlayerToThenDelete)
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            if (continueTailing && !playerControllScript.getIsSomerSaulting())
            {
                Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z).normalized;
                //direction.z = 1;
                GetComponent<Rigidbody>().velocity = direction * playerControllScript.getDefaultForwardSpeed();

                tilt = maxRotX / playerControllScript.getDefaultForwardSpeed();

                //Rotate to make it look natural
                Vector3 rotation = new Vector3
                   (
                   Mathf.Clamp(GetComponent<Rigidbody>().velocity.y * -tilt, minRotX, maxRotX),
                   Mathf.Clamp(GetComponent<Rigidbody>().velocity.x * tilt, minRotX, maxRotX),
                   Mathf.Clamp(GetComponent<Rigidbody>().velocity.x * -tilt * zTiltTurnFactor, minRotZ, maxRotZ)
               );
                GetComponent<Rigidbody>().rotation = Quaternion.Euler(-rotation.x, rotation.y + 180, rotation.z);
            }
            else
            {
                continueTailing = false;
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 1) * playerControllScript.getDefaultForwardSpeed() * 2.5f;
                GetComponent<Rigidbody>().rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
}
