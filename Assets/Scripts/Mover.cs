using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {

    public float speed;
    private Rigidbody rb;

    // Use this for initialization
    void Start ()
    {
        //When an object with this is created, have it travel in a certain direction until it hits something. 
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }
}
