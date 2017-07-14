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

    void Start()
    {
        timeOfLastShot = Time.time - timeBetweenShots;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(Time.time - timeOfLastShot > timeBetweenShots)
        {
            //Fire a shot at the position of the player
            Quaternion newAngle = new Quaternion(0, 0, 0, 1);
            GameObject newShot = Instantiate(enemyShot, transform.position, newAngle);

            newShot.GetComponent<CollisionWithThisHurtsPlayer>().damage = damage;
            newShot.GetComponent<Rigidbody>().velocity = transform.forward.normalized * -shotSpeed;

            timeOfLastShot = Time.time;
        }
	}
}
