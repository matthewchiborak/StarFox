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
            //Quaternion newAngle = new Quaternion(Mathf.Atan2(player.transform.position.x - transform.position.x, player.transform.position.z - transform.position.z), Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z), 0, 1);
            Quaternion newAngle = Quaternion.Euler(Mathf.Atan2(player.transform.position.x - transform.position.x, player.transform.position.z + 85 - transform.position.z), Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.z + 85 - transform.position.z), 0);
            GameObject newShot = Instantiate(enemyShot, transform.position, newAngle);
            
            Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, player.transform.position.z - transform.position.z).normalized;
            newShot.GetComponent<Rigidbody>().velocity = direction * shotSpeed;
                //transform.forward.normalized * -shotSpeed;

            timeOfLastShot = Time.time;
        }
	}
}
