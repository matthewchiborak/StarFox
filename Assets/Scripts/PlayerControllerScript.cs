using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour {

    private Transform transform;
    private Rigidbody rb;
    private Vector3 movement;

    private float speed;

    // Use this for initialization
    void Start ()
    {
        transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        speed = 8;
    }
	
    //Should be used instead of update when dealing with object with rigidbody because of physics calculations
    void FixedUpdate()
    {
        //Get user input and move the player if the game is still in progess
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            movement = new Vector3(moveHorizontal * speed * Time.deltaTime, moveVertical * speed * Time.deltaTime, 0);
            rb.MovePosition(transform.position + movement);
        }

        //Check if the player is firing
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Pew pew");
        }
    }
}
