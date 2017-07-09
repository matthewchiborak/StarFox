using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionWithThisHurtsPlayer : MonoBehaviour {

    public float damage;
    public bool destroySelfOnHit;

    void OnTriggerEnter(Collider other)
    {
        //Collide with bombs
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerControllerScript>().damagePlayer(damage);

            if(destroySelfOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
