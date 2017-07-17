using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombShotControlScript : MonoBehaviour {

    public GameObject explosion;
    private bool exploded;
    public float speed;
    private Rigidbody rb;

    public GameObject homingTarget;

    void Start()
    {
        exploded = false;
        //homingTarget = null;
        rb = GetComponent<Rigidbody>();
        //rb.velocity = transform.forward * speed;
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

    void Update ()
    {
        if (homingTarget != null)
        {
            GetComponent<Rigidbody>().velocity = (homingTarget.transform.position - GetComponent<Transform>().position).normalized * speed;
        }
        else
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Transform>().forward * speed;
        }
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
