using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombShotControlScript : MonoBehaviour {

    public GameObject explosion;

	public void explode()
    {
        Instantiate(explosion, GetComponent<Transform>().position, GetComponent<Transform>().rotation);
        Destroy(gameObject);
    }

    //Handle Collsions
    void OnTriggerEnter(Collider other)
    {
        //Collide with bombs
        //TODO change to go off when hit enemies or level geometry
        if (!other.gameObject.CompareTag("Player") 
            && !other.gameObject.CompareTag("PlayerShot"))
        {
            explode();
        }
    }
}
