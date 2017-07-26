using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAtPlayer : MonoBehaviour {

    public GameObject player;
    public GameObject enemyShot;

    public float damage;
    public float shotSpeed;

    public float timeBetweenShots;
    private float timeOfLastShot;

    private float shotLead;
    private Vector3 shotDirectionNotNorm;
    private Vector3 shotDirection;
    private float timeToReachPlayer;
    private float timeToReachPlayer2;
    private float timeToReachPlayer3;
    private float distanceToLeadShot;

    void Start()
    {
        timeOfLastShot = Time.time - timeBetweenShots;
        timeToReachPlayer = 0.5f;
        distanceToLeadShot = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (player.GetComponent<PlayerControllerScript>().isInAllRange())
        {
            //if (Time.time - timeOfLastShot > timeBetweenShots)
            //{
            //    //Fire a shot at the position of the player
            //    //Quaternion newAngle = new Quaternion(Mathf.Atan2(player.transform.position.x - transform.position.x, player.transform.position.z - transform.position.z), Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z), 0, 1);
            //    shotLead = (player.transform.position.z - transform.position.z) / (shotSpeed + player.GetComponent<PlayerControllerScript>().getCurrentSpeed());
            //    shotLead = shotSpeed * shotLead;

            //    Quaternion newAngle = Quaternion.Euler(Mathf.Atan2(player.transform.position.x - transform.position.x, player.transform.position.z + shotLead - transform.position.z), Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.z + shotLead - transform.position.z), 0);
            //    GameObject newShot = Instantiate(enemyShot, transform.position, newAngle);

            //    Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z).normalized;
            //    newShot.GetComponent<Rigidbody>().velocity = direction * shotSpeed;
            //    //transform.forward.normalized * -shotSpeed;

            //    timeOfLastShot = Time.time;
            //}
            if (Time.time - timeOfLastShot > timeBetweenShots)
            {
                //Fire a shot at the position of the player
                //Quaternion newAngle = new Quaternion(Mathf.Atan2(player.transform.position.x - transform.position.x, player.transform.position.z - transform.position.z), Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z), 0, 1);
                //shotLead = (player.transform.position.z - transform.position.z) / (shotSpeed + player.GetComponent<PlayerControllerScript>().getCurrentSpeed());
                //shotLead = shotSpeed * shotLead;
                //shotDirection = new Vector3(
                //    (player.transform.position.x - transform.position.x) / (player.GetComponent<PlayerControllerScript>().getForward().x * (shotSpeed + player.GetComponent<PlayerControllerScript>().getCurrentSpeed())),
                //    (player.transform.position.y - transform.position.y) / (player.GetComponent<PlayerControllerScript>().getForward().y * (shotSpeed + player.GetComponent<PlayerControllerScript>().getCurrentSpeed())),
                //    (player.transform.position.z - transform.position.z) / (player.GetComponent<PlayerControllerScript>().getForward().z * (shotSpeed + player.GetComponent<PlayerControllerScript>().getCurrentSpeed()))
                //    );

                //Estimate the time to reach player
                Vector3 normDirection = player.GetComponent<PlayerControllerScript>().getForward().normalized;
                timeToReachPlayer = -1 * ((((shotSpeed * (player.transform.position.z + distanceToLeadShot) - player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.z * transform.position.z) / (shotSpeed - player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.z)) - player.transform.position.z + distanceToLeadShot) / player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.z);
                timeToReachPlayer2 = -1 * ((((shotSpeed * (player.transform.position.x + distanceToLeadShot) - player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.x * transform.position.x) / (shotSpeed - player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.x)) - player.transform.position.x + distanceToLeadShot) / player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.x);
                timeToReachPlayer3 = -1 * ((((shotSpeed * (player.transform.position.y + distanceToLeadShot) - player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.y * transform.position.y) / (shotSpeed - player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.y)) - player.transform.position.y + distanceToLeadShot) / player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity() * normDirection.y);

                timeToReachPlayer = (timeToReachPlayer + timeToReachPlayer2 + timeToReachPlayer3) / 3;

                shotDirection = new Vector3(
                    //(player.transform.position.x - transform.position.x) / (player.GetComponent<PlayerControllerScript>().getForward().x * (shotSpeed + player.GetComponent<PlayerControllerScript>().getCurrentSpeed())),
                    //((player.GetComponent<PlayerControllerScript>().getForward().x * player.GetComponent<PlayerControllerScript>().getCurrentSpeed() * transform.position.x) - ()) / (),
                    //(player.transform.position.y - transform.position.y) / (player.GetComponent<PlayerControllerScript>().getForward().y * (shotSpeed + player.GetComponent<PlayerControllerScript>().getCurrentSpeed())),
                    //(player.transform.position.z - transform.position.z) / (player.GetComponent<PlayerControllerScript>().getForward().z * (shotSpeed + player.GetComponent<PlayerControllerScript>().getCurrentSpeed()))
                    (transform.position.x - player.transform.position.x + (timeToReachPlayer * player.GetComponent<PlayerControllerScript>().getForward().x * player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity())),
                    (transform.position.y - player.transform.position.y + (timeToReachPlayer * player.GetComponent<PlayerControllerScript>().getForward().y * player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity())),
                    (transform.position.z - player.transform.position.z + (timeToReachPlayer * player.GetComponent<PlayerControllerScript>().getForward().z * player.GetComponent<PlayerControllerScript>().getCurrentForwardVelocity()))
                );


                //Check if dividing by infinity
                //if((player.GetComponent<PlayerControllerScript>().getForward().x == 0))
                //{
                //    shotDirection.x = (player.transform.position.x - transform.position.x);
                //}
                //if ((player.GetComponent<PlayerControllerScript>().getForward().y == 0))
                //{
                //    shotDirection.y = (player.transform.position.y - transform.position.y);
                //}
                //if ((player.GetComponent<PlayerControllerScript>().getForward().z == 0))
                //{
                //    shotDirection.z = (player.transform.position.z - transform.position.z);
                //}

                shotDirection = shotDirection.normalized;

                shotDirection = new Vector3(
                    shotDirection.x * -shotSpeed,//(shotDirectionNotNorm.x / timeToReachPlayer),//-shotSpeed,
                    shotDirection.y * -shotSpeed, //(shotDirectionNotNorm.y / timeToReachPlayer),//-shotSpeed,
                    shotDirection.z * -shotSpeed //(shotDirectionNotNorm.z / timeToReachPlayer)//-shotSpeed
                    );

                //if(shotDirectionNotNorm.x > 0)
                //{
                //    shotDirection.x *= -1;
                //}
                //if (shotDirectionNotNorm.y > 0)
                //{
                //    shotDirection.y *= -1;
                //}
                //if (shotDirectionNotNorm.z > 0)
                //{
                //    shotDirection.z *= -1;
                //}


                Quaternion newAngle = Quaternion.Euler(Mathf.Atan2(player.transform.position.x - transform.position.x, player.transform.position.z + shotLead - transform.position.z), Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.z + shotLead - transform.position.z), 0);
                GameObject newShot = Instantiate(enemyShot, transform.position, newAngle);

                //Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z).normalized;
                newShot.GetComponent<Rigidbody>().velocity = shotDirection;// * shotSpeed;
                                                                           //transform.forward.normalized * -shotSpeed;

                timeOfLastShot = Time.time;
            }
        }
        else
        {
            if (Time.time - timeOfLastShot > timeBetweenShots)
            {
                //Fire a shot at the position of the player
                //Quaternion newAngle = new Quaternion(Mathf.Atan2(player.transform.position.x - transform.position.x, player.transform.position.z - transform.position.z), Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z), 0, 1);
                shotLead = (player.transform.position.z - transform.position.z) / (shotSpeed + player.GetComponent<PlayerControllerScript>().getCurrentSpeed());
                shotLead = shotSpeed * shotLead;

                Quaternion newAngle = Quaternion.Euler(Mathf.Atan2(player.transform.position.x - transform.position.x, player.transform.position.z + shotLead - transform.position.z), Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.z + shotLead - transform.position.z), 0);
                GameObject newShot = Instantiate(enemyShot, transform.position, newAngle);

                Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z).normalized;
                newShot.GetComponent<Rigidbody>().velocity = direction * shotSpeed;
                //transform.forward.normalized * -shotSpeed;

                timeOfLastShot = Time.time;
            }
        }
    }
}
