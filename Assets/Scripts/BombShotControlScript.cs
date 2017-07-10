using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombShotControlScript : MonoBehaviour {

    public GameObject explosion;
    private bool exploded;

    void Start()
    {
        exploded = false;
    }

	public void explode()
    {
        if(exploded)
        {
            return;
        }

        exploded = true;

        Destroy(gameObject);
        Instantiate(explosion, GetComponent<Transform>().position, GetComponent<Transform>().rotation);
    }

    //Handle Collsions
    void OnTriggerEnter(Collider other)
    {
        //Collide with bombs
        //TODO change to go off when hit enemies or level geometry
        //if (!other.gameObject.CompareTag("Player") 
        //    && !other.gameObject.CompareTag("PlayerShot"))
        if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Untagged"))
        {
            explode();
        }
    }
}
